using System;
using FluentAssertions;
using Heracles;
using Moq;
using NUnit.Framework;

namespace Given_instance_of.HypermediaProcessingOptions_class
{
    [TestFixture]
    public class when_merging
    {
        private HypermediaProcessingOptions PrimaryOptions { get; set; }

        private HypermediaProcessingOptions SecondaryOptions { get; set; }

        private HypermediaProcessingOptions Result { get; set; }

        public void TheTest()
        {
            Result = PrimaryOptions.MergeWith(SecondaryOptions);
        }

        [Test]
        public void Should_use_primary_options_original_url()
        {
            Result.OriginalUrl.Should().Be(PrimaryOptions.OriginalUrl);
        }

        [Test]
        public void Should_use_primary_options_links_policy()
        {
            Result.LinksPolicy.Should().Be(PrimaryOptions.LinksPolicy);
        }

        [Test]
        public void Should_use_secondary_options_aux_response()
        {
            Result.AuxiliaryResponse.Should().Be(SecondaryOptions.AuxiliaryResponse);
        }

        [Test]
        public void Should_use_secondary_options_aux_original_url()
        {
            Result.AuxiliaryOriginalUrl.Should().Be(SecondaryOptions.AuxiliaryOriginalUrl);
        }

        [SetUp]
        public void Setup()
        {
            PrimaryOptions = new HypermediaProcessingOptions(new Uri("http://temp.uri/"), LinksPolicy.AllHttp);
            SecondaryOptions = new HypermediaProcessingOptions(
                new Mock<IResponse>(MockBehavior.Strict).Object,
                new Uri("http://temp.uri/aux"),
                null,
                LinksPolicy.SameRoot);
            TheTest();
        }
    }
}
