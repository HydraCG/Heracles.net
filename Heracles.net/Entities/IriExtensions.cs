using System;
using RDeF.Entities;

namespace Heracles.Entities
{
    internal static class IriExtensions
    {
        internal static Iri ToRoot(this Iri iri)
        {
            Iri result = null;
            if (iri != null && !iri.IsBlank)
            {
                Uri uri = iri;
                if (uri != null && uri.IsAbsoluteUri)
                {
                    result = new Iri(uri.GetLeftPart(UriPartial.Authority));
                }
            }

            return result;
        }

        internal static bool IsHttp(this Iri iri)
        {
            return iri != null && iri.ToString().StartsWith("http", StringComparison.OrdinalIgnoreCase);
        }
    }
}
