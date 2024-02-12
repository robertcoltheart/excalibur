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
            var contents = File.ReadAllText(filename);

            return JsonSerializer.Deserialize<ExcaliburConfiguration>(contents)!;
        }

        return new ExcaliburConfiguration();
    }

    public void Save(ExcaliburConfiguration configuration)
    {
        var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var filename = Path.Combine(path, ConfigPath, "config.json");

        Directory.CreateDirectory(Path.Combine(path, ConfigPath));

        var json = JsonSerializer.Serialize(configuration);

        File.WriteAllText(filename, json);
    }
}
