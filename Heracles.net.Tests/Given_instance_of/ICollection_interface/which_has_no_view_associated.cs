using FluentAssertions;
using NUnit.Framework;

namespace Given_instance_of.ICollection_interface
{
    [TestFixture]
    public class which_has_no_view_associated : CollectionTest
    {
        [Test]
        public void Should_not_have_that_view_available()
        {
            Collection.GetIterator().Should().BeNull();
        }
    }
}
