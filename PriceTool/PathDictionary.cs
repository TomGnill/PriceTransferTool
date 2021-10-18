using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PriceTool
{
    public class PathDictionary
    {
        public string pathToNewPrices { get; set; }= string.Empty;
        public string pathToPriceList { get; set; }= string.Empty;
        public string pathToNewPriceList { get; set; } = string.Empty;

        [JsonIgnore]
        private string _saveDirPath = $@"C:\Users\{Environment.UserName}\AppData\Local\PriceToolCache\";
        [JsonIgnore]
        private string _savePath = $@"C:\Users\{Environment.UserName}\AppData\Local\PriceToolCache\PathSettings.json";
        public PathDictionary()
        { }

        public void Save()
        {
            CheckDir();
            if (File.Exists(_savePath))
            {
                File.Delete(_savePath);
            }
            Console.WriteLine(JsonSerializer.Serialize(this));
            File.WriteAllText(_savePath, JsonSerializer.Serialize<PathDictionary>(this), Encoding.UTF8);
        }

        public void CheckDir()
        {
            if (!Directory.Exists(_saveDirPath))
            {
                Directory.CreateDirectory(_saveDirPath);
            }
        }
        public void Load()
        {
            CheckDir();
            if (File.Exists(_savePath))
            {
                PathDictionary pd = JsonSerializer.Deserialize<PathDictionary>(File.ReadAllText(_savePath, Encoding.UTF8));
                this.pathToPriceList = pd.pathToPriceList;
                this.pathToNewPrices = pd.pathToNewPrices;
                this.pathToNewPriceList = pd.pathToNewPriceList;
            }
        }
    }
}
