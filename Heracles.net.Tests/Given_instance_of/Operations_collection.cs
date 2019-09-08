using System.Collections.Generic;
using FluentAssertions;
using Heracles.DataModel;
using Heracles.DataModel.Collections;
using Heracles.Namespaces;
using Heracles.Testing;
using Moq;
using NUnit.Framework;
using RDeF.Entities;

namespace Given_instance_of
{
    [TestFixture]
    public class Operations_collection
    {
        protected Mock<IOperation> Operation1 { get; private set; }

        protected Mock<IOperation> Operation2 { get; private set; }

        protected Mock<IOperation> Operation3 { get; private set; }

        protected Mock<IOperation> Operation4 { get; private set; }

        protected ICollection<IOperation> Operations { get; private set; }

        [Test]
        public void should_provide_only_type_matching_operations()
        {
            Operations.OfType(new Iri("OperationType1")).Should().BeEquivalentTo(Operation1.Object, Operation2.Object);
        }

        [Test]
        public void should_provide_only_type_and_expected_type_matching_operations()
        {
            Operations.OfType(new Iri("OperationType1")).Expecting(new Iri("ExpectedType2")).Should().BeEquivalentTo(Operation2.Object);
        }

        [Test]
        public void should_provide_only_type_and_returned_type_matching_operations()
        {
            Operations.OfType(new Iri("OperationType1")).Returning(new Iri("ReturnedType2")).Should().BeEquivalentTo(Operation2.Object);
        }

        [Test]
        public void should_provide_only_expected_headers_matching_operations()
        {
            Operations.ExpectingHeader("InHeader1").Should().BeEquivalentTo(Operation1.Object);
        }

        [Test]
        public void should_provide_only_returned_headers_matching_operations()
        {
            Operations.ReturningHeader("OutHeader1").Should().BeEquivalentTo(Operation1.Object);
        }

        [Test]
        public void should_provide_only_method_matching_operations()
        {
            Operations.OfMethod("SOME").Should().BeEquivalentTo(Operation1.Object);
        }

        [Test]
        public void should_provide_only_templated_operations()
        {
            Operations.WithTemplate().Should().BeEquivalentTo(Operation4.Object);
        }

        [SetUp]
        public void Setup()
        {
            Operation1 = Resource.Of<IOperation>()
                .With(_ => _.Type, new HashSet<Iri>() { hydra.Operation, new Iri("OperationType1") })
                .With(_ => _.Expects, new HashSet<IClass>() { Resource.Of<IClass>(new Iri("ExpectedType1")).Object })
                .With(_ => _.Returns, new HashSet<IClass>() { Resource.Of<IClass>(new Iri("ReturnedType1")).Object })
                .With(_ => _.ExpectedHeaders, new HashSet<string>() { "InHeader1" })
                .With(_ => _.ReturnedHeaders, new HashSet<string>() { "OutHeader1" })
                .With(_ => _.Method, "SOME");
            Operation2 = Resource.Of<IOperation>()
                .With(_ => _.Type, new HashSet<Iri>() { hydra.Operation, new Iri("OperationType1") })
                .With(_ => _.Expects, new HashSet<IClass>() { Resource.Of<IClass>(new Iri("ExpectedType2")).Object })
                .With(_ => _.Returns, new HashSet<IClass>() { Resource.Of<IClass>(new Iri("ReturnedType2")).Object })
                .With(_ => _.ExpectedHeaders, new HashSet<string>() { "InHeader2" })
                .With(_ => _.ReturnedHeaders, new HashSet<string>() { "OutHeader2" })
                .With(_ => _.Method, "TEST");
            Operation3 = Resource.Of<IOperation>()
                .With(_ => _.Type, new HashSet<Iri>() { hydra.Operation, new Iri("OperationType2") })
                .With(_ => _.Expects, new HashSet<IClass>() { Resource.Of<IClass>(new Iri("ExpectedType2")).Object })
                .With(_ => _.Returns, new HashSet<IClass>() { Resource.Of<IClass>(new Iri("ReturnedType2")).Object })
                .With(_ => _.ExpectedHeaders, new HashSet<string>() { "InHeader2" })
                .With(_ => _.ReturnedHeaders, new HashSet<string>() { "OutHeader2" })
                .With(_ => _.Method, "TEST");
            Operation4 = Resource.Of<IOperation>()
                .With(_ => _.Type, new HashSet<Iri>() { hydra.Operation, new Iri("OperationType3"), hydra.IriTemplate })
                .With(_ => _.Expects, new HashSet<IClass>() { Resource.Of<IClass>(new Iri("ExpectedType3")).Object })
                .With(_ => _.Returns, new HashSet<IClass>() { Resource.Of<IClass>(new Iri("ReturnedType3")).Object })
                .With(_ => _.ExpectedHeaders, new HashSet<string>() { "InHeader3" })
                .With(_ => _.ReturnedHeaders, new HashSet<string>() { "OutHeader3" })
                .With(_ => _.Method, "ANY");
            Operations = new List<IOperation>() { Operation1.Object, Operation2.Object, Operation3.Object, Operation4.Object };
        }
    }
}
