using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EditMaterialPropertiesForVisualizerSpecificElement : MonoBehaviour, IRecieveVisualizerSpecificElementsInfo, IRecieveVisualizerFloatValues
{
    [SerializeField] private string key;
    [SerializeField] private Material mat;
    [SerializeField] private string emissionIntensityKey;
    [SerializeField] private float defaultEmissionIntensity;
    private float emissionIntensity;

    private void Start()
    {
        Color startColor = VisualizerManager._Instance.GetColor(VisualizerColorType.COLOR, 0);
        mat.SetColor("_BaseColor", startColor);
        mat.SetColor("_EmissionColor", startColor);
        emissionIntensity = defaultEmissionIntensity;
    }

    public void RecieveVisualizerSpecificElementsInfo(Dictionary<string, VisualizerElementsSettings> info)
    {
        if (!info.ContainsKey(key))
        {
            return;
        }

        SetColors(VisualizerManager._Instance.GetColor(info[key].ColorType, info[key].ColorIndex), emissionIntensity);
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
}
