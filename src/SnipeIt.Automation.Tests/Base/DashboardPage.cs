using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace SnipeIt.Automation.Tests.Pages;

public class DashboardPage : BasePage
{
    public DashboardPage(IPage page) : base(page) { }

    public async Task ExpectLoadedAsync()
        // expect the dashboard page to be loaded by checking the URL
        => await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex(@"demo\.snipeitapp\.com/?$"));

        
}