using UnityEngine;
using UnityEngine.UI;

public class GradientKeyDisplay : MonoBehaviour
{
    [SerializeField] private Color color;
    [SerializeField] private Image image;

    private int index;
    private float time;

    public void Set(int index, Color c, float time)
    {
        this.time = time;
        this.index = index;
        color = c;
        image.color = color;
    }

    public void SetVisible(bool visible)
    {
        image.gameObject.SetActive(visible);
    }

    public void SelectKey()
    {
        GradientEditor._Instance.SetEditingKey(index, color, time);
    }
}
