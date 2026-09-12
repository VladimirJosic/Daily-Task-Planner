namespace DailyTaskPlaner.Api.Config;

/// <summary>
/// Loads values from a `.env` file into environment variables before the configuration
/// is built. This is the same mechanism docker compose uses for the microservice
/// version, so that both systems read the same values from the same source.
/// An environment variable that is already set wins, so inside a container it is
/// enough to pass the value through `environment`, with no `.env` file present.
/// </summary>
internal static class DotEnvLoader
{
    public static void LoadDotEnv(this WebApplicationBuilder builder)
    {
        var path = FindDotEnv(Directory.GetCurrentDirectory());

        if (path is null)
        {
            Console.WriteLine("No .env file found. Skipping...");
            return;
        }

        foreach (var raw in File.ReadAllLines(path))
        {
            var line = raw.Trim();

            if (line.Length == 0 || line.StartsWith('#'))
                continue;

            var separator = line.IndexOf('=');
            if (separator <= 0)
                continue;

            var key = line[..separator].Trim();
            var value = line[(separator + 1)..].Trim().Trim('"');

            if (Environment.GetEnvironmentVariable(key) is null)
                Environment.SetEnvironmentVariable(key, value);
        }

        Console.WriteLine($"Loaded environment file: {path}.");

        // Re-read configuration so the variables just set are picked up.
        builder.Configuration.AddEnvironmentVariables();
    }

    /// <summary>Looks for `.env` in the current directory and then upwards, to the repository root.</summary>
    private static string? FindDotEnv(string startDirectory)
    {
        var directory = new DirectoryInfo(startDirectory);

        for (var depth = 0; depth < 6 && directory is not null; depth++)
        {
            var candidate = Path.Combine(directory.FullName, ".env");

            if (File.Exists(candidate))
                return candidate;

            directory = directory.Parent;
        }

        return null;
    }
}
