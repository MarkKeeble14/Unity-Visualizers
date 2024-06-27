using UnityEngine;
using UnityEngine.UI;

public class GradientListElement : ListSelectionElement
{
    [SerializeField] private Image image;

    public void Set(int index, Gradient g)
    {
        SetIndex(index);
        // image.color = c;
    }

    public override void OpenListSelection()
    {
        Debug.Log("Opening Gradient Selection for Color #" + Index);
    }
}
