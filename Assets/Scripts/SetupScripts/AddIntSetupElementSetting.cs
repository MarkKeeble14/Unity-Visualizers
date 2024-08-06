using UnityEngine;

public class AddIntSetupElementSetting : AddSetupElementSetting
{
    public override VisualizerElementSettingType Type => VisualizerElementSettingType.INT;

    [SerializeField] private int defaultValue;
    private VisualizerElementIntSetting intSetting;

    protected override void InitializeSetting(VisualizerSetting obj)
    {
        intSetting = (VisualizerElementIntSetting)obj;
        intSetting.SetKey(MakeKey());
        intSetting.UpdateSetting(defaultValue.ToString());
    }
}
