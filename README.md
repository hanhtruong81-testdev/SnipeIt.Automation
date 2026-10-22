# SnipeIt.Automation

End-to-end UI automation for the [Snipe-IT demo](https://demo.snipeitapp.com), built with **.NET 10**, **Playwright for .NET** and **NUnit**.

## Scenario covered

A single E2E test (`AssetLifecycleTests`) runs these steps:

1. Log in to https://demo.snipeitapp.com/login
2. Create a new **Macbook Pro 13"** asset with status **Ready to Deploy**, checked out to a **random user**
3. Find the new asset in the assets list (search by asset tag) and verify the row
4. Open the asset from the list and validate its details (asset tag, serial, model, category, status, checked-out user)
5. Validate the **History** tab (a `create new` entry and a `checkout` entry, with item, target and notes)

## Prerequisites

| Tool | Version | Notes |
|---|---|---|
| .NET SDK | 10.x | Check with `dotnet --version` |
| PowerShell (`pwsh`) | 7+ | Needed to install Playwright browsers |
| Internet access | - | The test runs against the public demo site |

