using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class Service
{
    public string ContainerName { get; set; }
    public string Image { get; set; }
    public Dictionary<string, string> Environment { get; set; }
}

public class ComposeConfig
{
    public Dictionary<string, Service> Services { get; set; }

    public static ComposeConfig Parse(string filePath)
    {
        // Read the content of the YAML file
        string yamlContent = File.ReadAllText(filePath);

        // Create a deserializer instance
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        // Deserialize the YAML content into a ComposeConfig object
        ComposeConfig config = deserializer.Deserialize<ComposeConfig>(yamlContent);

        return config;
    }
}