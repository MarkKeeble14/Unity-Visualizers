using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

public class ComputerCursor : MonoBehaviour
{
    [SerializeField] private RectTransform cursor;
    [SerializeField] private RectTransform canvasRect;
    private float xDist;
    private float yDist;
    public static ComputerCursor _Instance { get; private set; }

    private List<ComputerScreenControl> interactableElements = new();

    [SerializeField] private Image[] cursorImages;
    [SerializeField] private Sprite hoveredSprite;
    [SerializeField] private Sprite notHoveredSprite;
    [SerializeField] private Sprite heldDownSprite;

    public void AddInteractable(ComputerScreenControl i)
    {
        interactableElements.Add(i);
    }

    public void RemoveInteractable(ComputerScreenControl i)
    {
        interactableElements.Remove(i);
    }

    public void AddInteractables(List<ComputerScreenControl> l)
    {
        interactableElements.AddRange(l);
    }

    public void RemoveInteractables(List<ComputerScreenControl> l)
    {
        for (int i = 0; i < l.Count; i++)
        {
            interactableElements.Remove(l[i]);
        }
    }

    public void AddInteractables(ComputerScreenControl[] l)
    {
        interactableElements.AddRange(l);
    }

    public void RemoveInteractables(ComputerScreenControl[] l)
    {
        for (int i = 0; i < l.Length; i++)
        {
            interactableElements.Remove(l[i]);
        }
    }

    private void Awake()
    {
        if (_Instance != null) Destroy(_Instance.gameObject);
        _Instance = this;
    }

    private void Update()
    {
        // check for hovers
        for (int i = 0; i < interactableElements.Count; i++)
        {
            ComputerScreenControl element = interactableElements[i];
            xDist = Mathf.Abs(element.Rect.position.x - cursor.position.x);
            yDist = Mathf.Abs(element.Rect.position.y - cursor.position.y);

            // Debug.DrawLine(element.Rect.position, cursor.position, Color.blue);

            if (xDist < element.Rect.sizeDelta.x / 2 && yDist < element.Rect.sizeDelta.y / 2)
            {
                element.Hovered();
                if (Input.GetMouseButtonDown(0))
                {
                    element.Clicked();
                }
            }
            else
            {
                element.NotHovered();
            }
        }

        // update cursor
        foreach (Image i in cursorImages)
        {
            if (Input.GetMouseButton(0))
            {
                i.sprite = heldDownSprite;
            } else
            {
                i.sprite = (ComputerScreenControl.NumHovered > 0 ? hoveredSprite : notHoveredSprite);
            }
        }
    }
}
