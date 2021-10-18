using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Text.Json;
using FileDialog = System.Windows.Forms.FileDialog;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;

namespace PriceTool.GUI
{
    public partial class MainWindow : System.Windows.Window
    {
        private KeyDictionary _keyDictionary;
        public MainWindow()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("ru-RU");
            InitializeComponent();
            _keyDictionary = new KeyDictionary();
            _keyDictionary.Load();
            pathToNewPrices.Text = @"C:\example.xlsx";
            pathToPriceList.Text = @"C:\example.xlsx";
            TryLoad();
        }

        private void Transfer_Button_Click(object sender, RoutedEventArgs e)
        {
            
            if (string.IsNullOrEmpty(pathToNewPrices.Text) && !string.IsNullOrEmpty(pathToNewPriceList.Text))
            {
                if (!string.IsNullOrEmpty(pathToNewPriceList.Text) && Directory.GetFiles(pathToNewPriceList.Text).Where(a => a.EndsWith(".xlsx"))
                    .ToList().Count == 0)
                {
                    MessageBox.Show("Нет файлов xlsx");
                }
                else
                {
                    if ( !string.IsNullOrEmpty(pathToPriceList.Text) && pathToPriceList.Text.EndsWith(".xlsx")
                                                                     && pathToNewPrices.Text != @"C:\example.xlsx")
                    {
                        ExcelParser firstParser = new ExcelParser();
                        ExcelParser secondExcel = new ExcelParser(pathToPriceList.Text, _keyDictionary);
                        secondExcel
                            .TransferPrices(firstParser.MultiParseList(pathToNewPriceList.Text));
                        if (secondExcel.IsChanged)
                        {
                            secondExcel.Workbook.Save();
                            CreateNotFoundProducts(PathToNotFoundProducts(), secondExcel);
                        }
                        else
                        {
                            MessageBox.Show("Без изменений");
                        }
                        MessageBox.Show("Готово");
                        if (secondExcel.NotFoundProducts.Count > 0)
                        {
                            MessageBox.Show(NotFoundProductsMessageBuilder(PathToNotFoundProducts(),
                                secondExcel.NotFoundProducts.Count));
                        }
                    }
                    else
                    {
                        MessageBox.Show("Укажите корректный путь");
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(pathToNewPrices.Text) && !string.IsNullOrEmpty(pathToPriceList.Text)
                                                                && pathToNewPrices.Text.EndsWith(".xlsx")&&
                                                                pathToPriceList.Text.EndsWith(".xlsx")
                                                                 && pathToNewPrices.Text != @"C:\example.xlsx")
                {
                    ExcelParser firstExcel = new ExcelParser(pathToNewPrices.Text, _keyDictionary);
                    ExcelParser secondExcel = new ExcelParser(pathToPriceList.Text, _keyDictionary);
                    secondExcel
                        .TransferPrices(firstExcel.ParsePriceList());
                    if (secondExcel.IsChanged)
                    {
                        secondExcel.Workbook.Save();
                        CreateNotFoundProducts(PathToNotFoundProducts(), secondExcel);
                    }
                    else
                    {
                        MessageBox.Show("Без изменений");
                    }
                    MessageBox.Show("Готово");
                    if (secondExcel.NotFoundProducts.Count > 0)
                    {
                        MessageBox.Show(NotFoundProductsMessageBuilder(PathToNotFoundProducts(),
                            secondExcel.NotFoundProducts.Count));
                    }
                }
                else
                {
                    MessageBox.Show("Укажите путь");
                }
            }
        }
        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private string PathToNotFoundProducts()
        {
            if(!string.IsNullOrEmpty(pathToNewPriceList.Text))
            {
                return pathToNewPriceList.Text + $"{Path.DirectorySeparatorChar}NotFoundProducts.xlsx";
            }
            if (!string.IsNullOrEmpty(pathToNewPrices.Text))
            {
                return Path.GetDirectoryName(pathToNewPrices.Text) + $"{Path.DirectorySeparatorChar}NotFoundProducts.xlsx";
            }
            else
            {
                return Path.GetTempPath() + $"{Path.DirectorySeparatorChar}NotFoundProducts.xlsx";
            }
        }

        private void CreateNotFoundProducts(string path, ExcelParser parser)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            parser.SaveNotFoundProducts(parser.NotFoundProducts, path)
                .SaveAs(path);
        }

        private string NotFoundProductsMessageBuilder(string path, int count)
        {
            return $"No products found : {count} \n " +
                   $"detailed : ${path}";
        }

        private void Select_pathToNewPrices_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                Filter = @" Excel files (*.xlsx)|*.xlsx",
                FilterIndex = 2,
                RestoreDirectory = true
            };
            dlg.ShowDialog();
            pathToNewPrices.Text = dlg.FileName;
            pathToNewPriceList.Text = string.Empty;
        }
        
        private void Select_pathToNewPriceList_Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pathToNewPriceList.Text = dlg.SelectedPath;
                pathToNewPrices.Text = string.Empty;
            }
        }

        private void Select_pathToPriceList_Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = @" Excel files (*.xlsx)|*.xlsx",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pathToPriceList.Text = saveFileDialog.FileName;
            }
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            PathDictionary pd = new PathDictionary
            {
                pathToNewPrices = pathToNewPrices.Text, pathToPriceList = pathToPriceList.Text, pathToNewPriceList = pathToNewPriceList.Text
            };
            pd.Save();
        }

        private void KeySettings_Button_Click(object sender, RoutedEventArgs e)
        {
            KeySettingsWindow newWindow = new KeySettingsWindow();
            newWindow.Show();
            newWindow.Closed += TransferSettings;
        }

        private void TransferSettings(object sender, EventArgs e)
        {
            _keyDictionary.Load();
        }

        private void TryLoad()
        {
            PathDictionary pd = new PathDictionary();
            pd.Load();
            if (!string.IsNullOrEmpty(pd.pathToPriceList))
            {
                pathToPriceList.Text = pd.pathToPriceList;
            }
            if (!string.IsNullOrEmpty(pd.pathToNewPriceList))
            {
                pathToNewPriceList.Text = pd.pathToNewPriceList;
            }
            else if (!string.IsNullOrEmpty(pd.pathToNewPrices))
            {
                pathToNewPrices.Text = pd.pathToNewPrices;
            }
        }
    }
}
