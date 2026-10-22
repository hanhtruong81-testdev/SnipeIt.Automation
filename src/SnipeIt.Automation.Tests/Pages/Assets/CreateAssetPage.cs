using System.Text.RegularExpressions;
using Microsoft.Playwright;
using SnipeIt.Automation.Tests.Models;
using SnipeIt.Automation.Tests.Models.Enums;
using SnipeIt.Automation.Tests.Pages.Components;
using static Microsoft.Playwright.Assertions;

namespace SnipeIt.Automation.Tests.Pages.Assets;

public class CreateAssetPage : BasePage
{
    private readonly Select2Dropdown _company;
    private readonly Select2Dropdown _model;
    private readonly Select2Dropdown _status;
    private readonly Select2Dropdown _checkoutUser;

    public CreateAssetPage(IPage page) : base(page)
    {
        _company = new Select2Dropdown(page, "company_id_select");
        _model = new Select2Dropdown(page, "model_select_id");
        _status = new Select2Dropdown(page, "status_select_id");
        _checkoutUser = new Select2Dropdown(page, "assigned_user_select");
    }
    private ILocator NameInput => Page.Locator("#name");
    private ILocator OptionalDetails => Page.Locator("#optional_details");
    private ILocator OptionalInformationHeader =>
    Page.GetByText("Optional Information", new() { Exact = true });
    private ILocator AssetTagInput => Page.Locator("#asset_tag");
    private ILocator SerialInput => Page.Locator("#serial_1");

    // The page has two Save buttons (top and bottom), so use .First to avoid a strict mode violation.
    private ILocator SaveButton =>
        Page.GetByRole(AriaRole.Button, new() { Name = "Save", Exact = true }).First;

    public async Task GotoAsync(string baseUrl)
    {
        await Page.GotoAsync($"{baseUrl}/hardware/create");
        await Expect(AssetTagInput).ToBeVisibleAsync();
    }

    /// <summary>Expand the Optional Information section if it is hidden (it contains the Asset Name field).</summary>
    private async Task ExpandOptionalInformationAsync()
    {
        if (await OptionalDetails.IsVisibleAsync())
            return;

        await OptionalInformationHeader.ClickAsync();
        await Expect(NameInput).ToBeVisibleAsync(); // Wait for the expand animation to finish.
    }
    
    /// <summary>Fill in the form, save it, and return the actual data (asset tag, model, and selected user).</summary>
    public async Task<AssetData> CreateAsync(AssetData data)
    {
        // The application pre-generates the asset tag; read it back for use in the verification steps.
        data.AssetTag = await AssetTagInput.InputValueAsync();
        data.Company = await _company.SelectRandomAsync();
        
        await ExpandOptionalInformationAsync();
        await NameInput.FillAsync(data.Name);
        
        await SerialInput.FillAsync(data.Serial);
        data.ModelDisplayText = await _model.SelectByTextAsync(data.ModelSearchText);
        await _status.SelectByTextAsync(data.Status.ToDisplayText());

        // The Checkout to section (the "User" tab by default) appears only after selecting Ready to Deploy.
        await _checkoutUser.WaitUntilVisibleAsync();
        data.CheckedOutTo = await _checkoutUser.SelectRandomAsync();

        await SaveButton.ClickAsync();

        // After a successful save, the page navigates away from /hardware/create.
        await Expect(Page).Not.ToHaveURLAsync(new Regex(@"/hardware/create"));

        return data;
    }
}