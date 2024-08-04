using UnityEngine;

public class AddTextDropdownSetupElementSetting : AddSetupElementSetting
{
    public override VisualizerElementSettingType Type => VisualizerElementSettingType.TEXT_DROPDOWN;
    [SerializeField] private string[] dropdownElements;

    [SerializeField] private float defaultValueIndex;
    private VisualizerElementTextDropdownSetting dropdownSetting;

    protected override void InitializeSetting(VisualizerSetting obj)
    {
        dropdownSetting = (VisualizerElementTextDropdownSetting)obj;
        dropdownSetting.SetKey(MakeKey());
        dropdownSetting.SetOptions(dropdownElements);

        if (!VisualizerManager._Instance.HasIntSetting(MakeKey()))
        {
            dropdownSetting.UpdateSetting(defaultValueIndex.ToString());
        }
    }
}
