using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using FluentAssertions;
using Heracles;
using NUnit.Framework;

namespace Given_instance_of.InvalidResponseException_class
{
    [TestFixture]
    public class when_serializing
    {
        private SerializationInfo Info { get; set; }

        private StreamingContext Context { get; set; }

        private InvalidResponseException Result { get; set; }

        public void TheTest()
        {
            new InvalidResponseException(404).GetObjectData(Info, Context);
            Result = (InvalidResponseException)typeof(InvalidResponseException)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                .First()
                .Invoke(new object[] { Info, Context });
        }

        [Test]
        public void Should_deserialize_instance_with_rellevant_properties()
        {
            Result.Status.Should().Be(404);
        }

        [SetUp]
        public void Setup()
        {
            Info = new SerializationInfo(typeof(InvalidResponseException), new FormatterConverter());
            Context = new StreamingContext();
            TheTest();
        }
    }
}
