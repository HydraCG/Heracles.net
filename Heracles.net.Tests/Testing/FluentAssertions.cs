using System.Text.RegularExpressions;
using FluentAssertions;
using FluentAssertions.Collections;
using RDeF.Entities;

namespace Heracles.Testing
{
    public static class FluentAssertions
    {
        public static AndWhichConstraint<GenericCollectionAssertions<T>, T> Contain<T>(
            this GenericCollectionAssertions<T> assertion,
            Regex iriPattern)
            where T : IEntity
        {
            return assertion.Contain(_ => iriPattern.IsMatch(_.Iri.ToString()));
        }
    }
}
