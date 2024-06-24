using UnityEngine;

public abstract class KeyControl : MonoBehaviour
{
    [SerializeField] protected KeyCode key;
    [SerializeField] private string action;

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
