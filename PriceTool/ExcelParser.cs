using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using System.Linq;

namespace PriceTool
{
    public class ExcelParser
    {
        private KeyDictionary _keyDictionary;
        public IXLWorkbook Workbook { get; set; }
        private IXLWorksheet MainSheet { get; set; }
        private List<string> CodeWords { get; set; }
        public List<Product> NotFoundProducts;
        public bool IsChanged = false;
        private int _vendorCodeColumnNumber;
        private int _nameColumnNumber;
        private int _priceColumnNumber;
        private int _priceStartRow;
        private int _newPricesColumn;

        public ExcelParser(string path, KeyDictionary keyDictionary)
        {
            _keyDictionary = keyDictionary;
            Workbook = new XLWorkbook(path);
            MainSheet = Workbook.Worksheets.Worksheet(1);
            CodeWords = new List<string>();
            CodeWords.AddRange(_keyDictionary.NameKey);
            CodeWords.AddRange(_keyDictionary.VendorCodeKey);
            CodeWords.AddRange(_keyDictionary.PriceKey);
        }

        public ExcelParser() { }

        public List<Product> ParsePriceList()
        {
            ParseColumnsNewPriceList();
            List<Product> newPrices = new List<Product>();
            for (int row = _priceStartRow; row < MainSheet.RowCount(); row++)
            {
                if (!string
                    .IsNullOrEmpty(MainSheet.Cell(row, _priceColumnNumber)
                        .Value.ToString()) && double.TryParse(MainSheet.Cell(row, _priceColumnNumber)
                    .Value.ToString(), out var price))
                {
                    string vendorCode = MainSheet.Cell(row, _vendorCodeColumnNumber).Value.ToString();
                    string name = MainSheet.Cell(row, _nameColumnNumber).Value.ToString();
                    newPrices.Add(new Product(vendorCode, name, price));
                }
            }
            return newPrices;
        }

        public List<Product> MultiParseList(string path)
        {
            var files = Directory.GetFiles(path);
            List<string> xlsxs = files.Where(a => a.EndsWith(".xlsx")).ToList();
            List<Product> allNewPrices = new List<Product>();
            foreach (var strings in xlsxs)
            {
                allNewPrices.AddRange(new ExcelParser(strings, _keyDictionary).ParsePriceList());
            }
            return allNewPrices;
        }

        public ExcelParser TransferPrices(List<Product> newPrices)
        {
            ParseColumnPriceList();
            for (int row = _priceStartRow; row < MainSheet.RowCount(); row++)
            {
                if (!string
                    .IsNullOrEmpty(MainSheet.Cell(row, _priceColumnNumber)
                        .Value.ToString())
                    && MainSheet.Cell(row, _nameColumnNumber).Value.ToString().Contains("("))
                {
                    string productName = MainSheet.Cell(row, _nameColumnNumber).Value.ToString();
                    while (productName.Contains("(") && productName.Contains(")"))
                    {
                        string vendorCode =
                            ExtensionMethods.ParseVendorCode(productName);
                        if (ExtensionMethods.CheckVendorCode(vendorCode, newPrices))
                        {
                            IsChanged = true;
                            Product product = newPrices.Find(prod => prod.VendorCode == vendorCode);
                            MainSheet.Cell(row, _newPricesColumn).Value = product.Price;
                            newPrices.Remove(product);
                            break;
                        }
                        if(vendorCode != "")
                        {
                           productName = productName
                               .Replace(vendorCode, string.Empty)
                               .Replace("()", string.Empty);
                        }
                    }
                }
            }

            NotFoundProducts = newPrices;
            return this;
        }

        public ExcelParser TransferPricesInvert(List<Product> newPrices)
        {
            ParseColumnPriceList();
            for (int row = _priceStartRow; row < MainSheet.RowCount(); row++)
            {
                if (!string
                        .IsNullOrEmpty(MainSheet.Cell(row, _priceColumnNumber)
                        .Value.ToString()))
                {
                    string productName = MainSheet.Cell(row, _nameColumnNumber).Value.ToString();
                    Product productPrice = ExtensionMethods.CheckVendorCodeByName(productName, newPrices);
                    if (productPrice != null)
                    {
                        IsChanged = true;
                        MainSheet.Cell(row, _newPricesColumn).Value = productPrice.Price;
                        newPrices.Remove(productPrice);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            NotFoundProducts = newPrices;
            return this;
        }

        public IXLWorkbook SaveNotFoundProducts(List<Product> notFoundProducts, string path)
        {
            IXLWorkbook newWorkbook = new XLWorkbook(); 
            newWorkbook.AddWorksheet();
            IXLWorksheet newWorksheet = newWorkbook.Worksheet(1);
            int row = 2;

            newWorksheet.Cell(1, 1).Value = "Артикул";
            newWorksheet.Cell(1, 2).Value = "Наименование";
            newWorksheet.Cell(1, 3).Value = "Цена";
            foreach (var product in notFoundProducts)
            {
                newWorksheet.Cell(row, 1).Value = product.VendorCode;
                newWorksheet.Cell(row, 2).Value = product.Name;
                newWorksheet.Cell(row, 3).Value = product.Price;
                row++;
            }
            return newWorkbook;
        }

        public void ParseColumnsNewPriceList()
        {
            int column = 1;
            for (int row = 1; row < MainSheet.RowCount(); row++)
            {
                if (CodeWords.Contains(MainSheet.Cell(row, column).Value.ToString())
                    || CodeWords.Contains(MainSheet.Cell(row, column + 1).Value.ToString()))
                {
                    for (column = 1; column < MainSheet.ColumnCount(); column++)
                    {
                        string cellValue = MainSheet.Cell(row, column).Value.ToString();
                        if (_keyDictionary.VendorCodeKey.Contains(cellValue))
                        {
                            _vendorCodeColumnNumber = column;
                        }
                        if (_keyDictionary.NameKey.Contains(cellValue))
                        {
                            _nameColumnNumber = column;
                        }
                        if (_keyDictionary.PriceKey.Contains(cellValue))
                        {
                            _priceColumnNumber = column;
                        }

                        if (_priceColumnNumber != 0
                            && _nameColumnNumber != 0
                            && _vendorCodeColumnNumber != 0)
                        {
                            break;
                        }
                    }

                    _priceStartRow = row + 1;
                    break;
                }
            }
        }

        public void ParseColumnPriceList()
        {
            CodeWords.AddRange(new List<string>() {"Код", "Цена Розница, руб." });
            _keyDictionary.PriceKey.Add("Цена Розница, руб.");
            int column = 1;
            for (int row = 1; row < MainSheet.RowCount(); row++)
            {
                if (CodeWords.Contains(MainSheet.Cell(row, column).Value.ToString()))
                {
                    for (column = 1; column < MainSheet.ColumnCount(); column++)
                    {
                        string cellValue = MainSheet.Cell(row, column).Value.ToString();
                        if (_keyDictionary.NameKey.Contains(cellValue))
                        {
                            _nameColumnNumber = column;
                        }
                        if (_keyDictionary.PriceKey.Contains(cellValue))
                        {
                            _priceColumnNumber = column;
                            _newPricesColumn = _priceColumnNumber + 1;
                            MainSheet.Cell(row, _newPricesColumn).Value = "Цена источник";
                        }
                        if (_priceColumnNumber != 0
                            && _nameColumnNumber != 0)
                        {
                            break;
                        }
                    }
                    _priceStartRow = row + 1;
                    break;
                }
            }
        }
    }
}
