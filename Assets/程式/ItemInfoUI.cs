// ItemInfoUI.cs
using UnityEngine;
using UnityEngine.UI; // 為了 Button 和 Image
using TMPro; // 為了 TextMeshProUGUI

public class ItemInfoUI : MonoBehaviour
{
    public GameObject infoPanel; // 道具資訊面板的根 GameObject
    public Image itemIcon; // 道具圖示
    public TextMeshProUGUI itemNameText; // 道具名稱
    public TextMeshProUGUI itemDescriptionText; // 道具描述
    public TextMeshProUGUI usageHintText; // 使用提示

    public Button closeButton; // 關閉按鈕

    void Awake()
    {
        // 確保一開始資訊面板是隱藏的
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }

        // 為關閉按鈕添加事件監聽器
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HideInfoPanel);
        }
    }

    // 顯示道具資訊面板
    public void ShowInfoPanel(ItemData data)
    {
        if (infoPanel != null && data != null)
        {
            infoPanel.SetActive(true);

            // 更新 UI 顯示
            if (itemIcon != null) itemIcon.sprite = data.itemIcon;
            if (itemNameText != null) itemNameText.text = data.itemName;
            if (itemDescriptionText != null) itemDescriptionText.text = data.itemDescription;
            if (usageHintText != null) usageHintText.text = data.usageHint;

            // 顯示面板時，解鎖鼠標以便操作 UI
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // 隱藏道具資訊面板
    public void HideInfoPanel()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
        // 隱藏面板後，重新鎖定鼠標 (PlayerController 會處理)
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    // 在 Inspector 中驗證引用是否完整
    void OnValidate()
    {
        if (infoPanel == null) Debug.LogWarning("ItemInfoUI: Info Panel 未指定。", this);
        if (itemIcon == null) Debug.LogWarning("ItemInfoUI: Item Icon 未指定。", this);
        if (itemNameText == null) Debug.LogWarning("ItemInfoUI: Item Name Text 未指定。", this);
        if (itemDescriptionText == null) Debug.LogWarning("ItemInfoUI: Item Description Text 未指定。", this);
        if (usageHintText == null) Debug.LogWarning("ItemInfoUI: Usage Hint Text 未指定。", this);
        if (closeButton == null) Debug.LogWarning("ItemInfoUI: Close Button 未指定。", this);
    }
}
