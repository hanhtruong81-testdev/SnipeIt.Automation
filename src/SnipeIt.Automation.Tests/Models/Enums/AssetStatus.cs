namespace SnipeIt.Automation.Tests.Models.Enums;

public enum AssetStatus
{
    ReadyToDeploy
}

public static class AssetStatusExtensions
{
    public static string ToDisplayText(this AssetStatus status) => status switch
    {
        AssetStatus.ReadyToDeploy => "Ready to Deploy",
        _ => throw new ArgumentOutOfRangeException(nameof(status))
    };
}