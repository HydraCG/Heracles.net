﻿using System.Threading;
using FluentAssertions;
using Heracles;
using Heracles.DataModel;
using Heracles.Testing;
using Moq;
using NUnit.Framework;

namespace Given_instance_of.HydraClient_class.when_fetching_a_resource
{
    [TestFixture]
    public class and_that_resource_was_not_found : ScenarioTest
    {
        public override void ScenarioSetup()
        {
            base.ScenarioSetup();
            HttpCall.Setup(_ => _.HttpCall(ResourceUrl, It.IsAny<IHttpOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Return.NotFound());
        }

        [Test]
        public void should_throw()
        {
            Client.Awaiting(_ => _.GetResource(Resource.Of<IResource>(ResourceUrl).Object))
                .Should().Throw<InvalidResponseException>()
                .Which.Status.Should().Be(404);
        }
    }
}
