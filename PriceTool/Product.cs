using System;
using System.Collections.Generic;
using System.Text;

namespace PriceTool
{
   public class Product
   {
       public string VendorCode { get; set; }
       public string Name { get; set; }
       public double Price { get; set; }

       public Product(string vendorCode, string name, double price)
       {
           VendorCode = vendorCode;
           Name = name;
           Price = price;
       }

       public Product( string name, int price)
       {
           VendorCode = ParseVendorCode(name);
           Name = name;
           Price = price;
       }

        private string ParseVendorCode(string Name)
       {
           return string.Empty;
       }
   }
}
