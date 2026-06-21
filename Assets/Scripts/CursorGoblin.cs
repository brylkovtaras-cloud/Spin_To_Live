using UnityEngine;
using UnityEngine.InputSystem;

public class CursorGoblin : MonoBehaviour
{
    public Texture2D cursorNormal;
    public Texture2D cursorClick;
    public Vector2 hotspot = Vector2.zero;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Cursor.SetCursor(cursorNormal, hotspot, CursorMode.Auto);
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Cursor.SetCursor(cursorClick, hotspot, CursorMode.Auto);
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            Cursor.SetCursor(cursorNormal, hotspot, CursorMode.Auto);
        }
    }
}
