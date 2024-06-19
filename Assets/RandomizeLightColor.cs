using System.Collections;
using UnityEngine;

public abstract class RandomizeLightColor : MonoBehaviour
{
    [SerializeField] private Light light;

    protected abstract Color GetRandomColor();

    // Start is called before the first frame update
    void Start()
    {
        light.color = GetRandomColor();
    }
}
