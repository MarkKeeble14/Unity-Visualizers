using UnityEngine;

public class AddFloatSetupElementSetting : AddSetupElementSetting
{
    public override VisualizerElementSettingType Type => VisualizerElementSettingType.FLOAT;

    [SerializeField] private float defaultValue;
    private VisualizerElementFloatSetting floatSetting;

    protected override void InitializeSetting(VisualizerSetting obj)
    {
        floatSetting = (VisualizerElementFloatSetting)obj;
        floatSetting.SetKey(MakeKey());

        if (!VisualizerManager._Instance.HasFloatSetting(MakeKey()))
        {
            floatSetting.SetInputFieldText(defaultValue);
            floatSetting.OnInput(defaultValue.ToString());
        }
    }
}
