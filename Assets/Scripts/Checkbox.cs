using UnityEngine;

public abstract class Checkbox : MonoBehaviour
{
    public bool Active { get; private set; }

    private void Awake()
    {
        UpdateUI();
    }

    public void OnClick()
    {
        Active = !Active;
        UpdateUI();
    }

    protected abstract void UpdateUI();
}
