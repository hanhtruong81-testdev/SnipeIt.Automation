using Microsoft.Playwright;
using SnipeIt.Automation.Tests.Models;
using SnipeIt.Automation.Tests.Models.Enums;
using static Microsoft.Playwright.Assertions;

namespace SnipeIt.Automation.Tests.Pages.Assets;

public class AssetDetailsPage : BasePage
{
    public AssetDetailsPage(IPage page) : base(page) { }
    private ILocator AssetName => Page.Locator(".js-copy-asset_name");
    private ILocator AssetTag => Page.Locator(".js-copy-asset_tag");
    private ILocator Serial => Page.Locator(".js-copy-license_key");
    private ILocator Model => Page.Locator(".js-copy-asset_model");
    private ILocator ModelNumber => Page.Locator(".js-copy-model_number");
    private ILocator Category => Page.Locator(".js-copy-category");

    // The "Ready to Deploy [Deployed] -> <user>" panel at the top of the page.
    private ILocator StatusPanel =>
        Page.Locator("div.well").Filter(new() { Has = Page.Locator("a[href*='/users/']") });

    public async Task ExpectDetailsAsync(AssetData data)
    {
        await Expect(AssetName).ToHaveTextAsync(data.Name);
        await Expect(AssetTag).ToHaveTextAsync(data.AssetTag);
        await Expect(Serial).ToHaveTextAsync(data.Serial);
        await Expect(Model).ToHaveTextAsync(data.ModelName);
        await Expect(Category).ToHaveTextAsync(data.Category);

        if (!string.IsNullOrEmpty(data.ModelNumber))
            await Expect(ModelNumber).ToHaveTextAsync(data.ModelNumber);

        await Expect(StatusPanel).ToContainTextAsync(data.Status.ToDisplayText());
        await Expect(StatusPanel).ToContainTextAsync("Deployed");
        await Expect(StatusPanel.GetByRole(AriaRole.Link, new() { Name = data.CheckedOutToName }))
            .ToBeVisibleAsync();
    }
}