using UnityEngine;
using TMPro; // 引用 TextMeshPro
using System.Collections; // 引用 Coroutine

public class PlayerSpawnManager : MonoBehaviour
{
    [Header("出生點設定")]
    // 陣列索引 0,1 是 1樓； 2,3 是 2樓 ... 12,13 是 7樓
    public Transform[] spawnPoints;
    public Transform teachingStartPoint; // 教學模式固定點

    [Header("UI 設定")]
    public GameObject missionPanel;
    public TextMeshProUGUI locationText; // 顯示樓層的文字框
    public TextMeshProUGUI objectiveText; // 顯示任務的文字框
    public float displayDuration = 4.0f; // 提示顯示幾秒後消失

    // --- 【新增】顏色設定欄位 ---
    [Header("提示文字顏色設定")]
    [Tooltip("教學模式的文字顏色 (建議：白色/青色)")]
    public Color tutorialColor = new Color(0.5f, 1f, 1f, 1f); // 預設淡青色

    [Tooltip("小火模式的文字顏色 (建議：黃色/橘色)")]
    public Color smallFireColor = Color.yellow; // 預設黃色

    [Tooltip("大火模式的文字顏色 (建議：紅色)")]
    public Color bigFireColor = Color.red;    // 預設紅色
    // --- 新增結束 ---


    void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            Debug.LogError("找不到 Player，請確認玩家有 Tag: Player");
            return;
        }

        // 檢查是否為限時模式 (難度 > 0)
        if (MainMenuController.SelectedDifficulty > 0)
        {
            // 🔥 限時模式邏輯 (隨機生成)
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                int index = Random.Range(0, spawnPoints.Length);
                if (index >= spawnPoints.Length) index = 0;

                player.transform.position = spawnPoints[index].position;
                player.transform.rotation = spawnPoints[index].rotation;

                int floorNumber = (index / 2) + 1;
                string locationMsg = $"目前位置：{floorNumber} 樓";
                string missionMsg = "";

                // 【修改】準備要使用的顏色變數
                Color targetColor;

                // 檢查難度
                if (MainMenuController.SelectedDifficulty == 2)
                {
                    // =====================================================
                    //       【核心修改：大火模式根據樓層改變任務】
                    // =====================================================

                    // 設定顏色為大火紅色 (這行不用變)
                    targetColor = bigFireColor;

                    // 根據計算出來的樓層 (floorNumber) 決定顯示什麼任務
                    if (floorNumber == 1)
                    {
                        // ★ 情況 A：出生在 1 樓 ★
                        missionMsg = "任務目標：原因是樓上正發生不可滅大火 請盡快跑去一樓，請迅速往出口撤離";
                    }
                    else
                    {
                        // ★ 情況 B：出生在 2~7 樓 ★
                        // (題目需求是3~7樓去安全房，但邏輯上只要不是1樓，在大火模式下通常都建議往安全區移動，
                        // 所以這裡寫 "else" 包含 2 樓在內的所有樓層，比較簡潔且合理)
                        missionMsg = "任務目標：火勢猛烈，原因是一樓火勢太大跑不出去請去二樓，請盡速前往 2 樓安全房避難";
                    }
                    // =====================================================
                    //             【修改結束】
                    // =====================================================
                }
                else
                {
                    // 小火模式 (難度1) 的文字，維持原樣不要動
                    missionMsg = "任務目標：撲滅所有火源並逃離大樓";
                    targetColor = smallFireColor;
                }

                Debug.Log($"限時模式(難度{MainMenuController.SelectedDifficulty})！玩家隨機出生在：{floorNumber} 樓");

                // 【修改】呼叫顯示 UI 時，多傳入一個顏色參數
                ShowMissionStartUI(locationMsg, missionMsg, targetColor);
            }
            else
            {
                Debug.LogError("錯誤：限時模式需要隨機生成，但 Spawn Points 陣列是空的！");
            }
        }
        else
        {
            // 📚 教學模式邏輯
            if (teachingStartPoint != null)
            {
                player.transform.position = teachingStartPoint.position;
                player.transform.rotation = teachingStartPoint.rotation;
            }

            Debug.Log("教學模式！玩家出生在固定位置");

            // 【修改】教學模式傳入教學顏色
            ShowMissionStartUI("目前位置：訓練中心", "任務目標：跟隨指引熟悉操作", tutorialColor);
        }
    }

    // --- 【修改】顯示 UI 的功能 (增加 Color 參數) ---
    private void ShowMissionStartUI(string location, string objective, Color objectiveColor)
    {
        if (missionPanel != null)
        {
            missionPanel.SetActive(true);

            if (locationText != null) locationText.text = location;

            if (objectiveText != null)
            {
                objectiveText.text = objective;
                // 【關鍵修改】在這裡將傳入的顏色套用到文字元件上
                objectiveText.color = objectiveColor;
            }

            // 開始倒數關閉 UI
            StartCoroutine(HideMissionPanelAfterDelay());
        }
    }

    // --- 自動淡出或關閉 UI 的協程 ---
    IEnumerator HideMissionPanelAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        if (missionPanel != null)
        {
            missionPanel.SetActive(false);
        }
    }
}