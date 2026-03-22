using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  修改點：移除 UnityEditor.UIElements 與 Unity.Mathematics，這些在 Built-in RP 不需要

public class PlayerCamera : MonoBehaviour
{
    public float sensitivity; //  修改點：給預設值避免忘記設定

    float xRotation;
    float yRotation;

    public Transform player;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity * Time.deltaTime;

        xRotation += mouseX;
        yRotation -= mouseY;
        yRotation = Mathf.Clamp(yRotation, -90f, 90f);

        // 修改點：明確使用 Quaternion.Euler，而不是 float3 或 DOTS API
        transform.rotation = Quaternion.Euler(yRotation, xRotation, 0);
        player.rotation = Quaternion.Euler(0, xRotation, 0);
    }
}
