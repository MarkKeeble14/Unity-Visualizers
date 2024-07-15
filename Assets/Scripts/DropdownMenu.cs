using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public abstract class DropdownMenu : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI indexText;
    [SerializeField] protected TextMeshProUGUI labelText;

    [SerializeField] private Canvas listCanvas;

    private RectTransform rectTransform;
    [SerializeField] private RectTransform list;

    [SerializeField] private DropdownElement templateElement;

    [SerializeField] private int elementHeight;

    protected List<DropdownElement> dropdownElements = new();

    public Action<int> OnSelectElement;

    protected int selectedIndex;
    private bool isOpen;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        listCanvas.sortingOrder = GetComponentInParent<Canvas>().sortingOrder + 1;
    }

    public void Click()
    {
        if (isOpen)
        {
            Close();
        } else
        {
            Open();
        }
    }

    private void Open()
    {
        isOpen = true;

        list.gameObject.SetActive(true);
    }

    private void Close()
    {
        isOpen = false;
        list.gameObject.SetActive(false);
    }

    protected void Clear()
    {
        foreach (Transform child in list.transform)
        {
            Destroy(child.gameObject);
        }
        dropdownElements.Clear();
    }

    protected DropdownElement CreateElementObject()
    {
        DropdownElement e = Instantiate(templateElement, list);

        RectTransform r = e.GetComponent<RectTransform>();
        Vector2 sizeDelta = r.sizeDelta;
        sizeDelta.y = elementHeight;
        r.sizeDelta = sizeDelta;

        dropdownElements.Add(e);
        e.SetIndex(dropdownElements.Count - 1);
        e.AddOnClick(() => SelectElementAtIndex(e.GetIndex()));

        return e;
    }

    public void SelectElementAtIndex(int index)
    {
        OnSelectElement?.Invoke(index);
        ActivateElementAtIndex(index);
        Close();
    }

    public void ActivateElementAtIndex(int index)
    {
        selectedIndex = index;
        indexText.text = index.ToString();
        SetElementActive(index);
    }

    protected abstract void SetElementActive(int index);
}
