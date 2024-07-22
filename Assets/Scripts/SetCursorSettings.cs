using UnityEngine;

public class SetCursorSettings : MonoBehaviour
{
    [SerializeField] private bool visible;

    private void Start()
    {
        Cursor.visible = visible;
    }
}
