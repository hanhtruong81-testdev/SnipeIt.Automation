namespace SnipeIt.Automation.Tests.Models;

public record AssetHistoryEntry(
    string CreatedBy,
    string Action,
    string Item,
    string Target,
    string Notes);