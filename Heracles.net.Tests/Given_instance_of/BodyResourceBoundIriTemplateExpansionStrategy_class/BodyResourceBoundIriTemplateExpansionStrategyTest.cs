using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Heracles;
using Heracles.Data.Model;
using Heracles.DataModel;
using Moq;
using NUnit.Framework;
using RDeF.Entities;
using RDeF.Mapping;
using RollerCaster;
using PropertyAttribute = RDeF.Mapping.Attributes.PropertyAttribute;

namespace Given_instance_of.BodyResourceBoundIriTemplateExpansionStrategy_class
{
    public abstract class BodyResourceBoundIriTemplateExpansionStrategyTest
    {
        protected IEnumerable<IIriTemplateMapping> Mappings { get; private set; }

        protected BodyResourceBoundIriTemplateExpansionStrategy Strategy { get; private set; }

        protected MappingsBuilder Builder { get; private set; }

        protected IEvent Body { get; private set; }

        protected IEvent Parameters { get; private set; }

        public virtual void TheTest()
        {
        }

        [SetUp]
        public void Setup()
        {
            Mappings = new[]
            {
                MappingOf(new Iri("http://schema.org/name"), "eventName"),
                MappingOf(new Iri("http://schema.org/description"), "eventDescription"),
            };
            Builder = new MappingsBuilder(Mappings);
            Strategy = new BodyResourceBoundIriTemplateExpansionStrategy();
            var mappingsRepository = new Mock<IMappingsRepository>(MockBehavior.Strict);
            mappingsRepository.Setup(_ => _.FindPropertyMappingFor(It.IsAny<IEntity>(), It.IsAny<Iri>(), It.IsAny<Iri>()))
                .Returns<IEntity, Iri, Iri>((entity, iri, graph) =>
                {
                    var propertyInfo = (
                        from property in typeof(IEvent).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        from propertyMapping in property.GetCustomAttributes<PropertyAttribute>()
                        where propertyMapping.Iri == iri
                        select property).First();
                    var result = new Mock<IPropertyMapping>(MockBehavior.Strict);
                    result.SetupGet(_ => _.PropertyInfo).Returns(propertyInfo);
                    return result.Object;
                });
            var context = new Mock<IEntityContext>(MockBehavior.Strict);
            context.SetupGet(_ => _.Mappings).Returns(mappingsRepository.Object);
            Body = new MulticastObject().ActLike<IEvent>();
            Body.Description = "Some description";
            Body.Unwrap().SetProperty(typeof(IEntity).GetProperty(nameof(IEntity.Context)), context.Object);
            Parameters = new MulticastObject().ActLike<IEvent>();
            Parameters.Name = "the-name";
            Parameters.Unwrap().SetProperty(typeof(IEntity).GetProperty(nameof(IEntity.Context)), context.Object);
            ScenarioSetup();
            TheTest();
        }

        public virtual void ScenarioSetup()
        {
        }

        private IIriTemplateMapping MappingOf(Iri property, string variable)
        {
            var propertyResource = new Mock<IResource>(MockBehavior.Strict);
            propertyResource.SetupGet(_ => _.Iri).Returns(property);
            var result = new Mock<IIriTemplateMapping>(MockBehavior.Strict);
            result.SetupGet(_ => _.Property).Returns(propertyResource.Object);
            result.SetupGet(_ => _.Variable).Returns(variable);
            return result.Object;
        }
    }
}
