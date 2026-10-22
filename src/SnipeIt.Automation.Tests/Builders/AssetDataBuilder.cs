using SnipeIt.Automation.Tests.Models;
using SnipeIt.Automation.Tests.Models.Enums;

namespace SnipeIt.Automation.Tests.Builders;

public class AssetDataBuilder
{
    private string _model = "Macbook Pro 13\"";
    private AssetStatus _status = AssetStatus.ReadyToDeploy;

    public AssetDataBuilder WithModel(string model) { _model = model; return this; }
    public AssetDataBuilder WithStatus(AssetStatus status) { _status = status; return this; }

    public AssetData Build()
    {
        var stamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");

        return new AssetData
        {
            Name = $"Asset {stamp}",
            Serial = $"SN-{stamp}",
            ModelSearchText = _model,
            Status = _status
        };
    }
}