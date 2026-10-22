using SnipeIt.Automation.Tests.Base;
using SnipeIt.Automation.Tests.Pages;

namespace SnipeIt.Automation.Tests.Tests;

[TestFixture]
public class LoginTests : BaseTest
{
    [Test]
    public async Task Login_WithValidCredentials_ShouldReachDashboard()
    {
        var loginPage = new LoginPage(Page);
        var dashboard = new DashboardPage(Page);

        await loginPage.GotoAsync(Settings.BaseUrl);
        await loginPage.LoginAsync(Settings.Username, Settings.Password);

        await dashboard.ExpectLoadedAsync();
    }
}