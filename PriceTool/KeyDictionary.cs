using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PriceTool
{
    public class KeyDictionary
    {
        public List<string> PriceKey { get; set; }
        public List<string> NameKey { get; set; }
        public List<string> VendorCodeKey { get; set; }
        
        [JsonIgnore]
        private string _saveDirPath = $@"C:\Users\{Environment.UserName}\AppData\Local\PriceToolCache\";
        [JsonIgnore]
        private string _savePath = $@"C:\Users\{Environment.UserName}\AppData\Local\PriceToolCache\KeySettings.json";

        public KeyDictionary()
        {
            PriceKey = new List<string>();
            NameKey = new List<string>();
            VendorCodeKey = new List<string>();
        }

        public void AddNameKey(string key)
        {
            NameKey.Add(key);
        }
        public void AddPriceKeys(string key)
        {
            PriceKey.Add(key);
        }
        public void AddVendorCodeKeys(string key)
        {
            VendorCodeKey.Add(key);
        }

        
        public void ExecuteSave()
        {
            CheckDir();
            ClearSettings();
            File.WriteAllText(_savePath, JsonSerializer.Serialize<KeyDictionary>(this), Encoding.UTF8);
            //await JsonSerializer.SerializeAsync<KeyDictionary>(fs, this);
        }

        public  void ClearSettings()
        {
            PriceKey = new List<string>();
            NameKey = new List<string>();
            VendorCodeKey = new List<string>();
            if (File.Exists(_savePath))
            {
                File.Delete(_savePath);
            }
        }

        public void CheckDir()
        {
            if (!Directory.Exists(_saveDirPath))
            {
                Directory.CreateDirectory(_saveDirPath);
            }
        }

        public bool Load()
        {
            if (!File.Exists(_savePath)) return false;
            KeyDictionary restoredSettings = JsonSerializer.Deserialize<KeyDictionary>(File.ReadAllText(_savePath, Encoding.UTF8));
            this.NameKey = restoredSettings.NameKey;
            this.PriceKey = restoredSettings.PriceKey;
            this.VendorCodeKey = restoredSettings.VendorCodeKey;
            return true;
        }
    }
}
