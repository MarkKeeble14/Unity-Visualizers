using UnityEngine;

public abstract class AddSetupElementSetting : MonoBehaviour
{
    [SerializeField] private string displayLabel;
    [SerializeField] private ToolTipContentType toolTipOnHover;
    [SerializeField] protected string setupElementKey;
    public abstract VisualizerElementSettingType Type { get; }

    private void Start()
    {
        VisualizerSetting toSpawn = VisualizerSetupManager._Instance.GetSettingOfType(Type);
        VisualizerSetting spawned = Instantiate(toSpawn, 
                                        VisualizerManager._Instance.GetSetupElementExtraSettingsTransform(setupElementKey));

        spawned.SetLabel(displayLabel);
        spawned.SetToolTip(toolTipOnHover);

        InitializeSetting(spawned);
    }

    protected abstract void InitializeSetting(VisualizerSetting obj);

    protected virtual string MakeKey()
    {
        return setupElementKey;
    }
}
