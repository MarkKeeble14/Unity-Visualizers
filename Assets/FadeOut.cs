using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]  
public class FadeOut : MonoBehaviour
{
    [SerializeField] private float fadeRate = 1;

    [SerializeField] private float delay;
    private float delayTimer;

    private Color currentColor;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        currentColor = image.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (delayTimer < delay)
        {
            delayTimer += Time.deltaTime;
            return;
        }

        if (image.color.a > 0)
        {
            currentColor.a -= Time.deltaTime * fadeRate;
            image.color = currentColor;
        }
    }
}
