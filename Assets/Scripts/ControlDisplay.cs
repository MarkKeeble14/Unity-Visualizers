using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControlDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private TextMeshProUGUI key;

    public void Set(string label, string key)
    {
        this.label.text = label;
        this.key.text = key;
    }
}
