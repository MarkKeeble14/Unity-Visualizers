using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public abstract class DropdownMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected TextMeshProUGUI indexText;
    [SerializeField] protected TextMeshProUGUI labelText;
    [SerializeField] private Canvas listCanvas;
    [SerializeField] private RectTransform list;
    [SerializeField] private RectTransform dropdown;

    [Header("Prefabs")]
    [SerializeField] private DropdownElement templateElement;

    [Header("Settings")]
    [SerializeField] private int elementHeight;
    protected List<DropdownElement> dropdownElements = new();
    [SerializeField] private string elementType;

    public Action<int> OnSelectElement;
    protected int selectedIndex;
    private bool isOpen;
    public bool IsOpen => isOpen;

    private void Awake()
    {
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
        if (dropdownElements.Count == 0)
        {
            UIManager._Instance.AddNewMessage(UIManager.MessageClass.WARNING, 
                "Request to open dropdown ignored - No " + elementType + "s to select from. Please load a " + elementType + " to select from first");
            return;
        }

        isOpen = true;

        dropdown.gameObject.SetActive(true);
    }

    public void Close()
    {
        isOpen = false;

        dropdown.gameObject.SetActive(false);
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
        e.AddOnClick(() =>
        {
            int index = e.GetIndex();
            OnSelectElement?.Invoke(index);
        });

        return e;
    }

    public void ActivateElementAtIndex(int index)
    {
        selectedIndex = index;
        indexText.text = index.ToString();
        SetElementActive(index);
        Close();
    }

    protected abstract void SetElementActive(int index);
}
