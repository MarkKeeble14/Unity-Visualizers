using UnityEngine;
using UnityEngine.UI;

public class InitializeSliderValueToAudioSourceVolume : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Slider slider;

    private void Awake()
    {
        slider.value = audioSource.volume;
    }
}
