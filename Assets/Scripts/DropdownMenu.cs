using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public abstract class DropdownMenu : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI indexText;
    [SerializeField] protected TextMeshProUGUI labelText;

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
        e.AddOnClick(() => SelectElement(e.GetIndex()));
        return e;
    }

    public void SelectElement(int index)
    {
        OnSelectElement?.Invoke(index);
        ActivateElement(index);
        selectedIndex = index;
        Close();
    }

    public void ActivateElement(int index)
    {
        SetElementActive(index);
        indexText.text = index.ToString();
    }

    protected abstract void SetElementActive(int index);
}
