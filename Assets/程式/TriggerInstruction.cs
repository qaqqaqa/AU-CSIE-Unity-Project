using UnityEngine;
using TMPro; // 確保您使用了 TextMeshPro，如果沒有，請將此行改為 using UnityEngine.UI; 並確保 GetComponentInChildren 是 Text 或 Image 等 UI 元件

public class TriggerInstruction : MonoBehaviour
{
    [Header("教學內容")]
    [TextArea(3, 5)]
    public string message;
    public GameObject uiPanel;
    public int requiredStep;
    public bool autoAdvance = false;

    [Header("視覺提示設定")]
    public GameObject selfGlowVisual; // 這個觸發器自己的發光物件
    public GameObject nextGlowVisual; // 下一個要被點亮的發光物件

    // 添加一個私有變數來追蹤玩家是否在觸發器內
    private bool isPlayerInside = false;

    // 在腳本啟動時檢查必要組件
    private void Awake()
    {
        // 檢查觸發器本身是否有 Collider 和 Rigidbody
        Collider myCollider = GetComponent<Collider>();
        if (myCollider == null)
        {
            Debug.LogError($"[TriggerInstruction: {gameObject.name}] 警告：此物件沒有 Collider 組件！觸發器事件將不會觸發。", this);
        }
        else if (!myCollider.isTrigger)
        {
            Debug.LogWarning($"[TriggerInstruction: {gameObject.name}] 警告：此物件的 Collider 沒有勾選 'Is Trigger'！觸發器事件可能無法正確運作。", this);
        }

        // 注意：理論上觸發器本身不需要 Rigidbody，但如果它被設計為移動，則可能需要。
        // 而進入/離開的物件 (Player) 則必須要有 Rigidbody。
    }

    private void OnTriggerEnter(Collider other)
    {
        // 偵錯：印出是哪個物件進入了觸發器
        Debug.Log($"[Trigger Enter] 物件 '{other.name}' 進入了 '{this.gameObject.name}' 的觸發範圍。其標籤為 '{other.tag}'。", this);

        // 檢查進入物件是否是玩家且具有必要的組件
        if (other.CompareTag("Player"))
        {
            Rigidbody playerRb = other.GetComponent<Rigidbody>();
            if (playerRb == null)
            {
                Debug.LogError($"[Trigger Enter] 錯誤：玩家物件 '{other.name}' 沒有 Rigidbody 組件！觸發器事件將不會被完整處理。", other);
                return; // 如果沒有 Rigidbody，直接返回
            }

            // 更新玩家在觸發器內的狀態
            isPlayerInside = true;
            Debug.Log($"[Trigger Enter] 玩家已進入 '{this.gameObject.name}' 觸發器。isPlayerInside = {isPlayerInside}");


            // 偵錯：印出當前的遊戲進度與此觸發器要求的進度
            // 確保 GameManager.Instance 不為 null
            if (GameManager.Instance == null)
            {
                Debug.LogError("[Trigger Enter] 錯誤：GameManager.Instance 為 null！請確保 GameManager 在場景中存在且已正確初始化。", this);
                return;
            }

            Debug.Log($"[Trigger Enter] 玩家進入 '{this.gameObject.name}'。遊戲目前步驟: {GameManager.Instance.currentStep} / 此觸發器要求步驟: {requiredStep}", this);

            if (GameManager.Instance.currentStep == requiredStep)
            {
                Debug.Log($"[Trigger Enter] 步驟正確！在 '{this.gameObject.name}' 執行動作。", this);

                // 1. 顯示UI提示
                if (uiPanel != null)
                {
                    if (!uiPanel.activeSelf) // 避免重複SetActive(true)
                    {
                        uiPanel.SetActive(true);
                        Debug.Log($"[Trigger Enter] 已設定 UI 面板 '{uiPanel.name}' 為啟用。", uiPanel);
                    }

                    TextMeshProUGUI tmpText = uiPanel.GetComponentInChildren<TextMeshProUGUI>();
                    if (tmpText != null)
                    {
                        tmpText.text = message;
                        Debug.Log($"[Trigger Enter] 已更新 UI 提示文字為: '{message}'", tmpText);
                    }
                    else
                    {
                        Debug.LogWarning($"[Trigger Enter] 警告：UI Panel '{uiPanel.name}' 中找不到 TextMeshProUGUI 組件！無法設定文字。", uiPanel);
                    }
                }
                else
                {
                    Debug.LogWarning($"[Trigger Enter] 警告：'{this.gameObject.name}' 的 UI Panel 欄位未設定！UI提示無法顯示。", this);
                }

                // 2. 關閉自己的光
                if (selfGlowVisual != null)
                {
                    if (selfGlowVisual.activeSelf) // 避免重複SetActive(false)
                    {
                        selfGlowVisual.SetActive(false);
                        Debug.Log($"[Trigger Enter] 已關閉 '{selfGlowVisual.name}' 的發光效果。", selfGlowVisual);
                    }
                }
                else
                {
                    Debug.Log($"[Trigger Enter] '{this.gameObject.name}' 沒有設定 selfGlowVisual。", this);
                }

                // 3. 如果設定了自動前進，就進入下一步
                if (autoAdvance)
                {
                    Debug.Log($"[Trigger Enter] '{this.gameObject.name}' 設定為自動前進，準備呼叫 NextStep()。", this);
                    GameManager.Instance.NextStep();
                    Debug.Log($"[Trigger Enter] GameManager.Instance.NextStep() 已被呼叫。當前步驟：{GameManager.Instance.currentStep}", GameManager.Instance);
                }
            }
            else
            {
                Debug.LogWarning($"[Trigger Enter] 步驟不符！'{this.gameObject.name}' 未執行任何動作。當前步驟 ({GameManager.Instance.currentStep}) 與要求步驟 ({requiredStep}) 不符。", this);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 這是最關鍵的偵錯點：確保此方法被呼叫
        Debug.Log($"[DEBUG: OnTriggerExit] 正在被呼叫。觸發器名稱: {this.gameObject.name}, 離開物件: {other.name}, 其標籤為 '{other.tag}'。", this);

        if (other.CompareTag("Player"))
        {
            // 更新玩家在觸發器內的狀態
            isPlayerInside = false;
            Debug.Log($"[Trigger Exit] 玩家已離開 '{this.gameObject.name}' 觸發器。isPlayerInside = {isPlayerInside}");

            // 偵錯：印出玩家離開的資訊
            Debug.Log($"[Trigger Exit] 玩家離開 '{this.gameObject.name}'。遊戲目前步驟: {GameManager.Instance.currentStep}", this);

            // 1. 玩家離開時，隱藏UI提示
            if (uiPanel != null)
            {
                // 檢查是否真的需要隱藏，並且它當前是啟用的
                if (uiPanel.activeSelf)
                {
                    uiPanel.SetActive(false);
                    Debug.Log($"[Trigger Exit] 已成功隱藏 UI 提示: '{uiPanel.name}'。", uiPanel);
                }
                else
                {
                    Debug.Log($"[Trigger Exit] UI 面板 '{uiPanel.name}' 已經是非啟用狀態，無需再次隱藏。", uiPanel);
                }
            }
            else
            {
                Debug.LogWarning($"[Trigger Exit] 警告：'{this.gameObject.name}' 的 UI Panel 欄位未設定！無法隱藏UI。", this);
            }

            // 2. 點亮下一個區域的光 (通常在玩家完成互動並離開後點亮)
            if (nextGlowVisual != null)
            {
                // 額外檢查：確保我們是在正確的步驟離開時才點亮下一個
                // 這通常意味著當前步驟已經前進到下一個階段 (requiredStep + 1)
                if (GameManager.Instance != null && GameManager.Instance.currentStep == requiredStep + 1)
                {
                    if (!nextGlowVisual.activeSelf) // 避免重複SetActive(true)
                    {
                        nextGlowVisual.SetActive(true);
                        Debug.Log($"[Trigger Exit] 步驟正確，已點亮下一個發光物件: '{nextGlowVisual.name}'。", nextGlowVisual);
                    }
                }
                else
                {
                    Debug.LogWarning($"[Trigger Exit] '{this.gameObject.name}' 準備點亮下一個物件 '{nextGlowVisual.name}'，但遊戲步驟不符。遊戲目前步驟: {GameManager.Instance?.currentStep ?? -1} / 預期步驟: {requiredStep + 1}。", this);
                }
            }
            else
            {
                Debug.Log($"[Trigger Exit] '{this.gameObject.name}' 沒有設定 nextGlowVisual。", this);
            }
        }
        else
        {
            Debug.Log($"[Trigger Exit] 離開物件 '{other.name}' 不是玩家 (Tag: '{other.tag}')，不執行UI隱藏操作。", other);
        }
    }
}

