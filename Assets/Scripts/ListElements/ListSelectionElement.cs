using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutElement))]
public abstract class ListSelectionElement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI indexText;
    [SerializeField] private Image lockButtonImage;
    [SerializeField] private Sprite lockIcon;
    [SerializeField] private Sprite unlockIcon;
    [SerializeField] private GameObject randomizeButton;

    private int index;
    public int Index => index;
    private bool locked;
    public bool IsLocked => locked;

    public abstract void Open();
    public abstract void Delete();

    public void SetIndex(int index)
    {
        this.index = index;
        indexText.text = index.ToString();
    }

    public void TryRandomize()
    {
        if (locked) return;
        Randomize(false);
    }

    public void ForceRandomize()
    {
        if (locked) return;
        Randomize(true);
    }

    protected abstract void Randomize(bool skipConfirmation);

    public void ToggleLocked()
    {
        SetLocked(!locked);
    }

    public void SetLocked(bool b)
    {
        locked = b;
        lockButtonImage.sprite = b ? unlockIcon: lockIcon;

        randomizeButton.SetActive(!locked);
    }
}
