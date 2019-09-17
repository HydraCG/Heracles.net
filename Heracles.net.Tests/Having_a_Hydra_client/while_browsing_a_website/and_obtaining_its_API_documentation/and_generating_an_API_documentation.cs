using System.Threading.Tasks;
using FluentAssertions;
using Heracles.Data.Model;
using NUnit.Framework;
using RDeF.Entities;

namespace Having_a_Hydra_client.while_browsing_a_website.and_obtaining_its_API_documentation
{
    [TestFixture]
    public class and_generating_an_API_documentation : ScenarioTest
    {
        private UserGuide UserGuide { get; set; }

        public override async Task TheTest()
        {
            await base.TheTest();
            foreach (var supportedClass in ApiDocumentation.SupportedClasses)
            {
                var classDocumentation =
                    $"##Class {supportedClass.DisplayName} ({supportedClass.Iri})\n\n" +
                    $"{supportedClass.TextDescription}\n\n" +
                    "###Properties:\n\n";
                foreach (var supportedProperty in supportedClass.SupportedProperties)
                {
                    classDocumentation +=
                        $"####{supportedProperty.Property.DisplayName} ({supportedProperty.Property.Iri})\n\n" +
                        $"{supportedProperty.Property.TextDescription}\n\n" +
                        "Values of type:\n";
                    foreach (var valueOfType in supportedProperty.Property.ValuesOfType)
                    {
                        classDocumentation += $"- {valueOfType.Iri}\n";
                    }

                    classDocumentation += "\n\n";
                }

                UserGuide.Classes[supportedClass.Iri] = classDocumentation;
            }
        }

        /// <summary>Use case 2.2. API documentation user document.</summary>
        [Test]
        public void should_provide_all_details_for_the_user_guide_as_in_use_case_2_2()
        {
            UserGuide.Classes.Should().ContainKey(new Iri("http://schema.org/Event"))
                .WhichValue.Should().Be(
                    "##Class Event (http://schema.org/Event)\n\n" +
                    "An event happening at a certain time and location, such as a concert, lecture, or festival.\n\n" +
                    "###Properties:\n\n" +
                    "####Name (http://schema.org/name)\n\n" +
                    "The name of the event.\n\n" +
                    "Values of type:\n- http://www.w3.org/2001/XMLSchema#string\n\n\n" +
                    "####Description (http://schema.org/description)\n\n" +
                    "Description of the event.\n\n" +
                    "Values of type:\n- http://www.w3.org/2001/XMLSchema#string\n\n\n" +
                    "####Start date (http://schema.org/startDate)\n\n" +
                    "The start date and time of the item (in ISO 8601 date format).\n\n" +
                    "Values of type:\n" +
                    "- http://www.w3.org/2001/XMLSchema#dateTime\n" +
                    "- http://www.w3.org/2001/XMLSchema#date\n\n\n" +
                    "####End date (http://schema.org/endDate)\n\n" +
                    "The end date and time of the item (in ISO 8601 date format).\n\n" +
                    "Values of type:\n" +
                    "- http://www.w3.org/2001/XMLSchema#dateTime\n" +
                    "- http://www.w3.org/2001/XMLSchema#date\n\n\n");
        }

        protected override void ScenarioSetup()
        {
            base.ScenarioSetup();
            UserGuide = new UserGuide();
        }
    }
}
