using Microsoft.Playwright;

namespace SnipeIt.Automation.Tests.Pages;

public class LoginPage : BasePage
{
    public LoginPage(IPage page) : base(page) { }

    private ILocator UsernameInput => Page.Locator("#username");
    
    private ILocator PasswordInput => Page.Locator("#password-field"); // locate password input field by ID instead of label
    private ILocator LoginButton => Page.GetByRole(AriaRole.Button, new() { Name = "Login" });

    public async Task GotoAsync(string baseUrl)
        => await Page.GotoAsync($"{baseUrl}/login");

    public async Task LoginAsync(string username, string password)
    {
        await UsernameInput.FillAsync(username);
        await PasswordInput.FillAsync(password);
        await LoginButton.ClickAsync();
    }
}