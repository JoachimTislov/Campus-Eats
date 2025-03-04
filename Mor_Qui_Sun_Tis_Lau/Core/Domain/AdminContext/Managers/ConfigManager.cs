namespace Mor_Qui_Sun_Tis_Lau.Core.Domain.AdminContext.Managers;

using System.IO;
using Newtonsoft.Json;

public class ConfigManager(string filePath)
{
    private readonly string _filePath = filePath;

    public Config GetConfig()
    {
        if (!File.Exists(_filePath))
        {
            var defaultConfig = new Config { DeliveryFee = 50.0m };
            UpdateConfig(defaultConfig);
            return defaultConfig;
        }

        string jsonString = File.ReadAllText(_filePath);
        return JsonConvert.DeserializeObject<Config>(jsonString) ?? new Config();
    }

    public void UpdateConfig(Config config)
    {
        string jsonString = JsonConvert.SerializeObject(config, Formatting.Indented);
        File.WriteAllText(_filePath, jsonString);
    }
}

public class Config
{
    [JsonProperty("delivery_fee")]
    public decimal DeliveryFee { get; set; }
}