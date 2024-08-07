using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public abstract class AddSetupElementSetting : AddSetting
{
    [SerializeField] protected VisualizerElementLabel label;
    [SerializeField] protected SettingType settingType;
    public abstract VisualizerElementSettingType Type { get; }

    private VisualizerSetting setting;

    public override void MakeSetting()
    {
        VisualizerSetting toSpawn = VisualizerSetupManager._Instance.GetSettingOfType(Type);
        VisualizerSetupElement element = VisualizerManager._Instance.GetSetupElement(label);
        setting = Instantiate(toSpawn, element.ExtraSettingsHolder);

        setting.SetLabel(MakeLabel());
        setting.SetToolTip(settingType);

        InitializeSetting(setting);
    }

    protected abstract void InitializeSetting(VisualizerSetting obj);

    protected string MakeKey()
    {
        return label.ToString() + "_" + settingType.ToString();
    }

    protected string MakeLabel()
    {
        return StringHelper.EnumToTitleCase(settingType.ToString());
    }
}
