using UnityEngine;

public class CursorSetter : MonoBehaviour
{
    [SerializeField] Texture2D cursorTex;
    [SerializeField] bool centerHotspot = true;
    [SerializeField] Vector2 customHotspot;

    void OnEnable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined; 

        Vector2 hotspot = centerHotspot && cursorTex
            ? new Vector2(cursorTex.width * 0.5f, cursorTex.height * 0.5f)
            : customHotspot;

        Cursor.SetCursor(cursorTex, hotspot, CursorMode.Auto); 
    }

    void OnDisable()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Cursor.lockState = CursorLockMode.None;
    }
}
