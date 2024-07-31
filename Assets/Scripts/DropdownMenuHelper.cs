using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropdownMenuHelper : MonoBehaviour
{
    [SerializeField] private RectTransform content;
    private DropdownMenu menu;
    public static bool IsScrolling => Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > scrollDetectionSensitivity;
    private static float scrollDetectionSensitivity = 0f;

    private void Awake()
    {
        menu = GetComponent<DropdownMenu>();
    }

    private void Update()
    {
        if (menu.IsOpen && !UIHelper.IsPointerOverSpecificUIElement(content.gameObject) && IsScrolling)
        {
            menu.Close();
        }
    }
}
