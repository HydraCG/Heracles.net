using System.Diagnostics.CodeAnalysis;
using RDeF.Entities;

namespace Heracles.Namespaces
{
    /// <summary>Defines hydra namespace and terms.</summary>
    [SuppressMessage("UnitTests", "TS0000:NoUnitTests", Justification = "No logic is available to be tested.")]
    [ExcludeFromCodeCoverage]
    public static class hydra
    {
        /// <summary>Defines a hydra namespace IRI.</summary>
        public static readonly Iri Namespace = new Iri("http://www.w3.org/ns/hydra/core#");

        /// <summary>Defines a hydra:apiDocumentation predicate IRI.</summary>
        public static readonly Iri apiDocumentation = new Iri((string)Namespace + "apiDocumentation");

        /// <summary>Defines a hydra:description predicate IRI.</summary>
        public static readonly Iri description = new Iri((string)Namespace + "description");

        /// <summary>Defines a hydra:entrypoint predicate IRI.</summary>
        public static readonly Iri entrypoint = new Iri((string)Namespace + "entrypoint");

        /// <summary>Defines a hydra:supportedClass predicate IRI.</summary>
        public static readonly Iri supportedClass = new Iri((string)Namespace + "supportedClass");

        /// <summary>Defines a hydra:title predicate IRI.</summary>
        public static readonly Iri title = new Iri((string)Namespace + "title");
        
        /// <summary>Defines a hydra:ApiDocumentation predicate IRI.</summary>
        public static readonly Iri ApiDocumentation = new Iri((string)Namespace + "ApiDocumentation");
        
        /// <summary>Defines a hydra:mapping predicate IRI.</summary>
        public static readonly Iri mapping = new Iri((string)Namespace + "mapping");

        /// <summary>Defines a hydra:object predicate IRI.</summary>
        public static readonly Iri @object = new Iri((string)Namespace + "object");

        /// <summary>Defines a hydra:property predicate IRI.</summary>
        public static readonly Iri property = new Iri((string)Namespace + "property");

        /// <summary>Defines a hydra:subject predicate IRI.</summary>
        public static readonly Iri subject = new Iri((string)Namespace + "subject");

        /// <summary>Defines a hydra:template predicate IRI.</summary>
        public static readonly Iri template = new Iri((string)Namespace + "template");

        /// <summary>Defines a hydra:variable predicate IRI.</summary>
        public static readonly Iri variable = new Iri((string)Namespace + "variable");

        /// <summary>Defines a hydra:variableRepresentation predicate IRI.</summary>
        public static readonly Iri variableRepresentation = new Iri((string)Namespace + "variableRepresentation");
        
        /// <summary>Defines a hydra:BasicRepresentation predicate IRI.</summary>
        public static readonly Iri BasicRepresentation = new Iri((string)Namespace + "BasicRepresentation");

        /// <summary>Defines a hydra:IriTemplate predicate IRI.</summary>
        public static readonly Iri IriTemplate = new Iri((string)Namespace + "IriTemplate");

        /// <summary>Defines a hydra:IriTemplateMapping predicate IRI.</summary>
        public static readonly Iri IriTemplateMapping = new Iri((string)Namespace + "IriTemplateMapping");

        /// <summary>Defines a hydra:Link predicate IRI.</summary>
        public static readonly Iri Link = new Iri((string)Namespace + "Link");

        /// <summary>Defines a hydra:TemplatedLink predicate IRI.</summary>
        public static readonly Iri TemplatedLink = new Iri((string)Namespace + "TemplatedLink");

        /// <summary>Defines a hydra:VariableRepresentation predicate IRI.</summary>
        public static readonly Iri VariableRepresentation = new Iri((string)Namespace + "VariableRepresentation");
        
        /// <summary>Defines a hydra:collection predicate IRI.</summary>
        public static readonly Iri collection = new Iri((string)Namespace + "collection");

        /// <summary>Defines a hydra:manages predicate IRI.</summary>
        public static readonly Iri manages = new Iri((string)Namespace + "manages");

        /// <summary>Defines a hydra:member predicate IRI.</summary>
        public static readonly Iri member = new Iri((string)Namespace + "member");

        /// <summary>Defines a hydra:pageIndex predicate IRI.</summary>
        public static readonly Iri pageIndex = new Iri((string)Namespace + "pageIndex");

        /// <summary>Defines a hydra:pageReference predicate IRI.</summary>
        public static readonly Iri pageReference = new Iri((string)Namespace + "pageReference");

        /// <summary>Defines a hydra:totalItems predicate IRI.</summary>
        public static readonly Iri totalItems = new Iri((string)Namespace + "totalItems");
        
        /// <summary>Defines a hydra:Collection predicate IRI.</summary>
        public static readonly Iri Collection = new Iri((string)Namespace + "Collection");

        /// <summary>Defines a hydra:PartialCollectionView predicate IRI.</summary>
        public static readonly Iri PartialCollectionView = new Iri((string)Namespace + "PartialCollectionView");
        
        /// <summary>Defines a hydra:readable predicate IRI.</summary>
        public static readonly Iri readable = new Iri((string)Namespace + "readable");

        /// <summary>Defines a hydra:required predicate IRI.</summary>
        public static readonly Iri required = new Iri((string)Namespace + "required");

        /// <summary>Defines a hydra:supportedOperation predicate IRI.</summary>
        public static readonly Iri supportedOperation = new Iri((string)Namespace + "supportedOperation");

        /// <summary>Defines a hydra:supportedProperty predicate IRI.</summary>
        public static readonly Iri supportedProperty = new Iri((string)Namespace + "supportedProperty");

        /// <summary>Defines a hydra:writeable predicate IRI.</summary>
        public static readonly Iri writeable = new Iri((string)Namespace + "writeable");
        
        /// <summary>Defines a hydra:Class predicate IRI.</summary>
        public static readonly Iri Class = new Iri((string)Namespace + "Class");

        /// <summary>Defines a hydra:SupportedProperty predicate IRI.</summary>
        public static readonly Iri SupportedProperty = new Iri((string)Namespace + "SupportedProperty");
        
        /// <summary>Defines a hydra:expects predicate IRI.</summary>
        public static readonly Iri expects = new Iri((string)Namespace + "expects");

        /// <summary>Defines a hydra:expectsHeader predicate IRI.</summary>
        public static readonly Iri expectsHeader = new Iri((string)Namespace + "expectsHeader");

        /// <summary>Defines a hydra:method predicate IRI.</summary>
        public static readonly Iri method = new Iri((string)Namespace + "method");

        /// <summary>Defines a hydra:possibleStatus predicate IRI.</summary>
        public static readonly Iri possibleStatus = new Iri((string)Namespace + "possibleStatus");

        /// <summary>Defines a hydra:returns predicate IRI.</summary>
        public static readonly Iri returns = new Iri((string)Namespace + "returns");

        /// <summary>Defines a hydra:returnsHeader predicate IRI.</summary>
        public static readonly Iri returnsHeader = new Iri((string)Namespace + "returnsHeader");
        
        /// <summary>Defines a hydra:Operation predicate IRI.</summary>
        public static readonly Iri Operation = new Iri((string)Namespace + "Operation");

        /// <summary>Defines a hydra:Status predicate IRI.</summary>
        public static readonly Iri Status = new Iri((string)Namespace + "Status");
        
        /// <summary>Defines a hydra:operation predicate IRI.</summary>
        public static readonly Iri operation = new Iri((string)Namespace + "operation");
        
        /// <summary>Defines a hydra:Resource predicate IRI.</summary>
        public static readonly Iri Resource = new Iri((string)Namespace + "Resource");
        
        /// <summary>Defines a hydra:first predicate IRI.</summary>
        public static readonly Iri first = new Iri((string)Namespace + "first");

        /// <summary>Defines a hydra:freetextQuery predicate IRI.</summary>
        public static readonly Iri freetextQuery = new Iri((string)Namespace + "freetextQuery");

        /// <summary>Defines a hydra:last predicate IRI.</summary>
        public static readonly Iri last = new Iri((string)Namespace + "last");

        /// <summary>Defines a hydra:next predicate IRI.</summary>
        public static readonly Iri next = new Iri((string)Namespace + "next");

        /// <summary>Defines a hydra:previous predicate IRI.</summary>
        public static readonly Iri previous = new Iri((string)Namespace + "previous");

        /// <summary>Defines a hydra:search predicate IRI.</summary>
        public static readonly Iri search = new Iri((string)Namespace + "search");

        /// <summary>Defines a hydra:hydraNamespace predicate IRI.</summary>
        public static readonly Iri view = new Iri((string)Namespace + "view");
    }
}
