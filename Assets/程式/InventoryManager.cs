using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // 【新增】必須引用場景管理

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [System.Serializable]
    public class InventoryItem
    {
        public ItemData itemData;
        public int quantity;
        public InventoryItem(ItemData data, int qty)
        {
            itemData = data;
            quantity = qty;
        }
    }

    [SerializeField] private List<InventoryItem> inventoryItems = new List<InventoryItem>();
    public int maxSlots = 10;

    public delegate void OnInventoryChanged();
    public static event OnInventoryChanged onInventoryChanged;

    public delegate void OnItemTakenFromInventory(ItemData itemData);
    public static event OnItemTakenFromInventory onItemTakenFromInventory;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // 第一次啟動時確保列表存在
        if (inventoryItems == null) inventoryItems = new List<InventoryItem>();
    }

    // --- 【核心修改：監聽場景切換事件】 ---
    void OnEnable()
    {
        // 開始監聽：只要場景載入完成，就呼叫 OnSceneLoaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // 停止監聽（避免記憶體洩漏）
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 當任何場景載入完成時，這個方法會自動被執行
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 強制清空背包
        if (inventoryItems != null)
        {
            inventoryItems.Clear();

            // 通知 UI 更新 (把畫面上的格子也清空)
            onInventoryChanged?.Invoke();

            Debug.Log($"場景 '{scene.name}' 載入完成，背包已自動清空！");
        }
    }
    // --- 修改結束 ---

    public bool AddItem(ItemData itemToAdd, int quantity = 1)
    {
        foreach (var item in inventoryItems)
        {
            if (item.itemData == itemToAdd && item.itemData.isStackable)
            {
                item.quantity += quantity;
                onInventoryChanged?.Invoke();
                Debug.Log($"增加了 {quantity} 個 {itemToAdd.itemName}。現在有 {item.quantity} 個。");
                return true;
            }
        }

        if (inventoryItems.Count < maxSlots)
        {
            inventoryItems.Add(new InventoryItem(itemToAdd, quantity));
            onInventoryChanged?.Invoke();
            Debug.Log($"添加了新道具: {itemToAdd.itemName}。");
            return true;
        }
        else
        {
            Debug.Log("背包已滿，無法添加 " + itemToAdd.itemName);
            return false;
        }
    }

    public void TryUseOrRemoveItem(int index)
    {
        if (index >= 0 && index < inventoryItems.Count)
        {
            InventoryItem item = inventoryItems[index];
            if (item != null && item.itemData != null)
            {
                onItemTakenFromInventory?.Invoke(item.itemData);
                item.quantity--;
                if (item.quantity <= 0)
                {
                    inventoryItems.RemoveAt(index);
                }
                onInventoryChanged?.Invoke();
            }
        }
    }

    public List<InventoryItem> GetInventoryItems()
    {
        return inventoryItems;
    }
}