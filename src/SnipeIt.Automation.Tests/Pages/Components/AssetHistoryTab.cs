using Microsoft.Playwright;
using SnipeIt.Automation.Tests.Models;
using SnipeIt.Automation.Tests.Pages;
using static Microsoft.Playwright.Assertions;

namespace SnipeIt.Automation.Tests.Pages.Components;

public class AssetHistoryTab : BasePage
{
    public AssetHistoryTab(IPage page) : base(page) { }

    private ILocator TabLink => Page.Locator("a[href='#history']");
    private ILocator Panel => Page.Locator("#history");
    private ILocator Rows => Panel.Locator("table tbody tr");

    public async Task OpenAsync()
    {
        await TabLink.ClickAsync();
        await Expect(Panel).ToBeVisibleAsync();
        await Expect(Rows.First).ToBeVisibleAsync(); // The table loads via AJAX.
        await Expect(Rows).ToHaveCountAsync(2);
    }

    /// <summary>Read the entire table into a list of entries, mapping columns by header name.</summary>
    public async Task<List<AssetHistoryEntry>> ReadEntriesAsync()
    {
        var headers = (await Panel.Locator("table thead th").AllInnerTextsAsync())
            .Select(h => h.Trim())
            .ToList();

        int Col(string name) => headers.FindIndex(h => h.Equals(name, StringComparison.OrdinalIgnoreCase));

        int createdBy = Col("Created By"), action = Col("Action"),
            item = Col("Item"), target = Col("Target"), notes = Col("Notes");

        Assert.That(new[] { createdBy, action, item, target, notes }, Has.None.EqualTo(-1),
            $"Không tìm thấy đủ cột trong bảng History. Các cột hiện có: {string.Join(", ", headers)}");

        var entries = new List<AssetHistoryEntry>();
        var count = await Rows.CountAsync();

        for (var i = 0; i < count; i++)
        {
            var cells = (await Rows.Nth(i).Locator("td").AllInnerTextsAsync())
                .Select(c => c.Trim())
                .ToList();

            entries.Add(new AssetHistoryEntry(
                CreatedBy: cells[createdBy],
                Action: cells[action],
                Item: cells[item],
                Target: cells[target],
                Notes: cells[notes]));
        }

        return entries;
    }

    public async Task ExpectHistoryAsync(AssetData data, string createdBy = "Admin User")
    {
        var entries = await ReadEntriesAsync();
        var expectedItem = $"{data.Name} #{data.AssetTag} - {data.ModelName}";

        var create = entries.Single(e => e.Action.Contains("create", StringComparison.OrdinalIgnoreCase));
        var checkout = entries.Single(e => e.Action.Contains("checkout", StringComparison.OrdinalIgnoreCase));

        Assert.Multiple(() =>
        {
            Assert.That(entries, Has.Count.EqualTo(2));

            Assert.That(create.CreatedBy, Is.EqualTo(createdBy));
            Assert.That(create.Item, Is.EqualTo(expectedItem));

            Assert.That(checkout.CreatedBy, Is.EqualTo(createdBy));
            Assert.That(checkout.Item, Is.EqualTo(expectedItem));
            Assert.That(checkout.Target, Is.EqualTo(data.CheckedOutToName));
            Assert.That(checkout.Notes, Is.EqualTo("Checked out on asset creation"));
        });
    }
}