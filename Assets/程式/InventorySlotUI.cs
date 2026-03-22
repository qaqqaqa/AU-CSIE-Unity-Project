using TMPro;
using UnityEngine;
using UnityEngine.EventSystems; // 可以保留或移除，取決於是否還需要 IPointerClickHandler
using UnityEngine.UI;

// 【修改】移除 IPointerEnterHandler, IPointerExitHandler
public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    public Image itemIcon;
    public TextMeshProUGUI itemQuantityText;
    public Button slotButton; // 如果您還需要點擊功能，保留 Button

    // --- 【新增】對格子內部介紹文字的引用 ---
    public TextMeshProUGUI descriptionText;
    // --- 新增結束 ---

    private ItemData currentItemData;
    private int currentQuantity;
    private int slotIndex;

    public delegate void OnSlotClicked(int index);
    public static event OnSlotClicked onSlotClicked;

    void Awake()
    {
        // 如果 descriptionText 沒有在 Inspector 中指定，嘗試自動尋找
        if (descriptionText == null)
        {
            descriptionText = GetComponentInChildren<TextMeshProUGUI>();
            // 注意：如果您的 Prefab 中有多個 TextMeshProUGUI，這裡可能找到錯誤的元件。
            // 最好還是在 Inspector 中手動指定。
            Debug.LogWarning($"格子 {gameObject.name} 未在 Inspector 中指定 DescriptionText，已嘗試自動尋找。");
        }

        // 確保介紹文字一開始是隱藏的
        if (descriptionText != null) descriptionText.gameObject.SetActive(false);
    }

    // 初始化格子
    public void InitSlot(ItemData data, int quantity, int slotIndex)
    {
        currentItemData = data;
        currentQuantity = quantity;
        this.slotIndex = slotIndex;

        if (data != null)
        {
            // 更新圖示和數量 (保持不變)
            itemIcon.sprite = data.itemIcon;
            itemIcon.enabled = true;
            itemQuantityText.enabled = quantity > 1;
            itemQuantityText.text = quantity > 1 ? quantity.ToString() : "";

            // --- 【新增】更新介紹文字 ---
            if (descriptionText != null)
            {
                descriptionText.text = data.itemDescription; // 設定文字內容
                descriptionText.gameObject.SetActive(true); // 顯示文字
            }
            // --- 新增結束 ---
        }
        else
        {
            ClearSlot(); // 如果傳入的 data 是 null，直接清空格子
        }
    }

    // 清空格子
    public void ClearSlot()
    {
        currentItemData = null;
        itemIcon.sprite = null;
        itemIcon.enabled = false;
        itemQuantityText.text = "";
        itemQuantityText.enabled = false;

        // --- 【新增】隱藏介紹文字 ---
        if (descriptionText != null)
        {
            descriptionText.text = ""; // 清空文字
            descriptionText.gameObject.SetActive(false); // 隱藏文字
        }
        // --- 新增結束 ---
    }

    // 處理滑鼠點擊 (保持不變)
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && currentItemData != null)
        {
            onSlotClicked?.Invoke(slotIndex);
        }
    }

    // --- 【移除】滑鼠移入/移出方法 ---
    // public void OnPointerEnter(PointerEventData eventData) { ... }
    // public void OnPointerExit(PointerEventData eventData) { ... }
    // --- 移除結束 ---


    public ItemData GetItemData() { return currentItemData; }
    public int GetQuantity() { return currentQuantity; }
}
