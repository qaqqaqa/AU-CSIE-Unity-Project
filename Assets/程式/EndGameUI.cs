using UnityEngine;
using TMPro; // 記得引用 TextMeshPro 來控制文字

public class EndGameUI : MonoBehaviour
{
    [Header("UI 元件參考")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI reasonText;

    // 這個方法將由 GameManager 呼叫
    public void ShowEndScreen(bool isSuccess, string reason)
    {
        // 根據成功或失敗，設定不同的標題和顏色
        if (isSuccess)
        {
            titleText.text = "任務成功";
            resultText.text = "成功";
            resultText.color = Color.green;
        }
        else
        {
            titleText.text = "任務失敗";
            resultText.text = "失敗";
            resultText.color = Color.red;
        }

        // 設定失敗或成功的原因文字
        reasonText.text = reason;

        // 將整個結算畫面 Panel 顯示出來
        gameObject.SetActive(true);
    }

    // 這個方法負責「重新開始目前關卡」
    public void OnRestartButtonClick()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartLevel();
        }
    }

    // --- 【新增的方法】 ---
    /// <summary>
    /// 這個方法將被「回到主選單」按鈕呼叫
    /// </summary>
    public void OnReturnToMainMenuClick()
    {
        // 通知 GameManager 回到主選單
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMainMenu();
        }
    }
}