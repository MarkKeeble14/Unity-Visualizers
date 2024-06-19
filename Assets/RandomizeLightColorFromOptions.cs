using System.Collections.Generic;
using UnityEngine;

public class RandomizeLightColorFromOptions : RandomizeLightColor
{
    [SerializeField] private List<Color> colors = new();
    protected override Color GetRandomColor()
    {
        return RandomHelper.GetRandomFromList(colors);
    }
}
