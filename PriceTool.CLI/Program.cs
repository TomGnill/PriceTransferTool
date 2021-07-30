using System;
using System.IO;

namespace PriceTool.CLI
{
   public class Program
    {
        static void Main(string[] args)
        {
            //ExcelParser firstExcel = new ExcelParser(args[0]);
            //ExcelParser secondExcel = new ExcelParser(args[1]);
            //secondExcel.TransferPrices(firstExcel.ParsePriceList());

            Console.WriteLine(ExtensionMethods.ParseVendorCode("Настенный комплект 20-1/2 универсальный серый(221020) gfsfs аыа (5252)"));
        }
    }
}
