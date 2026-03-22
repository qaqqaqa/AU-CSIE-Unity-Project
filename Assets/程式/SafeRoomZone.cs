using UnityEngine;

public class SafeRoomZone : MonoBehaviour
{
    [Header("設定")]
    public string targetTag = "Player";

    [Tooltip("顯示在結算畫面上的成功原因")]
    public string successReason = "待在安全房間";

    private void OnTriggerEnter(Collider other)
    {
        // 1. 確認碰到的是玩家
        if (other.CompareTag(targetTag))
        {
            // 2. 【關鍵檢查】確認現在是否為「大火模式」(難度 2)
            // 只有大火模式，這個安全房才有效
            if (MainMenuController.SelectedDifficulty == 2)
            {
                Debug.Log("玩家進入安全房 (大火模式)！");

                if (GameManager.Instance != null)
                {
                    // 3. 觸發勝利，顯示「待在安全房間」
                    GameManager.Instance.EndGame(true, successReason);
                }
            }
            else
            {
                // 如果是小火模式，進去安全房可能不會直接贏 (看您設計)
                Debug.Log("進入安全房，但不是大火模式，不觸發勝利。");
            }
        }
    }
}