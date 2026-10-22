using Microsoft.Playwright;

namespace SnipeIt.Automation.Tests.Pages;

public abstract class BasePage
{
    protected IPage Page { get; }

    protected BasePage(IPage page) => Page = page;
}