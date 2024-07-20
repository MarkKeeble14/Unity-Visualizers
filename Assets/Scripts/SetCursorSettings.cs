using UnityEngine;

public class SetCursorSettings : MonoBehaviour
{
    [SerializeField] private bool visible;
    private void Update()
    {
        Cursor.visible = visible;
    }
}
