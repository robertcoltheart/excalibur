using System.Text.Json;

namespace Excalibur.Configuration;

public class ConfigurationReader
{
    private const string ConfigPath = "Excalibur";

    public ExcaliburConfiguration Read()
    {
        var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var filename = Path.Combine(path, ConfigPath, "config.json");

        if (File.Exists(filename))
        {
            return JsonSerializer.Deserialize<ExcaliburConfiguration>(filename)!;
        }

        return new ExcaliburConfiguration();
    }

    public void Save(ExcaliburConfiguration configuration)
    {
        var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var filename = Path.Combine(path, ConfigPath, "config.json");

        JsonSerializer.Serialize(File.OpenWrite(filename), configuration);
    }
}
