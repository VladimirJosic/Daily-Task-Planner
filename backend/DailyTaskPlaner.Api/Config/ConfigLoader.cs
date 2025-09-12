namespace DailyTaskPlaner.Api.Config;

internal static class ConfigLoader
{
    public static void LoadDeveloperConfiguration(this WebApplicationBuilder builder)
    {
        string configFolder = Path.Combine(Directory.GetCurrentDirectory(), "Config");
        string configFileName = $"appsettings.{Environment.UserName}.json";

        string configPath = Path.Combine(configFolder, configFileName);

        if (File.Exists(configPath))
        {
            builder.Configuration.AddJsonFile(configPath, optional: true, reloadOnChange: true);
            Console.WriteLine($"Loaded developer-specific configuration: {configPath}.");
        }
        else
        {
            Console.WriteLine($"No developer-specific configuration file found: {configPath}. Skipping...");
        }
    }
}
