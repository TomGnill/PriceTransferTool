using System;
using System.Collections.Generic;
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

        public static string ParseVendorCode(string name)
        {
            return new Regex(@"\([^()]*\)$").Match(name).Value.Trim('(',')');
        }
    }
}
