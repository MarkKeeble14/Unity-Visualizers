using UnityEngine;

public abstract class KeyControl : MonoBehaviour
{
    [SerializeField] private bool hidden;
    public bool Hidden => hidden;

    [SerializeField] protected KeyCode key;
    [SerializeField] private string action;
    public ControlScheme PartOfScheme;

    public static bool Disable { get; set; }

    public string GetKey()
    {
        return key.ToString();
    }

    public string GetAction()
    {
        return action;
    }

    public string GetKeyAndActionString(string connection = " - ")
    {
        return GetKey() + connection + GetAction();
    }
}
