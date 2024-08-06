using UnityEngine;

public abstract class AddGeneralSetting : MonoBehaviour
{
    [SerializeField] protected SettingType settingType;
    public abstract VisualizerElementSettingType Type { get; }

    private void Start()
    {
        VisualizerSetting toSpawn = VisualizerSetupManager._Instance.GetSettingOfType(Type);
        VisualizerSetting spawned = Instantiate(toSpawn, VisualizerManager._Instance.GeneralSettingsList);

        spawned.SetLabel(MakeLabel());
        spawned.SetToolTip(settingType);

        InitializeSetting(spawned);

        spawned.SetHeight(VisualizerManager._Instance.DefaultExtraSettingHeight);
    }

    protected abstract void InitializeSetting(VisualizerSetting obj);

    protected string MakeKey()
    {
        return settingType.ToString();
    }

    protected string MakeLabel()
    {
        return StringHelper.EnumToTitleCase(settingType.ToString());
    }
}