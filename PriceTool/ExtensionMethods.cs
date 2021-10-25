using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

        public static Product CheckVendorCodeByName(string productName, List<Product> products)
        {
            foreach (var product in products)
            {
                if (productName.Contains(product.VendorCode))
                {
                    return product;
                }
            }

            return null;
        }

        public static string ParseVendorCode(string name)
        {
            return new Regex(@"\([^()]*\)").Match(name).Value.Trim('(',')');
        }
    }
}
