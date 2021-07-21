using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using FileDialog = System.Windows.Forms.FileDialog;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;

namespace PriceTool.GUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("ru-RU");
            InitializeComponent();
            pathToNewPrices.Text = @"C:\example.xlsx";
            pathToPriceList.Text = @"C:\example.xlsx";
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
                        ExcelParser secondExcel = new ExcelParser(pathToPriceList.Text);
                        secondExcel
                            .TransferPrices(firstParser.MultiParseList(pathToNewPriceList.Text)).Workbook.Save();
                        MessageBox.Show("Готово");
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
                    ExcelParser firstExcel = new ExcelParser(pathToNewPrices.Text);
                    ExcelParser secondExcel = new ExcelParser(pathToPriceList.Text);
                    var wb = secondExcel
                        .TransferPrices(firstExcel.ParsePriceList()).Workbook;
                    wb.Save();
                    MessageBox.Show("Готово");
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
    }
}
