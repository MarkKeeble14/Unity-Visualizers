using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GradientKeyDisplay : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [SerializeField] private Color color;
    public Color Color => color;
    [SerializeField] private Image backgroundImage;
    private float time;
    [SerializeField] private Image keyImage;

    private int transformIndex;
    private int keyIndex;
    private float halfSize;

    public bool Active => backgroundImage.enabled;

    private void Awake()
    {
        RectTransform r = transform as RectTransform;
        halfSize = r.sizeDelta.x / 2;
    }

    public void Set(int setAtChildIndex, int assignedKeyIndex, Color c, float time)
    {
        this.time = time;
        transformIndex = setAtChildIndex;
        keyIndex = assignedKeyIndex;
        color = c;
        keyImage.color = color;
    }

    public void SetVisible(bool visible)
    {
        keyImage.gameObject.SetActive(visible);
        backgroundImage.enabled = visible;
    }

    public void SelectKey()
    {
        GradientEditor._Instance.SetEditingKey(keyIndex, color, time);
    }

    private int GetIndexToMoveTo(Vector3 intendedPos)
    {
        if (intendedPos.x < transform.parent.GetChild(0).position.x) return 0;
        for (int i = 0; i < transform.parent.childCount - 1; i++)
        {
            Transform sibling1 = transform.parent.GetChild(i);
            Transform sibling2 = transform.parent.GetChild(i + 1);
            if (sibling1.position.x <= intendedPos.x && intendedPos.x <= sibling2.position.x)
            {
                return i;
            }
        }
        return transform.parent.childCount;
    }

    public void OnDrag(PointerEventData eventData)
    {
        bool shouldMoveRight = (Input.mousePosition.x + halfSize) > transform.position.x;
        bool shouldMoveLeft = (Input.mousePosition.x - halfSize) < transform.position.x;
        int index = GetIndexToMoveTo(Input.mousePosition);

        if (shouldMoveRight)
        {
            if (index > transform.parent.childCount)
            {
                transform.SetSiblingIndex(transform.parent.childCount);
            }
            else
            {
                transform.SetSiblingIndex(index);
            }
        }
        else if (shouldMoveLeft)
        {
            if (index < 0)
            {
                transform.SetSiblingIndex(0);
            }
            else
            {
                transform.SetSiblingIndex(index);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float newTime;
        if (transform.GetSiblingIndex() == transform.parent.childCount - 1)
            newTime = 1;
        else
            newTime = (float)transform.GetSiblingIndex() / transform.parent.childCount;
        GradientEditor._Instance.ReadKeysFromChildIndices();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        SelectKey();
    }
}
