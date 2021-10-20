using System.Windows;

namespace PriceTool.GUI
{
    public partial class KeySettingsWindow : Window
    {
        public KeyDictionary KeyDictionary;
        public KeySettingsWindow()
        {
            InitializeComponent();
            KeyDictionary = new KeyDictionary();
            KeyDictionary.Load();
            listNameKeys.ItemsSource = KeyDictionary.NameKey;
            listPriceKeys.ItemsSource = KeyDictionary.PriceKey;
            listVendorCodes.ItemsSource = KeyDictionary.VendorCodeKey;
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            KeyDictionary.ExecuteSave();
            this.Close();
        }

        private void Clear_Button_Click(object sender, RoutedEventArgs e)
        {
            KeyDictionary.ClearSettings();
            this.Close();
        }

        private void Add_Name_Button_Click(object sender, RoutedEventArgs e)
        {
            KeyDictionary.NameKey.Add(KeyName.Text);
            listNameKeys.ItemsSource = KeyDictionary.NameKey;
            KeyName.Text = string.Empty;
            KeyDictionary.ExecuteSave();
        }
        private void Add_Vendor_Button_Click(object sender, RoutedEventArgs e)
        {
            KeyDictionary.AddVendorCodeKeys(KeyVendor.Text);
            KeyVendor.Text = string.Empty;
            KeyDictionary.ExecuteSave();
        }
        private void Add_Price_Button_Click(object sender, RoutedEventArgs e)
        {
            KeyDictionary.AddPriceKeys(KeyPrice.Text);
            KeyPrice.Text = string.Empty;
            KeyDictionary.ExecuteSave();
        }
    }
}
