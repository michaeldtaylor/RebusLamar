using System;

namespace Overlord.Domain
{
    public class ProductVariantGenerationException : Exception
    {
        public ProductVariantGenerationException(string message = "Product variant generation failed")
            : base(message)
        {
        }

        public ProductVariantGenerationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
