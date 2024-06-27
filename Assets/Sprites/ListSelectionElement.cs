using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutElement))]
public abstract class ListSelectionElement : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI indexText;
    private LayoutElement layoutElement;

    private int index;
    public int Index => index;

    public abstract void OpenListSelection();

    private void Awake()
    {
        layoutElement = GetComponent<LayoutElement>();
    }

    public void SetIndex(int index)
    {
        this.index = index;
        indexText.text = index.ToString();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        layoutElement.ignoreLayout = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        layoutElement.ignoreLayout = false;
    }
}
