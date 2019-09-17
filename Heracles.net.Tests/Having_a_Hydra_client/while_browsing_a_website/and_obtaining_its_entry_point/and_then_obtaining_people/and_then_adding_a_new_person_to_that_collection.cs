using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Heracles;
using Heracles.Data.Model;
using Heracles.DataModel.Collections;
using NUnit.Framework;
using RDeF.Entities;

namespace Having_a_Hydra_client.while_browsing_a_website.and_obtaining_its_entry_point.and_then_obtaining_people
{
    [TestFixture]
    public class and_then_adding_a_new_person_to_that_collection : ScenarioTest
    {
        private IPerson Body { get; set; }

        private IResponse CreatedPerson { get; set; }

        private Exception Exception { get; set; }

        public override async Task TheTest()
        {
            await base.TheTest();
            try
            {
                Body = People.Context.Create<IPerson>(new Iri());
                Body.Name = "test-name";
                await People.Context.Commit(new[] { Body.Iri });
                var operation = People.Operations
                    .OfType(new Iri("http://schema.org/UpdateAction"))
                    .Expecting(new Iri("http://schema.org/Person"))
                    .First();
                CreatedPerson = await Client.Invoke(operation, Body);
            }
            catch (Exception error)
            {
                Exception = error;
            }
        }

        [Test]
        public void should_not_throw()
        {
            Exception.Should().BeNull();
        }

        [Test]
        public void should_return_with_a_201_Created()
        {
            CreatedPerson.Status.Should().Be(201);
        }
    }
}
