using SnipeIt.Automation.Tests.Base;
using SnipeIt.Automation.Tests.Builders;
using SnipeIt.Automation.Tests.Models;
using SnipeIt.Automation.Tests.Pages;
using SnipeIt.Automation.Tests.Pages.Assets;
using SnipeIt.Automation.Tests.Pages.Components;

namespace SnipeIt.Automation.Tests.Tests.Assets;

[TestFixture]
public class AssetLifecycleTests : BaseTest
{
    [Test]
    public async Task CreateAsset_ShouldBeSearchableAndShowCorrectDetails()
    {
        var loginPage = new LoginPage(Page);
        var createPage = new CreateAssetPage(Page);
        var listPage = new AssetsListPage(Page);
        var detailsPage = new AssetDetailsPage(Page);
        var historyTab = new AssetHistoryTab(Page);

        // Step 1: Login
        // Write log message indicating the start of the login step
        TestContext.Out.WriteLine("1. Starting login step...");
        await loginPage.GotoAsync(Settings.BaseUrl);
        await loginPage.LoginAsync(Settings.Username, Settings.Password);
        // Text screenshot of the login page after login
        await Page.ScreenshotAsync(new() { Path = Path.Combine(TestContext.CurrentContext.WorkDirectory, "TestResults", "login.png"), FullPage = true });
        TestContext.Out.WriteLine("1. Login step completed.");

        // Step 2: Create a Macbook Pro 13" asset - Ready to Deploy - check it out to a random user.
        TestContext.Out.WriteLine("2. Starting asset creation step...");
        await createPage.GotoAsync(Settings.BaseUrl);
        AssetData asset = await createPage.CreateAsync(new AssetDataBuilder().Build());
        TestContext.Out.WriteLine(
            $"Created asset for company={asset.Company}, tag={asset.AssetTag}, name={asset.Name}, serial={asset.Serial}, " +
            $" model={asset.ModelDisplayText}, user={asset.CheckedOutTo}");
        TestContext.Out.WriteLine("2. Asset creation completed.");
        // Step 2.5: Take screenshot of the created asset
        await Page.ScreenshotAsync(new() { Path = Path.Combine(TestContext.CurrentContext.WorkDirectory, "TestResults", $"{asset.AssetTag}_created.png"), FullPage = true });
        TestContext.Out.WriteLine("Screenshot of created asset taken and saved to TestResults folder.");

        // Step 3: Find the asset in the list.
        TestContext.Out.WriteLine("3. Starting asset search step...");
        await listPage.GotoAsync(Settings.BaseUrl);
        await listPage.SearchByAssetTagAsync(asset.AssetTag);
        await listPage.ExpectAssetRowAsync(asset);
        TestContext.Out.WriteLine("3. Asset search completed.");
        // Step 3.5: Take screenshot of the asset in the list
        await Page.ScreenshotAsync(new() { Path = Path.Combine(TestContext.CurrentContext.WorkDirectory, "TestResults", $"{asset.AssetTag}_list.png"), FullPage = true });
        TestContext.Out.WriteLine("Screenshot of asset in the list taken and saved to TestResults folder.");

        // Step 4: Open the details page from the list and verify it.
        TestContext.Out.WriteLine("4. Starting asset details verification step...");
        await listPage.OpenAssetAsync(asset.AssetTag);
        await detailsPage.ExpectDetailsAsync(asset);
        TestContext.Out.WriteLine("4. Asset details verification completed.");
        // Step 4.5: Take screenshot of the asset details page
        await Page.ScreenshotAsync(new() { Path = Path.Combine(TestContext.CurrentContext.WorkDirectory, "TestResults", $"{asset.AssetTag}_details.png"), FullPage = true });
        TestContext.Out.WriteLine("Screenshot of asset details page taken and saved to TestResults folder.");

        // Step 5: Verify the History tab (to be done in the next step).
        TestContext.Out.WriteLine("5. Starting asset history verification step...");
        await historyTab.OpenAsync();
        await historyTab.ExpectHistoryAsync(asset);
        TestContext.Out.WriteLine("5. Asset history verification completed.");
        // take screenshot of the asset history tab
        await Page.ScreenshotAsync(new() { Path = Path.Combine(TestContext.CurrentContext.WorkDirectory, "TestResults", $"{asset.AssetTag}_history.png"), FullPage = true });
        TestContext.Out.WriteLine("Screenshot of asset history tab taken and saved to TestResults folder.");
    }
}