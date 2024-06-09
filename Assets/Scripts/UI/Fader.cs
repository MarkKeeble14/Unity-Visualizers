using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]  
public class Fader : MonoBehaviour
{
    public static Fader _Instance { get; private set; }
    [SerializeField] private float fadeRate = 1;
    private float fadeTarget = 1;
    [SerializeField] private float delay;
    private float delayTimer;
    private Color currentColor;
    private Image image;
    [SerializeField] private bool paused;

    private void Awake()
    {
        if (_Instance != null) Destroy(gameObject);

        _Instance = this;

        image = GetComponent<Image>();
        currentColor = image.color;
    }

    public void FadeInBlocker() { fadeTarget = 1; }
    public void FadeOutBlocker() { fadeTarget = 0; }

    public void SetDelay(float delay) { this.delay = delay; }

    public void SetPaused(bool newValue) { paused = newValue; }

    // Update is called once per frame
    void Update()
    {
        if (delayTimer < delay)
        {
            delayTimer += Time.deltaTime;
            return;
        }

        currentColor.a = Mathf.MoveTowards(currentColor.a, fadeTarget, Time.deltaTime * fadeRate);
        image.color = currentColor;
    }
}
