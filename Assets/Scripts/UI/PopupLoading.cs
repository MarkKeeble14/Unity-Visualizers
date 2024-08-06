using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupLoading : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public string Text => text.text;

    private float timeAlive = 0;
    public float TimeAlive => timeAlive;


    public void Set(string text)
    {
        this.text.text = text;
    }

    private void Update()
    {
        timeAlive += Time.deltaTime;
    }
}
