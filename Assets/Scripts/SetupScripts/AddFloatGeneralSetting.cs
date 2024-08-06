using UnityEngine;

public class AddFloatGeneralSetting : AddGeneralSetting
{
    public override VisualizerElementSettingType Type => VisualizerElementSettingType.FLOAT;

    [SerializeField] private float defaultValue;
    private VisualizerElementFloatSetting floatSetting;

    protected override void InitializeSetting(VisualizerSetting obj)
    {
        floatSetting = (VisualizerElementFloatSetting)obj;
        floatSetting.SetKey(MakeKey());
        floatSetting.UpdateSetting(defaultValue.ToString());
    }
}
