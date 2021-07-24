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
            Console.WriteLine(Path.GetDirectoryName(@"C:\Users\andri\Desktop\excelBack\price_stm.xls"));
        }
    }
}
