
using System.Windows;
using Microsoft.Win32;
using FileDialog = System.Windows.Forms.FileDialog;
using MessageBox = System.Windows.MessageBox;
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
            InitializeComponent();
        }

        private void Transfer_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(pathToNewPrices.Text) && !string.IsNullOrEmpty(pathToPriceList.Text))
            {
                ExcelParser firstExcel = new ExcelParser(pathToNewPrices.Text);
                ExcelParser secondExcel = new ExcelParser(pathToPriceList.Text);
                secondExcel.TransferPrices(firstExcel.ParsePriceList());
            }
            else
            {
                MessageBox.Show("Укажите путь");
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
                Filter = @"Excel files (*.xlsx)|*.xlsx",
                FilterIndex = 1,
                RestoreDirectory = true
            };
            dlg.ShowDialog();
            pathToNewPrices.Text = dlg.FileName;
        }

        private void Select_pathToPriceList_Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = @"Excel files (*.xlsx)|*.xlsx",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pathToPriceList.Text = saveFileDialog.FileName;
            }
        }
    }
}
