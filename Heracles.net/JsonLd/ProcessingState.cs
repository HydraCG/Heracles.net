using System;
using System.Collections.Generic;
using Heracles.Entities;
using Heracles.Namespaces;
using RDeF.Entities;

namespace Heracles.JsonLd
{
    internal sealed class ProcessingState : IDisposable
    {
        private readonly IOntologyProvider _ontologyProvider;
        private readonly IDictionary<Iri, ISet<Statement>> _statements;

        internal ProcessingState(IEntityContext context, IOntologyProvider ontologyProvider, Iri rootResource, LinksPolicy linksPolicy)
        {
            _ontologyProvider = ontologyProvider;
            _statements = new Dictionary<Iri, ISet<Statement>>();
            Context = context;
            ForbiddenHypermeda = new HashSet<Iri>();
            AllHypermedia = new HashSet<Iri>();
            BaseUrl = rootResource;
            Root = rootResource.ToRoot();
            LinksPolicy = linksPolicy;
        }

        internal event EventHandler<EventArgs> ProcessingCompleted;

        internal IEntityContext Context { get; }

        internal LinksPolicy LinksPolicy { get; }

        internal Iri BaseUrl { get; }

        internal Iri Root { get; }

        internal ISet<Iri> ForbiddenHypermeda { get; }

        internal ISet<Iri> AllHypermedia { get; }

        internal IEnumerable<Statement> StatementsOf(Iri iri)
        {
            return _statements.TryGetValue(iri, out ISet<Statement> result) ? result : (IEnumerable<Statement>)Array.Empty<Statement>();
        }

        internal IDisposable StartGatheringStatementsFor(ISerializableEntitySource entitySource)
        {
            return new StatementGatheringProcessingState(entitySource, _ontologyProvider, _statements, ForbiddenHypermeda);
        }

        internal void MarkAsOwned(Iri iri)
        {
            AllHypermedia.Remove(iri);
            ForbiddenHypermeda.Add(iri);
        }

        void IDisposable.Dispose()
        {
            ProcessingCompleted?.Invoke(this, EventArgs.Empty);
        }

        private sealed class StatementGatheringProcessingState : IDisposable
        {
            private readonly ISerializableEntitySource _entitySource;
            private readonly IOntologyProvider _ontologyProvider;
            private readonly IDictionary<Iri, ISet<Statement>> _statements;
            private readonly ISet<Iri> _forbiddenHypermedia;
            private Iri _lastSetIri;
            private ISet<Statement> _lastSet;

            internal StatementGatheringProcessingState(
                ISerializableEntitySource entitySource,
                IOntologyProvider ontologyProvider,
                IDictionary<Iri, ISet<Statement>> statements,
                ISet<Iri> forbiddenHypermedia)
            {
                _entitySource = entitySource;
                _ontologyProvider = ontologyProvider;
                _statements = statements;
                _forbiddenHypermedia = forbiddenHypermedia;
                _entitySource.StatementAsserted += OnStatementAsserted;
            }

            void IDisposable.Dispose()
            {
                _entitySource.StatementAsserted -= OnStatementAsserted;
            }

            private void OnStatementAsserted(object sender, StatementEventArgs e)
            {
                EnsureSetFor(e.Statement.Subject).Add(e.Statement);
                if (e.Statement.Object != null && e.Statement.Predicate.ToString().StartsWith(hydra.Namespace))
                {
                    _forbiddenHypermedia.Add(e.Statement.Object);
                }

                var type = _ontologyProvider.GetDomainFor(e.Statement.Predicate);
                if (type != null)
                {
                    _statements[e.Statement.Subject].Add(Assert(new Statement(e.Statement.Subject, rdf.type, type), e));
                }

                if (e.Statement.Object != null)
                {
                    type = _ontologyProvider.GetRangeFor(e.Statement.Predicate);
                    if (type != null)
                    {
                        EnsureSetFor(e.Statement.Object).Add(Assert(new Statement(e.Statement.Object, rdf.type, type), e));
                    }
                }
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

            private Statement Assert(Statement statement, StatementEventArgs e)
            {
                e.AdditionalStatementsToAssert.Add(statement);
                return statement;
            }
        }
    }
}
