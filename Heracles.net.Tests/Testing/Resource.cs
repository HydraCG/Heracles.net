using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Heracles.DataModel;
using Moq;
using RDeF.Entities;
using RDeF.Serialization;

namespace Heracles.Testing
{
    internal static class Resource
    {
        internal static readonly Iri Null = new Iri("_:null");

        internal static async Task<Stream> AsStream<TEntity>(this TEntity entity) where TEntity : IResource
        {
            var result = new MemoryStream();
            var serializer = new JsonLdWriter();
            using (var writer = new StreamWriter(result, Encoding.UTF8, 4096, true))
            {
                var graphs =
                    from statement in await entity.Context.EntitySource.Load(entity.Iri)
                    group statement by statement.Graph into graphStatements
                    select new KeyValuePair<Iri, IEnumerable<Statement>>(
                        graphStatements.Key,
                        graphStatements);
                await serializer.Write(writer, graphs);
            }

            result.Seek(0, SeekOrigin.Begin);
            return result;
        }

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
