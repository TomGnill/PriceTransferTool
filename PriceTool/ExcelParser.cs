using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClosedXML.Excel;
using System.Linq;
using DocumentFormat.OpenXml.Wordprocessing;

namespace PriceTool
{
    public class ExcelParser
    {
        public IXLWorkbook Workbook { get; set; }
        private IXLWorksheet MainSheet { get; set; }
        public List<Product> NotFoundProducts;
        public bool IsChanged = false;
        private int _vendorCodeColumnNumber;
        private int _nameColumnNumber;
        private int _priceColumnNumber;
        private int _priceStartRow;
        private int _newPricesColumn;

        public ExcelParser(string path)
        {
            Workbook = new XLWorkbook(path);
            MainSheet = Workbook.Worksheets.Worksheet(1);
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
                allNewPrices.AddRange(new ExcelParser(strings).ParsePriceList());
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
                        .Value.ToString()))
                {
                    string vendorCode = ExtensionMethods.ParseVendorCode(MainSheet.Cell(row, _nameColumnNumber).Value.ToString());
                    if (ExtensionMethods.CheckVendorCode(vendorCode, newPrices))
                    {
                        IsChanged = true;
                        Product product = newPrices.Find(prod => prod.VendorCode == vendorCode);
                        MainSheet.Cell(row, _newPricesColumn).Value = product.Price;
                        newPrices.Remove(product);
                    }
                }
            }

            NotFoundProducts = newPrices;
            return this;
        }

        public IXLWorkbook SaveNotFoundProducts(List<Product> notFoundProducts, string path)
        {
            File.Delete(path);
            IXLWorkbook newWorkbook = new XLWorkbook(path); 
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
            Workbook.Save();
            return newWorkbook;
        }

        public void ParseColumnsNewPriceList()
        {
            int column = 1;
            for (int row = 1; row < MainSheet.RowCount(); row++)
            {
                if ((MainSheet.Cell(row, column).Value.ToString() == "Наименование") ||
                    (MainSheet.Cell(row, column).Value.ToString() == "Артикул"))
                {
                    for (column = 1; column < MainSheet.ColumnCount(); column++)
                    {
                        if (MainSheet.Cell(row, column).Value.ToString()
                            == "Артикул")
                        {
                            _vendorCodeColumnNumber = column;
                        }
                        if (MainSheet.Cell(row, column).Value.ToString()
                            == "Наименование")
                        {
                            _nameColumnNumber = column;
                        }
                        if (MainSheet.Cell(row, column).Value.ToString()
                            == "РРЦ, руб. с НДС" || MainSheet.Cell(row, column).Value.ToString().ToLower()
                            == "Цена".ToLower())
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
            int column = 1;
            for (int row = 1; row < MainSheet.RowCount(); row++)
            {
                if ((MainSheet.Cell(row, column).Value.ToString() == "Наименование товара") ||
                    (MainSheet.Cell(row, column).Value.ToString() == "Код"))
                {
                    for (column = 1; column < MainSheet.ColumnCount(); column++)
                    {
                        if (MainSheet.Cell(row, column).Value.ToString()
                            == "Наименование товара")
                        {
                            _nameColumnNumber = column;
                        }
                        if (MainSheet.Cell(row, column).Value.ToString()
                            == "Цена Розница, руб." || MainSheet.Cell(row, column).Value.ToString().ToLower()
                            == "Цена".ToLower())
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
