using System.Collections.Generic;
using JsonLD.Core;
using RDeF.Entities;
using RDeF.Serialization;

namespace Heracles.JsonLd
{
    internal static class RDFDatasetExtensions
    {
        internal static IEnumerable<Statement> AsStatements(this RDFDataset rdfDataset)
        {
            foreach (var graph in rdfDataset.GraphNames())
            {
                foreach (var quad in rdfDataset.GetQuads(graph))
                {
                    yield return quad.AsStatement();
                }
            }
        }
    }
}
