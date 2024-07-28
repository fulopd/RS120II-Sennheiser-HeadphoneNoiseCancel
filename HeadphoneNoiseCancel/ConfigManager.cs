using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeadphoneNoiseCancel
{
    internal class ConfigManager
    {

        private string filePath;
        private Dictionary<string, int> configValues;

        public ConfigManager(string filePath)
        {
            this.filePath = filePath;
            this.configValues = new Dictionary<string, int>();

            if (File.Exists(filePath))
            {
                LoadConfig();
            }
        }

        private void LoadConfig()
        {
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var parts = line.Split('=');
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    if (int.TryParse(parts[1].Trim(), out int value))
                    {
                        configValues[key] = value;
                    }
                    else
                    {
                        Console.WriteLine($"Hibás érték a konfigurációs fájlban: {line}");
                    }
                }
            }
        }

        public int? GetValue(string key)
        {
            return configValues.ContainsKey(key) ? configValues[key] : (int?)null;
        }

        public void SetValue(string key, int value)
        {
            configValues[key] = value;
            SaveConfig();
        }

        private void SaveConfig()
        {
            var lines = new List<string>();
            foreach (var kvp in configValues)
            {
                lines.Add($"{kvp.Key}={kvp.Value}");
            }
            File.WriteAllLines(filePath, lines);
        }
    }
}