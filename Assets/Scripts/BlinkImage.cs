using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkImage : MonoBehaviour
{
    [SerializeField] private float blinkSpeed = 1;
    [SerializeField] private Image image;
    private float timer;
    private Color c;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime * blinkSpeed;

        c = image.color;
        if (Mathf.FloorToInt(timer) % 2 == 0)
            c.a = 0;
        else
            c.a = 1;
        image.color = c;
    }
}
