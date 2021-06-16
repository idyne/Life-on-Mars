using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    private Camera cam;
    private RectTransform rect;

    private void Awake()
    {
        cam = Camera.main;
        rect = GetComponent<RectTransform>();
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 anchor = new Vector2(mousePosition.x / Screen.width, mousePosition.y / Screen.height);
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.anchoredPosition = Vector2.zero;
        rect.SetAsLastSibling();
    }
}
