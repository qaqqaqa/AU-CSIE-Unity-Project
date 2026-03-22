using UnityEngine;

public class Camera2 : MonoBehaviour
{
    public float sensitivity = 2f;
    float rotationX = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // 垂直旋轉
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -60f, 60f);
        transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        // 水平旋轉角色
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
