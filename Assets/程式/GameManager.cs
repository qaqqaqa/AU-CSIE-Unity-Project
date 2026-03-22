using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int currentStep = 0;

    [Header("遊戲計時與狀態")]
    [Tooltip("遊戲總時間 (秒)")]
    public float totalGameTime = 300f;
    private float timeRemaining;
    private bool isGameEnded = false;

    [Header("UI 介面")]
    [Tooltip("時間顯示文字")]
    public TextMeshProUGUI timerText;
    [Tooltip("結算畫面腳本")]
    public EndGameUI endGameUI;

    // 儲存當前場景中需要被撲滅的火焰
    private List<FireScript> activeFires;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        timeRemaining = totalGameTime;
        isGameEnded = false;
        Time.timeScale = 1f;

        // 初始化列表，但先不自動抓取，等待 RandomFireActivator 呼叫 SetFiresToTrack
        if (activeFires == null)
        {
            activeFires = new List<FireScript>();
        }

        // 如果您希望在沒有 RandomFireActivator 的教學關卡也能運作
        // 可以保留下面這段自動抓取的備用方案
        // 但因為您現在主要依賴 RandomFireActivator，所以這裡先保持空列表
    }

    // --- 【核心方法】由 RandomFireActivator 呼叫 ---
    public void SetFiresToTrack(List<FireScript> firesToTrack)
    {
        activeFires = firesToTrack;

        Debug.Log($"--- GameManager 已收到任務，本次需撲滅 {activeFires.Count} 個火源 ---");

        // 如果一開始就沒有火源 (例如某些特殊情況)，直接勝利
        if (activeFires.Count == 0 && !isGameEnded)
        {
            Debug.Log("【勝利檢查】: 偵測到 0 個火源，立即觸發勝利！");
            EndGame(true, "所有火災皆已撲滅");
        }
    }

    private void Update()
    {
        if (isGameEnded) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        }
        else
        {
            timeRemaining = 0;

            // --- 【核心修改：根據模式判斷時間到的原因】 ---
            string timeUpReason = "時間耗盡，未滅火或是跑到一樓"; // 預設原因

            // 如果是大火模式 (難度 2)，時間到代表沒逃出去也沒進安全房
            if (MainMenuController.SelectedDifficulty == 2)
            {
                timeUpReason = "沒有待在安全房";
            }

            EndGame(false, timeUpReason);
            // --- 修改結束 ---
        }

        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = (int)timeRemaining / 60;
            int seconds = (int)timeRemaining % 60;
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void NextStep()
    {
        currentStep++;
        Debug.Log("進入教學步驟：" + currentStep);
    }

    public void FireExtinguished(FireScript fire)
    {
        if (isGameEnded) return;

        if (activeFires.Contains(fire))
        {
            activeFires.Remove(fire);
            Debug.Log($"【收到回報】: '{fire.gameObject.name}' 已撲滅。剩餘 {activeFires.Count} 個火源。");
        }
        else
        {
            // 這條警告可以幫助您發現是否有不在列表中的火被滅了
            Debug.LogWarning($"【警告】: '{fire.gameObject.name}' 回報已撲滅，但它並不在 GameManager 的追蹤列表中！");
        }

        if (activeFires.Count == 0)
        {
            Debug.Log("【勝利檢查】: 火焰列表為 0！正在觸發勝利畫面...");
            EndGame(true, "所有火災皆已撲滅");
        }
        else
        {
            Debug.Log($"【勝利檢查】: 火焰列表還有 {activeFires.Count} 個，勝利未觸發。");
        }
    }

    public void EndGame(bool isSuccess, string reason)
    {
        if (isGameEnded) return;

        isGameEnded = true;
        Debug.Log($"遊戲結束。原因: {reason}");

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (endGameUI != null)
        {
            endGameUI.ShowEndScreen(isSuccess, reason);
        }
    }

    public bool IsGameEnded() { return isGameEnded; }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}