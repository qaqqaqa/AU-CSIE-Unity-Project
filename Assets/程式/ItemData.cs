using UnityEngine;

[CreateAssetMenu(fileName = "New ItemData", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("基本資訊")]
    public string itemName = "New Item";
    public Sprite itemIcon = null;
    public GameObject itemModelPrefab; // 對應的預製件 (教學關放舊的，限時關放新的 OneTime 版)

    [Header("道具屬性")]
    public ItemType itemType = ItemType.Generic;
    public bool isStackable = false;
    public int maxStackSize = 1;

    [Header("UI 顯示資訊")]
    [TextArea(3, 10)]
    public string itemDescription = "這是道具的描述。";
    [TextArea(1, 3)]
    public string usageHint = "點擊滑鼠左鍵使用。";

    // --- 【新增】耐久度系統 ---
    [Header("耐久度 / 限時設定")]
    [Tooltip("當前剩餘時間/次數。設為 -1 代表無限使用 (教學關卡模式)。")]
    public float currentDurability = -1f;

    [Tooltip("最大時間/次數 (用於重置)。設為 -1 代表無限。")]
    public float maxDurability = -1f;
    // --- 新增結束 ---

    public enum ItemType
    {
        Generic,
        FireExtinguisher,
        WetCloth,
        SodaPowder,
        Key,
        FirstAidKit
    }
}