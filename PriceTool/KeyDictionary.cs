using System;
using System.Collections.Generic;
using System.IO;
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
            if (!Directory.Exists(_saveDirPath))
            {
                Directory.CreateDirectory(_saveDirPath);
            }
            if (!Load())
            {
                PriceKey = new List<string>();
                NameKey = new List<string>();
                VendorCodeKey = new List<string>();
            }
        }

        public void AddNameKeys(string keys)
        {
            string[] strings = keys.Split(',');
            NameKey.AddRange(strings);
        }
        public void AddPriceKeys(string keys)
        {
            string[] strings = keys.Split(',');
            PriceKey.AddRange(strings);
        }
        public void AddVendorCodeKeys(string keys)
        {
            string[] strings = keys.Split(',');
            VendorCodeKey.AddRange(strings);
        }

        public void Save()
        {
            Task.Run(() => (ExecuteSave()));
        }
        public async Task ExecuteSave()
        {
            using (FileStream fs = new FileStream(_savePath, FileMode.OpenOrCreate))
            {
                await JsonSerializer.SerializeAsync<KeyDictionary>(fs, this);
            }
        }

        public  void ClearSettings()
        {
            if (File.Exists(_savePath))
            {
                File.Delete(_savePath);
            }
        }

        private bool Load()
        {
          return Task.Run(TryLoad).Result;
        }
        private async Task<bool> TryLoad()
        {
            if (File.Exists(_savePath))
            {
                using (FileStream fs = new FileStream(_savePath, FileMode.OpenOrCreate))
                {
                    if (File.Exists(_savePath))
                    {
                        KeyDictionary restoredSettings = await JsonSerializer.DeserializeAsync<KeyDictionary>(fs);
                        this.NameKey = restoredSettings.NameKey;
                        this.PriceKey = restoredSettings.PriceKey;
                        this.VendorCodeKey = restoredSettings.VendorCodeKey;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
