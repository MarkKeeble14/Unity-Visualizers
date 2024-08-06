using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EditMaterialPropertiesForVisualizerSpecificElement : MonoBehaviour, IRecieveVisualizerElementsInfo, IRecieveVisualizerFloatValues
{
    [SerializeField] private VisualizerElementLabel label;
    [SerializeField] private Material mat;
    [SerializeField] private string emissionIntensityKey;
    private float emissionIntensity;

    private void Start()
    {
        Color startColor = VisualizerManager._Instance.GetColor(VisualizerColorType.COLOR, 0);
        mat.SetColor("_BaseColor", startColor);
        mat.SetColor("_EmissionColor", startColor);
    }

    public void RecieveVisualizerFloatValues(Dictionary<string, float> values)
    {
        if (!values.ContainsKey(emissionIntensityKey))
        {
            VisualizerManager._Instance.RegisterFloatValue(emissionIntensityKey, emissionIntensity);
            return;
        }

        emissionIntensity = values[emissionIntensityKey];
        SetColors(mat.GetColor("_BaseColor"), emissionIntensity);
    }

    private void SetColors(Color c, float emissionIntensity)
    {
        mat.SetColor("_BaseColor", c);
        mat.SetColor("_EmissionColor", c * emissionIntensity);
    }

    public void RecieveVisualizerElementsInfo(Dictionary<VisualizerElementLabel, VisualizerElementsSettings> info)
    {
        if (!info.ContainsKey(label)) { return; }

        SetColors(VisualizerManager._Instance.GetColor(info[label].ColorType, info[label].ColorIndex), emissionIntensity);
    }
}
