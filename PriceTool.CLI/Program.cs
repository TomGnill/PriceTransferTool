using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace PriceTool.CLI
{
   public class Program
    {
        static void Main(string[] args)
        {
            //ExcelParser firstExcel = new ExcelParser(args[0]);
            //ExcelParser secondExcel = new ExcelParser(args[1]);
            //secondExcel.TransferPrices(firstExcel.ParsePriceList());

            //Console.WriteLine(ExtensionMethods.ParseVendorCode("Настенный комплект 20-1/2 универсальный серый(221020) gfsfs аыа (5252)"));

            KeyDictionary keyDictionary = new KeyDictionary();
            keyDictionary.Load();
            keyDictionary.ClearSettings();
            keyDictionary.AddNameKey("NameOf");
            keyDictionary.AddNameKey("Слово на русском");
            Console.WriteLine(string.Join("...", keyDictionary.NameKey));
            keyDictionary.ExecuteSave();
            //Directory.CreateDirectory($@"C:\Users\{Environment.UserName}\AppData\Local\PriceToolCache\");
            //Console.WriteLine(JsonSerializer.Serialize<KeyDictionary>(keyDictionary));
        }
    }
}
