using UnityEngine;

public abstract class Checkbox : MonoBehaviour
{
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
        UpdateUI();
    }

    protected abstract void IsActive();
    protected abstract void IsInactive();

    private void UpdateUI()
    {
        if (active) IsActive(); else IsInactive();
    }
}
