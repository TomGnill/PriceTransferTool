using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PriceTool
{
    public static class ExtensionMethods
    {
        public static bool CheckVendorCode(string vendorCode, List<Product> products)
        {
            return products
                .Select(a => a.VendorCode)
                .ToList()
                .Contains(vendorCode);
        }
    }
}
