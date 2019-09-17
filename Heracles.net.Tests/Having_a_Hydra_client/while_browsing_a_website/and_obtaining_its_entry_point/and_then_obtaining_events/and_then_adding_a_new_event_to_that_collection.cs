using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using FluentAssertions;
using Heracles;
using Heracles.Data.Model;
using Heracles.DataModel.Collections;
using NUnit.Framework;
using RDeF.Entities;
using Heracles.Testing;

namespace Having_a_Hydra_client.while_browsing_a_website.and_obtaining_its_entry_point.and_then_obtaining_events
{
    [TestFixture]
    public class and_then_adding_a_new_event_to_that_collection : ScenarioTest
    {
        private IEvent Body { get; set; }

        private IResponse CreatedEvent { get; set; }

        private Exception Exception { get; set; }

        public override async Task TheTest()
        {
            await base.TheTest();
            try
            {
                Body = Events.Context.Create<IEvent>(new Iri());
                await Events.Context.Commit(new[] { Body.Iri });
                var operation = Events.Operations
                    .OfType(new Iri("http://schema.org/CreateAction"))
                    .Expecting(new Iri("http://schema.org/Event"))
                    .First();
                CreatedEvent = await Client.Invoke(operation, Body);
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
            CreatedEvent.Status.Should().Be(201);
        }

        [Test]
        public async Task should_provide_a_resources_URL()
        {
            CreatedEvent.Headers["Location"].Should().HaveCount(1)
                .And.Subject.First().Should().MatchRegex(".*/api/events/" + 
                    Convert.ToBase64String(MD5.Create().ComputeHash(await Body.AsStream())).Replace("+", "\\+"));
        }
    }
}
