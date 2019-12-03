using System;
using System.Collections.Generic;
using Heracles.Entities;
using Heracles.Namespaces;
using RDeF.Entities;

namespace Heracles.Rdf
{
    internal sealed class ProcessingState : IDisposable
    {
        private readonly IOntologyProvider _ontologyProvider;
        private readonly IDictionary<Iri, ISet<Statement>> _statements;
        private readonly IDictionary<Iri, int> _statementsStatistics;

        internal ProcessingState(
            IEntityContext context,
            IOntologyProvider ontologyProvider,
            Iri rootResource,
            LinksPolicy linksPolicy,
            string originatingMediaType)
        {
            _ontologyProvider = ontologyProvider;
            _statements = new Dictionary<Iri, ISet<Statement>>();
            _statementsStatistics = new Dictionary<Iri, int>();
            Context = context;
            ForbiddenHypermeda = new HashSet<Iri>();
            AllHypermedia = new HashSet<Iri>();
            BaseUrl = rootResource;
            Root = rootResource.ToRoot();
            LinksPolicy = linksPolicy;
            OriginatingMediaType = originatingMediaType;
        }

        internal event EventHandler<EventArgs> ProcessingCompleted;

        internal IEntityContext Context { get; }

        internal LinksPolicy LinksPolicy { get; }

        internal Iri BaseUrl { get; }

        internal Iri Root { get; }

        internal string OriginatingMediaType { get; }

        internal ISet<Iri> ForbiddenHypermeda { get; }

        internal ISet<Iri> AllHypermedia { get; }

        void IDisposable.Dispose()
        {
            ProcessingCompleted?.Invoke(this, EventArgs.Empty);
        }

        internal IEnumerable<Statement> StatementsOf(Iri iri)
        {
            return _statements.TryGetValue(iri, out ISet<Statement> result) ? result : (IEnumerable<Statement>)Array.Empty<Statement>();
        }

        internal int NumberOfStatementsOf(Iri iri)
        {
            return _statementsStatistics.TryGetValue(iri, out int result) ? result : 0;
        }

        internal IDisposable StartGatheringStatementsFor(ISerializableEntitySource entitySource, Func<Statement, bool> statementsFilter)
        {
            return new StatementGatheringProcessingState(
                entitySource,
                _ontologyProvider,
                _statements,
                _statementsStatistics,
                ForbiddenHypermeda,
                statementsFilter);
        }

        internal void MarkAsOwned(Iri iri)
        {
            AllHypermedia.Remove(iri);
            ForbiddenHypermeda.Add(iri);
        }

        private sealed class StatementGatheringProcessingState : IDisposable
        {
            private readonly ISerializableEntitySource _entitySource;
            private readonly IOntologyProvider _ontologyProvider;
            private readonly IDictionary<Iri, ISet<Statement>> _statements;
            private readonly IDictionary<Iri, int> _statementsStatistics;
            private readonly ISet<Iri> _forbiddenHypermedia;
            private readonly Func<Statement, bool> _statementsFilter;
            private Iri _lastSetIri;
            private ISet<Statement> _lastSet;

            internal StatementGatheringProcessingState(
                ISerializableEntitySource entitySource,
                IOntologyProvider ontologyProvider,
                IDictionary<Iri, ISet<Statement>> statements,
                IDictionary<Iri, int> statementsStatistics,
                ISet<Iri> forbiddenHypermedia,
                Func<Statement, bool> statementsFilter)
            {
                _entitySource = entitySource;
                _ontologyProvider = ontologyProvider;
                _statements = statements;
                _statementsStatistics = statementsStatistics;
                _forbiddenHypermedia = forbiddenHypermedia;
                _statementsFilter = statementsFilter;
                _entitySource.StatementAsserted += OnStatementAsserted;
            }

            void IDisposable.Dispose()
            {
                _entitySource.StatementAsserted -= OnStatementAsserted;
            }

            private void OnStatementAsserted(object sender, StatementEventArgs e)
            {
                EnsureStatisticsFor(e.Statement.Subject)[e.Statement.Subject]++;
                if (_statementsFilter(e.Statement))
                {
                    EnsureSetFor(e.Statement.Subject).Add(e.Statement);
                }

                var type = _ontologyProvider.GetDomainFor(e.Statement.Predicate);
                if (type != null)
                {
                    Assert(e.Statement.Subject, new Statement(e.Statement.Subject, rdf.type, type), e);
                }

                if (e.Statement.Object != null)
                {
                    if (e.Statement.Predicate.ToString().StartsWith(hydra.Namespace))
                    {
                        _forbiddenHypermedia.Add(e.Statement.Object);
                    }

                    type = _ontologyProvider.GetRangeFor(e.Statement.Predicate);
                    if (type != null)
                    {
                        Assert(e.Statement.Object, new Statement(e.Statement.Object, rdf.type, type), e);
                    }
                }
            }

            private IDictionary<Iri, int> EnsureStatisticsFor(Iri iri)
            {
                if (!_statementsStatistics.ContainsKey(iri))
                {
                    _statementsStatistics[iri] = 0;
                }

                return _statementsStatistics;
            }

            private ISet<Statement> EnsureSetFor(Iri iri)
            {
                ISet<Statement> set = _lastSet;
                if (_lastSetIri != iri)
                {
                    if (!_statements.TryGetValue(iri, out set))
                    {
                        set = new HashSet<Statement>();
                        _statements[iri] = set;
                    }

                    _lastSet = set;
                    _lastSetIri = iri;
                }

                return set;
            }

            private void Assert(Iri owner, Statement statement, StatementEventArgs e)
            {
                e.AdditionalStatementsToAssert.Add(statement);
                if (_statementsFilter(statement))
                {
                    EnsureSetFor(owner).Add(statement);
                }
            }
        }
    }
}
