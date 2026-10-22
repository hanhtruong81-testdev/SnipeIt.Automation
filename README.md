# SnipeIt.Automation

End-to-end UI automation for the [Snipe-IT demo](https://demo.snipeitapp.com), built with **.NET 10**, **Playwright for .NET** and **NUnit**.

## Scenario covered

A single E2E test (`AssetLifecycleTests`) runs these steps:

1. Log in to https://demo.snipeitapp.com/login
2. Create a new **Macbook Pro 13"** asset with status **Ready to Deploy**, checked out to a **random user**
3. Find the new asset in the assets list (search by asset tag) and verify the row
4. Open the asset from the list and validate its details (asset tag, serial, model, category, status, checked-out user)
5. Validate the **History** tab (a `create new` entry and a `checkout` entry, with item, target and notes)

## Testing approach to full fill the requirement
1. Login with pre-config username / password
2. After logged into the system successfully, a login success message appears on the dashboard page
3. Click on asset page (/hardware) and click + "Create new" button to navigate to "hardware/create" page.
4. Input company, serial number, select model "Laptops - Macbook Pro 13", Set Status to "Ready to Deploy", select a random user from the dropdown list, input Asset Name from "Optional Information" section. Then click on Save button. A new asset created successfully with a Asset Tag align with current timestamp.
5. Use the new created asset tag and input to search box to confirm the asset is appear on asset list.
6. Click on the asset from asset list to view the asset details
7. Check the asset tag, asset name, status, user who deployed to, asset model are matched with what have input.
8. Click on history tab from the asset detail page to see there are 2 entries, one row for status = create new, one row for checkout.

Each assess step will generate a screenshot to folder '/src/SnipeIt.Automation.Tests/bin/Debug/net10.0/TestResults'



## Prerequisites

| Tool | Version | Notes |
|---|---|---|
| .NET SDK | 10.x | Check with `dotnet --version` |
| PowerShell (`pwsh`) | 7+ | Needed to install Playwright browsers |
| Internet access | - | The test runs against the public demo site |

## Setup

From the repository root:

```powershell
dotnet restore
dotnet build

# Install Playwright browsers (once per machine, or after upgrading Playwright)
pwsh src/SnipeIt.Automation.Tests/bin/Debug/net10.0/playwright.ps1 install

# Or Chromium only
pwsh src/SnipeIt.Automation.Tests/bin/Debug/net10.0/playwright.ps1 install chromium
```

If `pwsh` is not available: `dotnet tool install --global PowerShell`, or run the script with Windows PowerShell:
`powershell -ExecutionPolicy Bypass -File <path>/playwright.ps1 install`.

## Running the tests

```powershell
# Headless (default)
dotnet test

# Headed (watch the browser)
$env:HEADED = "1"
dotnet test

# Run only the E2E test with detailed console output
dotnet test --filter AssetLifecycleTests --logger "console;verbosity=detailed"
```

On bash / macOS / Linux use `HEADED=1 dotnet test`.

### Other useful options

```powershell
# Different browser
$env:BROWSER = "firefox"      # chromium (default) | firefox | webkit
dotnet test

# Debug with Playwright Inspector
$env:PWDEBUG = "1"
dotnet test
```

### Debugging failures

On failure, the base test class saves a full-page screenshot and a Playwright trace to `TestResults/`. Open a trace with:

```powershell
pwsh src/SnipeIt.Automation.Tests/bin/Debug/net10.0/playwright.ps1 show-trace TestResults/<TestName>.zip
```

## Configuration

Settings live in `src/SnipeIt.Automation.Tests/appsettings.json`:

```json
{
  "TestSettings": {
    "BaseUrl": "https://demo.snipeitapp.com",
    "Username": "admin",
    "Password": "password",
    "DefaultTimeoutMs": 15000
  }
}
```

Any value can be overridden with an environment variable using `__` as the section separator:

```powershell
$env:TestSettings__BaseUrl = "https://demo.snipeitapp.com"
$env:TestSettings__Password = "..."
```

> The credentials above are the **public demo credentials** shown on the Snipe-IT login page, not real secrets.

## Project structure

```
SnipeIt.Automation/
├── SnipeIt.Automation.sln
├── README.md
└── src/
    └── SnipeIt.Automation.Tests/
        ├── appsettings.json            # Base URL, credentials, timeouts
        ├── Config/
        │   ├── TestSettings.cs         # Strongly-typed settings
        │   └── SettingsProvider.cs     # Loads json + environment variables
        ├── Base/
        │   └── BaseTest.cs             # Extends PageTest: viewport, timeouts, tracing, artifacts
        ├── Pages/                      # Page Object Model
        │   ├── BasePage.cs
        │   ├── LoginPage.cs
        │   ├── DashboardPage.cs
        │   ├── Assets/
        │   │   ├── CreateAssetPage.cs  # Create asset form
        │   │   ├── AssetsListPage.cs   # Search and open asset from list
        │   │   └── AssetDetailsPage.cs # Asset details validation
        │   └── Components/
        │       ├── Select2Dropdown.cs  # Reusable Select2 wrapper
        │       └── AssetHistoryTab.cs  # History tab reader and assertions
        ├── Models/
        │   ├── AssetData.cs            # Data created and later verified
        │   ├── AssetHistoryEntry.cs    # One row of the History table
        │   └── Enums/AssetStatus.cs
        ├── Builders/
        │   └── AssetDataBuilder.cs     # Builds test data (unique serial)
        └── Tests/
            ├── LoginTests.cs           # Login smoke test
            └── Assets/
                └── AssetLifecycleTests.cs  # The E2E scenario
```

## Design decisions

- **Page Object Model with reusable components.** Tests contain no selectors; they call page methods. Snipe-IT uses Select2 dropdowns everywhere, so a single `Select2Dropdown` wrapper (keyed by the underlying `<select>` id) handles Model, Status and User.
- **Stable locators.** Prefer ids, roles and the application's own `js-copy-*` classes over long CSS/XPath. Ambiguous matches are avoided with exact matching (e.g. the Password field is located by `#password-field`, since the page contains several elements with the text "Password").
- **Playwright auto-waiting and web-first assertions.** `Expect(...)` with built-in retries is used instead of `Thread.Sleep`. Ajax tables (assets list, History) are awaited by asserting on their rows.
- **Data created by the test is the data verified.** The asset tag is generated by the application, so it is read back from the form. The serial is unique per run (`SN-<UTC timestamp>`). The selected model text and random user are captured and reused in every later assertion.
- **History table is read by column header name**, not by column index, so assertions survive column reordering. `Assert.Multiple` reports all mismatches at once.
- **Typed data model.** `AssetData` exposes parsed values (`Category`, `ModelName`, `CheckedOutToName`) derived from the Select2 display text, which includes extra information such as `(username) - #id`.
- **Configuration is separate from code** and can be overridden by environment variables, which suits CI.
- **One E2E test with clearly labelled steps.** The five steps depend on each other, so they are kept in a single test instead of order-dependent tests.

## Known limitations

- **Shared, periodically reset demo environment.** Data can be reset at any time (the site shows a "Resetting the Database" page). Random failures such as lost sessions or a missing asset may simply mean a reset occurred during the run; re-run after a few minutes.
- **Random user comes from the first page of results.** The User dropdown loads over AJAX with paging, so the random pick is limited to the rows loaded initially. A user that cannot be checked out to would fail the save step; there is no retry yet.
- **Demo-specific assumptions.** The Macbook Pro 13" model exists and is unique by name, the admin user has a default company, and the History table contains exactly two entries for a freshly created asset.
- **UI text dependencies.** Assertions rely on labels such as `create new`, `checkout` and `Checked out on asset creation`. A different Snipe-IT version or language may require updates.
- **Test duration.** The E2E test takes roughly 30 to 40 seconds because the demo site and Ajax dropdowns are slow.
- **No parallelism.** There is a single test and all runs share the same demo data.

- **Login user**: Currently this script skips asserting who creates the asset in history tab as initally I set it to "Admin User" but I found that it should be a dynamic check instead. I will work on that if I have more time in the future.