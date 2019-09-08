using System.Collections.Generic;
using FluentAssertions;
using Heracles.DataModel.Collections;
using NUnit.Framework;
using RDeF.Entities;

namespace Given_instance_of
{
    [TestFixture]
    public class Types_collection
    {
        protected static readonly Iri Type1 = new Iri("type1");

        protected static readonly Iri Type2 = new Iri("type2");

        protected ICollection<Iri> Type { get; private set; }

        [Test]
        public void should_exclude_required_type()
        {
            Type.Except(Type1).Should().BeEquivalentTo(Type2);
        }

        [SetUp]
        public void Setup()
        {
            Type = new List<Iri>() { Type1, Type2 };
        }
    }
}
