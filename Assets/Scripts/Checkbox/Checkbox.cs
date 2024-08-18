using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public abstract class Checkbox : MonoBehaviour
{
    [SerializeField] private UnityEvent<bool> onClickEvent;

    private bool active;
    public bool Active
    {
        get 
        {
            return active;
        } 
        set
        {
            active = value;
            UpdateUI();
        }
    }

    private void Awake()
    {
        UpdateUI();
    }

    public void OnClick()
    {
        active = !active;
        onClickEvent?.Invoke(active);
        UpdateUI();
    }

    protected abstract void IsActive();
    protected abstract void IsInactive();

    private void UpdateUI()
    {
        if (active) IsActive(); else IsInactive();
    }
}
