using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement & Look (移動與視角)")]
    [Tooltip("施加力量的大小 (建議 50-100)")]
    public float moveSpeed = 20f;
    [Tooltip("限制最大移動速度 (建議 5-8)")]
    public float maxVelocity = 4f;
    public float lookSensitivity = 2f;
    public Camera playerCamera;

    [Header("Jumping (跳躍)")]
    public float jumpForce = 8f;
    public LayerMask groundMask;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    [Header("Interaction (互動)")]
    public float interactionDistance = 3f;
    public LayerMask interactableMask;

    [Header("Item Holding (手持物品)")]
    public Transform itemHoldPoint;
    private GameObject currentHoldingItemModel;
    private ItemData currentHoldingItemData;

    [Header("Dropped Item Spawn (丟棄物品生成點)")]
    public Transform droppedItemSpawnPoint;

    [Header("Input Keys (按鍵設定)")]
    public KeyCode pickUpToInventoryKey = KeyCode.U;
    public KeyCode useOrEquipKey = KeyCode.E;
    public KeyCode dropItemFromHandKey = KeyCode.G;
    public KeyCode inventoryKey = KeyCode.P;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.C;

    [Header("UI")]
    public InventoryUI inventoryUI;

    [Header("Crouching (蹲下設定)")]
    public float crouchSpeedMultiplier = 0.5f; // 蹲下時速度變為原本的幾倍
    public float crouchHeight = 1f;            // 蹲下時碰撞體高度
    public float standingHeight = 2f;          // 站立時碰撞體高度

    [Tooltip("相機站立時的 Y 軸高度")]
    public float cameraStandHeight = 1.6f;
    [Tooltip("相機蹲下時的 Y 軸高度")]
    public float cameraCrouchHeight = 0.8f;
    [Tooltip("蹲下/站立動作的平滑速度")]
    public float crouchTransitionSpeed = 10f;

    private bool isCrouching = false;

    // 內部變數
    private Rigidbody rb;
    private float rotationX = 0;
    private bool isGrounded;
    private bool isInventoryOpen = false;
    private Animator animator;
    private Vector2 moveInput;
    private CapsuleCollider playerCollider;
    private float currentMoveSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        playerCollider = GetComponent<CapsuleCollider>();

        // 錯誤檢查
        if (rb == null) Debug.LogError("PlayerController: Rigidbody not found!", this);
        if (playerCollider == null) Debug.LogError("PlayerController: CapsuleCollider not found!", this);
        if (playerCamera == null) Debug.LogError("PlayerController: playerCamera is not assigned!", this);
        if (itemHoldPoint == null) Debug.LogError("PlayerController: itemHoldPoint is not assigned!", this);
        if (groundCheck == null) Debug.LogError("PlayerController: groundCheck is not assigned!", this);
        if (droppedItemSpawnPoint == null) Debug.LogError("PlayerController: droppedItemSpawnPoint is not assigned!", this);

        // 初始化游標狀態
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 註冊事件
        InventoryManager.onItemTakenFromInventory += SpawnItemFromInventory;
        InventoryUI.onInventoryUIStateChanged += HandleInventoryUIState;

        // 初始化速度與相機高度
        currentMoveSpeed = moveSpeed;
        if (playerCamera != null)
        {
            // 嘗試自動抓取當前相機高度作為站立高度 (可選)
            // cameraStandHeight = playerCamera.transform.localPosition.y;
        }
    }

    void OnDestroy()
    {
        // 取消註冊事件
        InventoryManager.onItemTakenFromInventory -= SpawnItemFromInventory;
        InventoryUI.onInventoryUIStateChanged -= HandleInventoryUIState;
    }

    // Update 處理輸入、視角、動畫等非物理邏輯
    void Update()
    {
        // 遊戲結束檢查
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded())
        {
            return;
        }

        // 背包開關
        if (Input.GetKeyDown(inventoryKey))
        {
            ToggleInventory();
        }

        // 如果背包開啟，暫停操作
        if (isInventoryOpen) return;

        // 地面檢測
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

        // 執行各項功能
        HandleLook();            // 視角旋轉
        HandleCrouch();          // 蹲下狀態切換
        HandleCameraHeight();    // 相機高度平滑移動
        HandleJump();            // 跳躍
        UpdateAnimation();       // 動畫更新
        HandleInteractionAndUse(); // 物品互動與使用
    }

    // FixedUpdate 處理物理移動 (AddForce)
    void FixedUpdate()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsGameEnded())
        {
            if (!isInventoryOpen)
            {
                HandleMovement();
            }
        }
    }

    // --- 功能實作 ---

    void HandleInventoryUIState(bool isOpen)
    {
        isInventoryOpen = isOpen;
        if (!isOpen)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        if (isInventoryOpen)
        {
            inventoryUI.Show();
            if (currentHoldingItemModel != null) currentHoldingItemModel.SetActive(false);
        }
        else
        {
            inventoryUI.Hide();
            if (currentHoldingItemModel != null) currentHoldingItemModel.SetActive(true);
        }
        Cursor.lockState = isInventoryOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isInventoryOpen;
        if (animator != null) animator.SetBool("isWalking", false);
    }

    void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;
        transform.Rotate(Vector3.up * mouseX);
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
    }

    void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        moveInput = new Vector2(moveHorizontal, moveVertical); // 用於動畫

        // 計算移動方向
        Vector3 moveDirection = transform.right * moveHorizontal + transform.forward * moveVertical;
        moveDirection.Normalize(); // 防止斜向移動過快

        // 使用 AddForce 移動 (VelocityChange 模式反應較快)
        // Time.fixedDeltaTime 已經內含在物理引擎中，VelocityChange 模式下通常不需要乘，
        // 但如果您習慣以前的數值手感，可以自行調整 moveSpeed
        rb.AddForce(moveDirection * currentMoveSpeed, ForceMode.VelocityChange);

        // 限制最大水平速度
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);
        if (horizontalVelocity.magnitude > maxVelocity)
        {
            Vector3 limitedVelocity = horizontalVelocity.normalized * maxVelocity;
            rb.linearVelocity = new Vector3(limitedVelocity.x, currentVelocity.y, limitedVelocity.z);
        }
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(crouchKey))
        {
            isCrouching = !isCrouching;

            // 調整碰撞體高度
            if (playerCollider != null)
            {
                playerCollider.height = isCrouching ? crouchHeight : standingHeight;
                Vector3 center = playerCollider.center;
                center.y = isCrouching ? crouchHeight / 2f : standingHeight / 2f;
                playerCollider.center = center;
            }

            // 調整移動速度
            currentMoveSpeed = isCrouching ? moveSpeed * crouchSpeedMultiplier : moveSpeed;

            // 觸發動畫
            if (animator != null)
            {
                animator.SetBool("isCrouching", isCrouching);
            }
        }
    }

    void HandleCameraHeight()
    {
        if (playerCamera == null) return;

        float targetHeight = isCrouching ? cameraCrouchHeight : cameraStandHeight;
        Vector3 currentPos = playerCamera.transform.localPosition;

        // 平滑過渡相機高度
        float newHeight = Mathf.Lerp(currentPos.y, targetHeight, Time.deltaTime * crouchTransitionSpeed);
        playerCamera.transform.localPosition = new Vector3(currentPos.x, newHeight, currentPos.z);
    }

    void UpdateAnimation()
    {
        if (animator == null) return;
        bool isWalking = moveInput.magnitude > 0.1f && isGrounded;
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isCrouching", isCrouching);
    }

    void HandleInteractionAndUse()
    {
        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        bool hitSomething = Physics.Raycast(ray, out hit, interactionDistance, interactableMask);

        // 繪製除錯射線
        Debug.DrawRay(ray.origin, ray.direction * (hitSomething ? hit.distance : interactionDistance), hitSomething ? Color.green : Color.blue);

        // 撿起物品 (放入背包)
        if (Input.GetKeyDown(pickUpToInventoryKey) && hitSomething)
        {
            InteractableItem item = hit.collider.GetComponent<InteractableItem>();
            if (item != null && InventoryManager.Instance.AddItem(item.itemData))
            {
                item.NotifyPickup();
                Debug.Log($"Picked up {item.itemData.itemName} to inventory.");
            }
        }

        // 使用手持物品 (IUsable)
        if (currentHoldingItemModel != null)
        {
            IUsable usable = currentHoldingItemModel.GetComponent<IUsable>();
            if (usable != null && currentHoldingItemData != null)
            {
                if (Input.GetKeyDown(useOrEquipKey)) usable.Use();
            }
        }
        // 未手持物品時的互動 (開門、直接滅火等)
        else if (Input.GetKeyDown(useOrEquipKey))
        {
            if (hitSomething)
            {
                InteractableItem item = hit.collider.GetComponent<InteractableItem>();
                OpenDown door = hit.collider.GetComponentInParent<OpenDown>();
                AutoIgniteSofa sofa = hit.collider.GetComponent<AutoIgniteSofa>();

                if (item != null)
                {
                    EquipItemToHand(item); // 直接裝備到手上
                }
                else if (door != null)
                {
                    door.Switch(); // 開關門
                }
                else if (sofa != null)
                {
                    Debug.Log("偵測到沙發，嘗試滅火！");
                    sofa.ExtinguishFire();
                }
            }
        }

        // 丟棄手持物品
        if (Input.GetKeyDown(dropItemFromHandKey) && currentHoldingItemModel != null)
        {
            DropItemFromHand();
        }
    }

    private void SpawnItemFromInventory(ItemData itemData)
    {
        if (itemData?.itemModelPrefab == null || droppedItemSpawnPoint == null) return;

        // 生成物品
        GameObject spawned = Instantiate(itemData.itemModelPrefab, droppedItemSpawnPoint.position, droppedItemSpawnPoint.rotation);

        // 確保有 InteractableItem 組件以便再次撿起
        if (spawned.GetComponent<InteractableItem>() == null)
        {
            spawned.AddComponent<InteractableItem>().itemData = itemData;
        }

        // 給予一點推力
        Rigidbody rb = spawned.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(Vector3.up * 2f + playerCamera.transform.forward * 0.5f, ForceMode.Impulse);
        }

        Debug.Log($"Spawned {itemData.itemName} from inventory.");

        // ✅【特殊處理】如果是抹布，確保功能正常
        // 這裡判斷名稱是否包含 "抹布"，請確保您的 ItemData 中名稱設定正確
        if (itemData.itemName.Contains("抹布") || itemData.itemName.Contains("WetCloth") || itemData.itemName.Contains("Rag"))
        {
            WetCloth cloth = spawned.GetComponent<WetCloth>();
            if (cloth == null)
            {
                cloth = spawned.AddComponent<WetCloth>();
            }

            // 自動設定火層 LayerMask (假設 Layer 名稱是 "Fire")
            int fireLayer = LayerMask.NameToLayer("Fire");
            if (fireLayer >= 0)
            {
                cloth.fireLayerMask = 1 << fireLayer;
            }
            else
            {
                Debug.LogWarning("PlayerController: 找不到 'Fire' Layer，抹布可能無法正常滅火！");
            }

            Debug.Log("🧻 已從背包拿出抹布並啟用滅火功能！");
        }
    }

    private void EquipItemToHand(InteractableItem itemToEquip)
    {
        if (currentHoldingItemModel != null) DropItemFromHand();

        currentHoldingItemModel = itemToEquip.gameObject;
        currentHoldingItemData = itemToEquip.itemData;

        currentHoldingItemModel.transform.SetParent(itemHoldPoint);
        currentHoldingItemModel.transform.localPosition = Vector3.zero;
        currentHoldingItemModel.transform.localRotation = Quaternion.identity;

        // 裝備時關閉物理和碰撞
        Collider col = currentHoldingItemModel.GetComponent<Collider>();
        if (col != null) col.enabled = false;
        Rigidbody rb = currentHoldingItemModel.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // 銷毀地上的物品 (因為已經變成手上的子物件)
        // 注意：這裡邏輯稍微修正，直接利用現有物件，不需要 Destroy(itemToEquip) 
        // 除非 itemToEquip 是另一個腳本組件，通常 InteractableItem 是掛在 GameObject 上的
        Destroy(itemToEquip); // 移除 InteractableItem 組件，避免拿在手上還能被撿
    }

    private void DropItemFromHand()
    {
        if (currentHoldingItemModel == null) return;

        // 恢復物理和碰撞
        Collider col = currentHoldingItemModel.GetComponent<Collider>();
        if (col != null) col.enabled = true;
        Rigidbody rb = currentHoldingItemModel.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        currentHoldingItemModel.transform.SetParent(null);
        currentHoldingItemModel.transform.position = playerCamera.transform.position + playerCamera.transform.forward * 0.7f;

        // 重新加入 InteractableItem 組件以便再次撿起
        if (currentHoldingItemModel.GetComponent<InteractableItem>() == null)
        {
            currentHoldingItemModel.AddComponent<InteractableItem>().itemData = currentHoldingItemData;
        }

        currentHoldingItemModel = null;
        currentHoldingItemData = null;
    }
}


