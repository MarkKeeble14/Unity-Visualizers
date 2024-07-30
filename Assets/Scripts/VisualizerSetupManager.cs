using System.Collections.Generic;
using UnityEngine;

public enum VisualizerElementSettingType
{
    FLOAT,
    INT,
    BOOL,
    INPUT,
    DROPDOWN
}

public class VisualizerSetupManager : MonoBehaviour
{
    public static VisualizerSetupManager _Instance { get; private set; }

    [SerializeField] private List<SerializableKeyValuePair<VisualizerElementSettingType, VisualizerSetting>> visualizerElementSettingPrefabDict = new();

    private void Awake()
    {
        if (_Instance != null) Destroy(_Instance.gameObject);
        _Instance = this;
    }

    public VisualizerSetting GetSettingOfType(VisualizerElementSettingType type)
    {
        foreach (SerializableKeyValuePair<VisualizerElementSettingType, VisualizerSetting> kvp in visualizerElementSettingPrefabDict)
        {
            if (kvp.Key == type)
            {
                return kvp.Value;
            }
        }
        throw new ElementWithKeyNotFoundException(typeof(VisualizerSetting), type.ToString());
    }
}
