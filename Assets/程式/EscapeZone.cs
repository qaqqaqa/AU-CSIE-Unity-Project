using UnityEngine;

public class EscapeZone : MonoBehaviour
{
    [Header("設定")]
    [Tooltip("只有這個 Tag 的物件進入才會觸發勝利 (通常是 Player)")]
    public string targetTag = "Player";

    // 這個變數可以保留作為預設值，或是給小火模式使用
    [Tooltip("預設的成功原因文字")]
    public string successReason = "成功離開火災現場，並撥打119";

    private void OnTriggerEnter(Collider other)
    {
        // 1. 檢查進入觸發範圍的是不是玩家
        if (other.CompareTag(targetTag))
        {
            Debug.Log("玩家抵達撤離點！");

            // 2. 通知 GameManager 結束遊戲
            if (GameManager.Instance != null)
            {
                // --- 【核心修改：根據難度決定文字】 ---
                string finalMessage = successReason; // 先預設為 Inspector 設定的文字

                // 檢查目前難度 (2 代表大火模式)
                if (MainMenuController.SelectedDifficulty == 2)
                {
                    // 大火模式：只顯示離開現場，不顯示撥打119
                    finalMessage = "成功離開火災現場";
                }
                else
                {
                    // 小火模式 (或其他)：顯示完整文字
                    finalMessage = "成功離開火災現場，並撥打119";
                }
                // --- 修改結束 ---

                // 第一個參數 true 代表「成功」
                // 第二個參數傳入我們剛剛決定好的 finalMessage
                GameManager.Instance.EndGame(true, finalMessage);
            }
            else
            {
                Debug.LogError("EscapeZone: 找不到 GameManager 實例！無法觸發勝利。");
            }
        }
    }
}