using System;

namespace PriceTool.CLI
{
   public class Program
    {
        static void Main(string[] args)
        {
            ExcelParser firstExcel = new ExcelParser(args[0]);
            ExcelParser secondExcel = new ExcelParser(args[1]);
            secondExcel.TransferPrices(firstExcel.ParsePriceList()).SaveExcel(args[1]);
        }
    }
}
