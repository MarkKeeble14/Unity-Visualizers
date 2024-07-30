using TMPro;
using UnityEngine;

public class VisualizerElementInputSetting : VisualizerSetting
{
    [SerializeField] private TMP_InputField inputField;

    public void OnInput(string str)
    {
        throw new System.NotImplementedException();
    }

    protected override void Initialize()
    {
        // 
    }
}