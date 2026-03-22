using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class playermove : MonoBehaviour
{
    Rigidbody rb;
    float horizontalInput;
    float verticalInput;

    public float speed ;               //  預設移動速度
    public float jumpSpeed ;           //  預設跳躍力
    public float groundDrag ;          //  預設地面阻力

    bool isGrounded;
    public LayerMask groundMask;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;          //  防止剛體傾倒
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");   //  修改點：修正大小寫錯誤

        //  修改點：使用 Raycast 偵測是否貼地（建議角色的中心點正確）
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.2f, groundMask);

        if (isGrounded)
            rb.linearDamping = groundDrag;          // 🔧 修改點：正確使用 drag
        else
            rb.linearDamping = 0;

        //  修改點：只有落地才能跳
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        //  修改點：標準化移動方向並施加力量
        Vector3 direction = transform.right * horizontalInput + transform.forward * verticalInput;
        rb.AddForce(direction.normalized * speed, ForceMode.Force);
    }
}
