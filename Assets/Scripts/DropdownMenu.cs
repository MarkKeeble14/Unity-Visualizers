using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public abstract class DropdownMenu : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI indexText;
    [SerializeField] protected TextMeshProUGUI labelText;
    [SerializeField] protected Image labelTextBackground;

    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private RectTransform list;

    [SerializeField] private DropdownElement templateElement;

    [SerializeField] private int elementHeight;

    protected List<DropdownElement> dropdownElements = new();

    public Action<int> OnSelectElement;

    private bool isOpen;

    private void Awake()
    {
        Vector2 sizeDelta = list.sizeDelta;
        Vector2 anchoredPosition = list.anchoredPosition;

        sizeDelta.y = rectTransform.sizeDelta.y;
        anchoredPosition.y = rectTransform.sizeDelta.y;

        list.sizeDelta = sizeDelta;
        list.anchoredPosition = anchoredPosition;
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
        Close();
    }

    public void ActivateElement(int index)
    {
        SetElementActive(index);
        indexText.text = index.ToString();
    }

    protected abstract void SetElementActive(int index);
}
