using UnityEngine;

public class AddFloatSetupElementSetting : AddSetupElementSetting
{
    public override VisualizerElementSettingType Type => VisualizerElementSettingType.FLOAT;

    [SerializeField] private float defaultValue;
    [SerializeField] private string floatValueDBKey;
    private VisualizerElementFloatSetting floatSetting;

    protected override void InitializeSetting(VisualizerSetting obj)
    {
        floatSetting = (VisualizerElementFloatSetting)obj;
        floatSetting.SetKey(MakeKey());
        floatSetting.UpdateSetting(defaultValue.ToString());
    }

    protected override string MakeKey()
    {
        return setupElementKey + "_" + floatValueDBKey;
    }
}
