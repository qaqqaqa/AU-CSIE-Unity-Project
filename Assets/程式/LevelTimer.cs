using UnityEngine;
using UnityEngine.UI; // 如果要顯示在 UI Text 上需要引用這個命名空間

public class LevelTimer : MonoBehaviour
{
    public float timeLimit = 60f; // 設定關卡時間限制 (例如 60 秒)
    private float currentTime; // 當前計時器的時間
    public Text timerText; // 用於顯示時間的 UI Text 元件

    private bool timerIsRunning = false; // 計時器是否正在運行

    void Start()
    {
        currentTime = timeLimit; // 初始化當前時間為時間限制
        timerIsRunning = true; // 在關卡開始時啟動計時器
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime; // 減少時間
                DisplayTime(currentTime); // 更新 UI 顯示
            }
            else
            {
                currentTime = 0; // 確保時間不會變成負數
                timerIsRunning = false; // 停止計時器
                DisplayTime(currentTime); // 最後更新一次 UI 顯示
                // 在這裡處理時間到或遊戲失敗的邏輯
                Debug.Log("時間到！遊戲失敗！");
                // 例如：呼叫一個遊戲結束的函式或載入場景
                // GameManager.Instance.GameOver();
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        // 確保顯示的時間不會是負數
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }

        // 計算分鐘和秒數
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        //float milliseconds = (timeToDisplay % 1) * 1000; // 如果需要毫秒

        // 將時間格式化為 "分鐘:秒數"
        // 例如：05:30
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        // 如果需要毫秒，可以使用 "{0:00}:{1:00}.{2:000}"
        // timerText.text = string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);

    }

    // 如果需要在遊戲中途停止計時器 (例如遊戲暫停)
    public void StopTimer()
    {
        timerIsRunning = false;
    }

    // 如果需要在遊戲中途啟動計時器 (例如遊戲繼續)
    public void StartTimer()
    {
        timerIsRunning = true;
    }
}
