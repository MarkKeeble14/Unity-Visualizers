using UnityEngine;

public class AddBoolGeneralSetting : AddGeneralSetting
{
    public override VisualizerElementSettingType Type => VisualizerElementSettingType.BOOL;

    [SerializeField] private bool defaultValue;
    private VisualizerElementBoolSetting boolSetting;

    protected override void InitializeSetting(VisualizerSetting obj)
    {
        boolSetting = (VisualizerElementBoolSetting)obj;
        boolSetting.SetKey(MakeKey());
        boolSetting.Set(defaultValue);
    }
}