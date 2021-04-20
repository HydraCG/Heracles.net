using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Heracles.DataModel;
using Heracles.DataModel.Collections;
using NUnit.Framework;
using RDeF.Entities;
using RollerCaster;

namespace Given_instance_of.JsonLdHypermediaProcessor_class.when_parsing.JSON_LD_response
{
    [TestFixture]
    public class with_templated_operation : ScenarioTest
    {
        private Stream _inputJsonLd;

        protected override Stream InputJsonLd
        {
            get { return _inputJsonLd; }
        }
        
        protected override Uri Uri { get; } = new Uri("http://temp.uri/api/people", UriKind.Absolute);

        private IOperation AddPerson { get; set; }
        
        public override void ScenarioSetup()
        {
            _inputJsonLd = GetResourceNamed("operationInput.json");
            Response.SetupGet(_ => _.Url).Returns(Uri);
            base.ScenarioSetup();
        }

        public override async Task TheTest()
        {
            await base.TheTest();
            AddPerson = Result.Operations.WithTemplate().First();
        }

        [Test]
        public void should_point_to_the_collection()
        {
            AddPerson.ActLike<ITemplatedOperation>().ExpandTarget(new Dictionary<string, string>() { { "name", "test" } })
                .Target.Iri.Should().Be(new Iri("http://temp.uri/api/people/test"));
        }
    }
}
