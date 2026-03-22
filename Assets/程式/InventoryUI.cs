using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; // 雖然移除了 descriptionText，但其他地方可能用到 TMP

public class InventoryUI : MonoBehaviour
{
    // --- Singleton 模式 (保留，PlayerController 可能需要) ---
    public static InventoryUI Instance { get; private set; }
    // --- Singleton 結束 ---

    public GameObject inventoryPanel;
    public GameObject inventorySlotPrefab;
    public Transform slotContainer;
    public Button closeButton;
    // --- 【已移除】介紹文字變數 ---
    // public TextMeshProUGUI descriptionText;

    private List<InventorySlotUI> inventorySlots = new List<InventorySlotUI>();

    public delegate void OnInventoryUIStateChanged(bool isOpen);
    public static event OnInventoryUIStateChanged onInventoryUIStateChanged;

    private bool isInventoryOpen = false;

    void Awake()
    {
        // --- Singleton 初始化 (保留) ---
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("發現重複的 InventoryUI 實例，已將其摧毀。");
            Destroy(gameObject);
            return;
        }
        // --- Singleton 結束 ---

        inventoryPanel.SetActive(false);
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }

        // --- 【已移除】介紹文字相關的 Awake 邏輯 ---
    }

    void Start()
    {
        // 訂閱事件 (保持不變)
        if (InventoryManager.Instance != null)
        {
            InventoryManager.onInventoryChanged += UpdateInventoryUI;
        }
        else
        {
            Debug.LogError("InventoryUI: 找不到 InventoryManager 的實例！");
        }
        InventorySlotUI.onSlotClicked += OnInventorySlotClicked;
    }

    void OnDestroy()
    {
        // 取消訂閱事件 (保持不變)
        if (InventoryManager.Instance != null)
        {
            InventoryManager.onInventoryChanged -= UpdateInventoryUI;
        }
        InventorySlotUI.onSlotClicked -= OnInventorySlotClicked;
    }

    void Update()
    {
        // 遊戲結束檢查 (保持不變)
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded())
        {
            return;
        }

        // 防呆邏輯 (保持不變)
        if (!isInventoryOpen && Time.timeScale == 0f)
        {
            Debug.LogWarning("UI 已關閉但時間尚未恢復，自動修正！");
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void Show()
    {
        inventoryPanel.SetActive(true);
        isInventoryOpen = true;
        onInventoryUIStateChanged?.Invoke(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UpdateInventoryUI(); // 打開時更新一次 UI
    }

    public void Hide()
    {
        inventoryPanel.SetActive(false);
        isInventoryOpen = false;
        onInventoryUIStateChanged?.Invoke(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        // --- 【已移除】關閉背包時隱藏介紹的呼叫 ---
        // HideDescription();
    }

    // --- 【已移除】顯示介紹的方法 ---
    // public void ShowDescription(string description, Vector3 slotPosition) { ... }

    // --- 【已移除】隱藏介紹的方法 ---
    // public void HideDescription() { ... }


    public void SetupInventorySlots(int numberOfSlots)
    {
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }
        inventorySlots.Clear();

        for (int i = 0; i < numberOfSlots; i++)
        {
            GameObject slotGO = Instantiate(inventorySlotPrefab, slotContainer);
            InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();
            if (slotUI != null)
            {
                slotUI.InitSlot(null, 0, i);
                inventorySlots.Add(slotUI);
            }
        }
    }

    public void UpdateInventoryUI()
    {
        if (InventoryManager.Instance == null) return;

        List<InventoryManager.InventoryItem> items = InventoryManager.Instance.GetInventoryItems();

        if (inventorySlots.Count < items.Count)
        {
            SetupInventorySlots(items.Count);
        }

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (i < items.Count)
            {
                inventorySlots[i].InitSlot(items[i].itemData, items[i].quantity, i);
            }
            else
            {
                inventorySlots[i].ClearSlot();
            }
        }
    }

    private void OnInventorySlotClicked(int index)
    {
        if (InventoryManager.Instance == null) return;

        Debug.Log($"玩家點擊了背包索引 {index} 的格子。");
        InventoryManager.Instance.TryUseOrRemoveItem(index);
        Hide();
    }
}

