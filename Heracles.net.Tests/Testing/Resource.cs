using System;
using System.Linq.Expressions;
using Heracles.DataModel;
using Moq;
using RDeF.Entities;

namespace Heracles.Testing
{
    internal static class Resource
    {
        internal static readonly Iri Null = new Iri("_:null");

        internal static Mock<T> Of<T>(Iri iri = null) where T : class, IResource
        {
            var actualIri = iri ?? new Iri();
            var result = new Mock<T>(MockBehavior.Strict);
            result.As<IEntity>().SetupGet(_ => _.Iri).Returns(iri == Null ? null : actualIri);
            result.Setup(_ => _.GetHashCode()).Returns(actualIri.GetHashCode());
            result.Setup(_ => _.Equals(It.IsAny<object>())).Returns<object>(_ => result.Object == _);
            return result;
        }

        internal static Mock<T> With<T, TProperty>(this Mock<T> resource, Expression<Func<T, TProperty>> property, TProperty value)
            where T : class
        {
            resource.SetupGet(property).Returns(value);
            return resource;
        }

        internal static Mock<T> With<T, TEntity, TProperty>(this Mock<T> resource, Expression<Func<TEntity, TProperty>> property, TProperty value)
            where T : class
            where TEntity : class
        {
            resource.As<TEntity>().SetupGet(property).Returns(value);
            return resource;
        }
    }
}
