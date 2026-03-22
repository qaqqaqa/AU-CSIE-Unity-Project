using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMoveWithInteraction : MonoBehaviour
{
    Rigidbody rb;
    float horizontalInput;
    float verticalInput;

    [Header("Movement Settings")]
    public float speed = 10f;
    public float jumpSpeed = 8f;
    public float groundDrag = 5f;

    [Header("Ground Check Settings")]
    public LayerMask groundMask;
    public float groundCheckDistance = 0.6f;
    public float groundCheckOffsetY = -0.5f;
    bool isGrounded;

    [Header("Interaction Settings")]
    public LayerMask interactableMask;
    public float interactionDistance = 3f;
    public KeyCode interactKey = KeyCode.Mouse0; // 互動/拾取/放下 按鈕 (左鍵)
    public KeyCode useItemKey = KeyCode.E;      // 使用物品 按鈕 (E鍵)
    public Transform handHoldPoint;             // *** 將手持位置的空物件拖曳到這裡 ***
    private FireExtinguisher heldExtinguisher = null; // 當前持有的滅火器引用
    // private Transform currentInteractable = null; // 我們將改為檢查組件類型

    [Header("Camera Settings")]
    public Camera mainCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null) Debug.LogError("PlayerMoveWithInteraction: 找不到主攝影機！", this);
        if (handHoldPoint == null) Debug.LogError("PlayerMoveWithInteraction: 手持位置（Hand Hold Point）未指定！", this);
    }

    void Update()
    {
        // --- 獲取輸入 ---
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // --- 地面檢測 ---
        CheckGrounded();

        // --- 跳躍輸入 ---
        HandleJumpInput();

        // --- 互動/拾取/放下 檢測 ---
        HandleInteraction(); // 為了清晰起見重新命名

        // --- 使用持有物品的邏輯 ---
        HandleItemUsage();

        // --- 設定阻力 ---
        rb.linearDamping = isGrounded ? groundDrag : 0.1f;
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void CheckGrounded()
    {
        Vector3 rayStart = transform.position + Vector3.up * groundCheckOffsetY;
        isGrounded = Physics.Raycast(rayStart, Vector3.down, groundCheckDistance, groundMask);
        Debug.DrawRay(rayStart, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
    }

    void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        }
    }

    void HandleInteraction()
    {
        if (mainCamera == null) return;

        // 放下持有物品
        if (heldExtinguisher != null && Input.GetKeyDown(interactKey))
        {
            DropExtinguisher();
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.yellow);

            // 優先處理滅火器
            if (heldExtinguisher == null)
            {
                FireExtinguisher potentialPickup = hit.transform.GetComponent<FireExtinguisher>();
                if (potentialPickup != null)
                {
                    if (Input.GetKeyDown(interactKey))
                    {
                        PickupExtinguisher(potentialPickup);
                        return;
                    }
                }
            }

            // **這裡新增檢查門的互動**
            OpenDown door = hit.transform.GetComponent<OpenDown>();
            if (door != null)
            {
                if (Input.GetKeyDown(interactKey))
                {
                    door.Switch();
                    return;
                }
            }
        }
    }


    void PickupExtinguisher(FireExtinguisher extinguisherToPickup)
    {
        if (handHoldPoint == null)
        {
            Debug.LogError("無法拾取：手持位置未設定！");
            return;
        }

        heldExtinguisher = extinguisherToPickup;
        heldExtinguisher.OnPickup();

        // 關掉物理碰撞
        Rigidbody extinguisherRb = heldExtinguisher.GetComponent<Rigidbody>();
        if (extinguisherRb != null)
        {
            extinguisherRb.isKinematic = true;
            extinguisherRb.detectCollisions = false;
        }

        heldExtinguisher.transform.SetParent(handHoldPoint);
        heldExtinguisher.transform.localPosition = Vector3.zero;

        // 這裡改成你的正確角度，假設要直立拿著（自己可以微調）
        heldExtinguisher.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        Debug.Log($"已拾取：{heldExtinguisher.name}");
    }

    void DropExtinguisher()
    {
        if (heldExtinguisher == null) return;

        Debug.Log($"正在放下：{heldExtinguisher.name}");

        heldExtinguisher.StopFoam();
        heldExtinguisher.OnDrop();

        heldExtinguisher.transform.SetParent(null);

        Rigidbody dropRb = heldExtinguisher.GetComponent<Rigidbody>();
        if (dropRb != null)
        {
            dropRb.isKinematic = false;
            dropRb.detectCollisions = true;

            // 在往前丟一點力量
            dropRb.AddForce(mainCamera.transform.forward * 2f + Vector3.up * 1f, ForceMode.Impulse);

            // 加一點隨機旋轉
            dropRb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse);
        }

        heldExtinguisher = null;
    }


    void HandleItemUsage()
    {
        // 只有在持有滅火器時才檢查使用按鍵
        if (heldExtinguisher != null)
        {
            // 按下 E 鍵
            if (Input.GetKey(useItemKey))
            {
                heldExtinguisher.StartFoam();
            }
            // 放開 E 鍵
            else //或者用 Input.GetKeyUp(useItemKey) 也可以
            {
                heldExtinguisher.StopFoam();
            }
        }
    }


    void MovePlayer()
    {
        Vector3 moveDirection = (transform.right * horizontalInput + transform.forward * verticalInput).normalized;

        if (isGrounded)
        {
            rb.AddForce(moveDirection * speed * 10f, ForceMode.Force);
        }
        // else { // 可選的空中控制
        //    rb.AddForce(moveDirection * speed * 10f * airMultiplier, ForceMode.Force);
        // }
    }
}