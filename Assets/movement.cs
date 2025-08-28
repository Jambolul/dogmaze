using UnityEngine;

public class DogFollowMouse : MonoBehaviour
{
    public float smoothing = 20f;

    void OnDisable() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }


    void Update()
    {
        var m = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        m.z = transform.position.z;
        float t = 1f - Mathf.Exp(-smoothing * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, m, t);
    }
}
