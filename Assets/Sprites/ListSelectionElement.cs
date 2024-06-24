using UnityEngine;
using TMPro;

public abstract class ListSelectionElement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI indexText;

    private int index;
    public int Index => index;

    public void SetIndex(int index)
    {
        this.index = index;
        indexText.text = index.ToString();
    }

    public abstract void OpenListSelection();
}
