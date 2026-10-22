using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using SnipeIt.Automation.Tests.Config;

namespace SnipeIt.Automation.Tests.Base;

public abstract class BaseTest : PageTest
{
    protected TestSettings Settings => SettingsProvider.Settings;

    public override BrowserNewContextOptions ContextOptions() => new()
    {
        ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
        IgnoreHTTPSErrors = true
    };

    [SetUp]
    public void ConfigureTimeouts()
    {
        Page.SetDefaultTimeout(Settings.DefaultTimeoutMs);
        Page.SetDefaultNavigationTimeout(Settings.DefaultTimeoutMs * 2);
    }

    [SetUp]
    public async Task StartTracing()
    {
        await Context.Tracing.StartAsync(new() { Screenshots = true, Snapshots = true, Sources = true });
    }

    [TearDown]
    public async Task SaveArtifactsOnFailure()
    {
        var failed = TestContext.CurrentContext.Result.Outcome.Status
            == NUnit.Framework.Interfaces.TestStatus.Failed;

        var dir = Path.Combine(TestContext.CurrentContext.WorkDirectory, "TestResults");
        Directory.CreateDirectory(dir);
        var name = TestContext.CurrentContext.Test.Name;

        if (failed)
        {
            await Page.ScreenshotAsync(new() { Path = Path.Combine(dir, $"{name}.png"), FullPage = true });
            await Context.Tracing.StopAsync(new() { Path = Path.Combine(dir, $"{name}.zip") });
        }
        else
        {
            await Context.Tracing.StopAsync(); // Do not save if the test passes.
        }
    }
}