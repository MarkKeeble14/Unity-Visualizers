using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public abstract class AddSetupElementSetting : MonoBehaviour, IRecieveVisualizerElementsInfo
{
    [SerializeField] protected VisualizerElementLabel label;
    [SerializeField] protected SettingType settingType;
    public abstract VisualizerElementSettingType Type { get; }

    private bool hasCreatedSetupElement;

    protected abstract void InitializeSetting(VisualizerSetting obj);

    protected string MakeKey()
    {
        return label.ToString() + "_" + settingType.ToString();
    }

    protected string MakeLabel()
    {
        return StringHelper.EnumToTitleCase(settingType.ToString());
    }

    public void RecieveVisualizerElementsInfo(Dictionary<VisualizerElementLabel, VisualizerElementsSettings> info)
    {
        if (hasCreatedSetupElement) return;
        hasCreatedSetupElement = true;

        VisualizerSetting toSpawn = VisualizerSetupManager._Instance.GetSettingOfType(Type);
        VisualizerSetupElement element = VisualizerManager._Instance.GetSetupElement(label);
        VisualizerSetting spawned = Instantiate(toSpawn, element.ExtraSettingsHolder);

        spawned.SetLabel(MakeLabel());
        spawned.SetToolTip(settingType);

        InitializeSetting(spawned);
    }
}
