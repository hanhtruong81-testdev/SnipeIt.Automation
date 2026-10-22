using Microsoft.Extensions.Configuration;

namespace SnipeIt.Automation.Tests.Config;

public static class SettingsProvider
{
    private static readonly Lazy<TestSettings> Instance = new(Load);

    public static TestSettings Settings => Instance.Value;

    private static TestSettings Load()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.CI.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        return configuration
            .GetSection(TestSettings.SectionName)
            .Get<TestSettings>()
            ?? throw new InvalidOperationException("Missing TestSettings section.");
    }
}