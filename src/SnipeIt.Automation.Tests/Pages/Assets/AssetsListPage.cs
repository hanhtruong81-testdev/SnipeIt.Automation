using System.Text.RegularExpressions;
using Microsoft.Playwright;
using SnipeIt.Automation.Tests.Models;
using static Microsoft.Playwright.Assertions;
using SnipeIt.Automation.Tests.Models.Enums;
namespace SnipeIt.Automation.Tests.Pages.Assets;

public class AssetsListPage : BasePage
{
    public AssetsListPage(IPage page) : base(page) { }

    private ILocator SearchInput => Page.Locator("input.search-input");
    private ILocator Rows => Page.Locator("table tbody tr");

    private ILocator RowFor(string assetTag) =>
        Rows.Filter(new() { HasText = assetTag });

    public async Task GotoAsync(string baseUrl)
    {
        await Page.GotoAsync($"{baseUrl}/hardware");
        await Expect(SearchInput).ToBeVisibleAsync();
    }

    /// <summary>Search by asset tag and wait for the table to finish filtering (exactly one row remains).</summary>
    public async Task SearchByAssetTagAsync(string assetTag)
    {
        await SearchInput.ClickAsync();
        // Bootstrap Table listens for keyboard events, so use PressSequentially instead of Fill.
        await SearchInput.PressSequentiallyAsync(assetTag, new() { Delay = 20 });

        await Expect(RowFor(assetTag)).ToBeVisibleAsync();
        await Expect(Rows).ToHaveCountAsync(1);
    }

    /// <summary>Verify that the asset row in the list matches the data just created.</summary>
    public async Task ExpectAssetRowAsync(AssetData data)
    {
        var row = RowFor(data.AssetTag);

        await Expect(row).ToContainTextAsync(data.Serial);
        await Expect(row).ToContainTextAsync(data.Name);
        await Expect(row).ToContainTextAsync(data.ModelName);
        await Expect(row).ToContainTextAsync(data.Category);
        await Expect(row).ToContainTextAsync(data.Status.ToDisplayText());
        await Expect(row).ToContainTextAsync(data.CheckedOutToName);
    }

    /// <summary>Click the Asset Tag link in the row to open the details page.</summary>
    public async Task OpenAssetAsync(string assetTag)
    {
        await RowFor(assetTag)
            .GetByRole(AriaRole.Link, new() { Name = assetTag, Exact = true })
            .ClickAsync();

        await Expect(Page).ToHaveURLAsync(new Regex(@"/hardware/\d+"));
    }
}