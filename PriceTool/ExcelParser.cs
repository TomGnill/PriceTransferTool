using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClosedXML.Excel;
using System.Linq;

namespace PriceTool
{
    public class ExcelParser
    {
        public IXLWorkbook Workbook { get; set; }
        private IXLWorksheet MainSheet { get; set; }
        private int _vendorCodeColumnNubmer;
        private int _nameColumnNubmer;
        private int _priceColumnNubmer;
        private int _priceStartRow;

        public ExcelParser(string path)
        {
            Workbook = new XLWorkbook(path);
            MainSheet = Workbook.Worksheets.Worksheet(1);
        }

        public void SaveExcel(string path)
        {
            Workbook.SaveAs(path);
        }

        public List<Product> ParsePriceList()
        {
            ParseColumns();
            List<Product> newPrices = new List<Product>();

            for (int row = _priceStartRow; row < MainSheet.RowCount(); row++)
            {
                if (!string
                    .IsNullOrEmpty(MainSheet.Cell(row, _priceColumnNubmer)
                        .Value.ToString()))
                {
                    string vendorCode = MainSheet.Cell(row, _vendorCodeColumnNubmer).Value.ToString();
                    string name = MainSheet.Cell(row, _nameColumnNubmer).Value.ToString();
                    double price = double.Parse(MainSheet.Cell(row, _priceColumnNubmer).Value.ToString());
                    newPrices.Add(new Product(vendorCode, name, price));
                }
            }

            return newPrices;
        }

        public ExcelParser TransferPrices(List<Product> newPrices)
        {
            ParseColumns();
            for (int row = _priceStartRow; row < MainSheet.RowCount(); row++)
            {
                if (!string
                    .IsNullOrEmpty(MainSheet.Cell(row, _priceColumnNubmer)
                        .Value.ToString()))
                {
                    string vendorCode = MainSheet.Cell(row, _vendorCodeColumnNubmer).Value.ToString();
                    if (ExtensionMethods.CheckVendorCode(vendorCode, newPrices))
                    {
                        Product product = newPrices.Find(prod => prod.VendorCode == vendorCode);
                        MainSheet.Cell(row, _nameColumnNubmer).Value = product.Name;
                        MainSheet.Cell(row, _priceColumnNubmer).Value = product.Price;
                    }
                }
            }

            return this;
        }



        public void ParseColumns()
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
                            _vendorCodeColumnNubmer = column;
                        }
                        if (MainSheet.Cell(row, column).Value.ToString()
                            == "Наименование")
                        {
                            _nameColumnNubmer = column;
                        }
                        if (MainSheet.Cell(row, column).Value.ToString()
                            == "РРЦ, руб. с НДС")
                        {
                            _priceColumnNubmer = column;
                        }

                        if (_priceColumnNubmer != 0
                            && _nameColumnNubmer != 0
                            && _vendorCodeColumnNubmer != 0)
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
