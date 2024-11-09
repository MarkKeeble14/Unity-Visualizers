using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

public class ComputerCursor : MonoBehaviour
{
    [SerializeField] private RectTransform cursor;
    public RectTransform CursorRect => cursor;
    [SerializeField] private RectTransform canvasRect;
    private float xDist;
    private float yDist;
    public static ComputerCursor _Instance { get; private set; }

    private List<ComputerScreenControl> interactableElements = new();

    [SerializeField] private Image[] cursorImages;
    [SerializeField] private Sprite hoveredSprite;
    [SerializeField] private Sprite notHoveredSprite;
    [SerializeField] private Sprite heldDownSprite;

    [SerializeField] private CallPlayOneShotContainer clickDown;
    [SerializeField] private CallPlayOneShotContainer clickRelease;
    Vector3[] elementCorners = new Vector3[4];

    private bool active = true;
    private bool clicked;
    private bool isMouseDown;

    private List<ComputerScreenControl> cursorOverElements = new();
    private ComputerScreenControl highestPrioControl;

    public void Enable()
    {
        active = true;
    }

    public void Disable()
    {
        active = false;
    }

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
        if (!active)
        {
            isMouseDown = false;
            return;
        }

        cursorOverElements.Clear();
        clicked = false;

        if (Input.GetMouseButtonDown(0))
        {
            clicked = true;
        }

        // check for hovers
        for (int i = 0; i < interactableElements.Count; i++)
        {
            ComputerScreenControl element = interactableElements[i];

            if (element.IsHeld)
            {
                element.Held();

                if (Input.GetMouseButtonUp(0))
                {
                    element.Released();
                }
            }

            if (element.Disabled)
            {
                element.NotHovered();
                continue;
            }

            element.Rect.GetWorldCorners(elementCorners);
            if (element.transform.GetChild(0).gameObject.activeInHierarchy &&
                cursor.position.x > elementCorners[0].x && cursor.position.x < elementCorners[2].x
                && cursor.position.y > elementCorners[3].y && cursor.position.y < elementCorners[1].y)
            {
                if (!isMouseDown)
                {
                    cursorOverElements.Add(element);
                }
            }
            else
            {
                element.NotHovered();
            }
        }

        if (cursorOverElements.Count > 0)
        {
            highestPrioControl = cursorOverElements[0];

            if (cursorOverElements.Count > 1)
            {
                for (int i = 1; i < cursorOverElements.Count; i++)
                {
                    ComputerScreenControl c = cursorOverElements[i];
                    if (c.Priority > highestPrioControl.Priority)
                    {
                        highestPrioControl = c;
                    }
                }
            }

            ComputerScreenControl[] activateControls = highestPrioControl.gameObject.GetComponents<ComputerScreenControl>();
            foreach (ComputerScreenControl c in activateControls)
            {
                c.Hovered();
                if (clicked)
                {
                    c.Clicked();
                }
            }
            foreach (ComputerScreenControl c in cursorOverElements)
            {
                if (c.gameObject != highestPrioControl.gameObject)
                {
                    c.NotHovered();
                }
            }
        } else
        {
            highestPrioControl = null;
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

        if (Input.GetMouseButtonDown(0))
        {
            clickDown.PlayOneShot();
            isMouseDown = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            clickRelease.PlayOneShot();
            isMouseDown = false;
        }
    }
}
