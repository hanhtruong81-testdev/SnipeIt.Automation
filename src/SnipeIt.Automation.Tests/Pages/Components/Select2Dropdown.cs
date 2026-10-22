using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace SnipeIt.Automation.Tests.Pages.Components;

/// <summary>
/// Wrapper for a Select2 dropdown. Accepts the id of the original &lt;select&gt; element
/// (e.g., model_select_id); the displayed container is #select2-{id}-container.
/// </summary>
public class Select2Dropdown
{
    private readonly IPage _page;
    private readonly string _selectId;

    public Select2Dropdown(IPage page, string selectId)
    {
        _page = page;
        _selectId = selectId;
    }

    private ILocator Selection =>
        _page.Locator($"span.select2-selection:has(#select2-{_selectId}-container)");

    private ILocator SearchInput => _page.Locator("input.select2-search__field");

    private ILocator OpenDropdown => _page.Locator("span.select2-dropdown");

    // Only actual option rows; exclude "Searching..." and "No results".
    private ILocator Options => _page.Locator(
        "li.select2-results__option[role='option']:not(.loading-results)");

    public ILocator SelectedTextLocator => _page.Locator($"#select2-{_selectId}-container");

    public Task WaitUntilVisibleAsync() => Selection.WaitForAsync();

    public async Task<string> SelectByTextAsync(string text)
    {
        await OpenAsync();

        if (await SearchInput.IsVisibleAsync())
            await SearchInput.FillAsync(text);

        // Auto-retry until an option containing the text appears, avoiding stale results before AJAX finishes loading.
        var contains = Options.Filter(new() { HasText = text });
        await contains.First.WaitForAsync();

        var exact = Options.Filter(new()
        {
            HasTextRegex = new Regex($@"^\s*{Regex.Escape(text)}\s*$", RegexOptions.IgnoreCase)
        });

        var target = await exact.CountAsync() > 0 ? exact.First : contains.First;

        var selectedText = (await target.InnerTextAsync()).Trim();
        await target.ClickAsync();
        return selectedText;
    }

    public async Task<string> SelectRandomAsync()
    {
        await OpenAsync();
        await Options.First.WaitForAsync();

        var count = await Options.CountAsync();
        var target = Options.Nth(Random.Shared.Next(count));

        var selectedText = (await target.InnerTextAsync()).Trim();
        await target.ClickAsync();
        return selectedText;
    }

    private async Task OpenAsync()
    {
        await Selection.ClickAsync();
        await OpenDropdown.WaitForAsync();
    }
}