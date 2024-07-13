using TMPro;
using UnityEngine;

public abstract class VisualizerSetting : MonoBehaviour
{
    [SerializeField] private string label;
    [SerializeField] protected string key;
    [SerializeField] private TextMeshProUGUI labelText;

    private void Start()
    {
        labelText.text = label;

        Initialize();
    }

    protected abstract void Initialize();
}
