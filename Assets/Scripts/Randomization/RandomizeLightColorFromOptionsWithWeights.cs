using UnityEngine;

public class RandomizeLightColorFromOptionsWithWeights : RandomizeLightColor
{
    [SerializeField] private PercentageMap<Color> colors = new();
    protected override Color GetRandomColor()
    {
        return colors.GetOption();
    }
}
