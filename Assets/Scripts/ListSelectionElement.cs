using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutElement))]
public abstract class ListSelectionElement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI indexText;

    private int index;
    public int Index => index;

    public abstract void Open();
    public abstract void Delete();

    public void SetIndex(int index)
    {
        this.index = index;
        indexText.text = index.ToString();
    }

    public virtual void Randomize() { }
}
