using UnityEngine;
using UnityEngine.SceneManagement; // 引用場景管理

public class MainMenuController : MonoBehaviour
{
    [Header("UI 面板")]
    public GameObject controlsPanel;   // 人物操作面板
    public GameObject levelInfoPanel;  // 關卡介紹主面板
    public GameObject modeSelectPanel; // 模式選擇面板

    [Header("關卡介紹頁面")]
    public GameObject tutorialPage;    // 教學關卡頁 (Page 1)
    public GameObject timedPage;       // 限時關卡頁 (Page 2)

    // 【靜態變數】儲存難度：0=教學/無, 1=小火, 2=大火
    public static int SelectedDifficulty = 0;

    // --- 人物操作功能 ---
    public void OpenControls()
    {
        if (controlsPanel != null) controlsPanel.SetActive(true);
    }

    public void CloseControls()
    {
        if (controlsPanel != null) controlsPanel.SetActive(false);
    }

    // --- 關卡介紹功能 ---
    public void OpenLevelInfo()
    {
        if (levelInfoPanel != null)
        {
            levelInfoPanel.SetActive(true);
            // 每次打開時，重置回第一頁
            ShowTutorialPage();
        }
    }

    public void CloseLevelInfo()
    {
        if (levelInfoPanel != null) levelInfoPanel.SetActive(false);
    }

    // --- 翻頁功能 ---
    public void ShowTutorialPage()
    {
        if (tutorialPage != null) tutorialPage.SetActive(true);
        if (timedPage != null) timedPage.SetActive(false);
    }

    public void ShowTimedPage()
    {
        if (tutorialPage != null) tutorialPage.SetActive(false);
        if (timedPage != null) timedPage.SetActive(true);
    }

    // --- 【模式選擇與開始遊戲功能】 ---

    // 1. 打開模式選擇面板 (綁定在主選單的「限時關卡」按鈕)
    public void OpenModeSelect()
    {
        if (modeSelectPanel != null) modeSelectPanel.SetActive(true);
    }

    // 2. 關閉模式選擇面板
    public void CloseModeSelect()
    {
        if (modeSelectPanel != null) modeSelectPanel.SetActive(false);
    }

    // 3. 選擇小火模式 (綁定在「小火模式」按鈕)
    public void StartSmallFireMode()
    {
        SelectedDifficulty = 1; // 設定為小火
        LoadTimedLevel();
    }

    // 4. 選擇大火模式 (綁定在「大火模式」按鈕)
    public void StartBigFireMode()
    {
        SelectedDifficulty = 2; // 設定為大火
        LoadTimedLevel();
    }

    // 5. 【新增】開始教學模式 (綁定在主選單的「教學關卡」按鈕)
    public void StartTutorialMode()
    {
        // 重要：重置難度為 0，這樣 PlayerSpawnManager 就會知道要用「固定出生點」
        SelectedDifficulty = 0;

        // 載入教學關卡場景 (請確認您的教學關卡場景名稱是否為 "TeachingLevel")
        // 如果您的教學關卡名稱不同，請修改下面的字串
        SceneManager.LoadScene("Tutorial");
    }

    // --- 輔助方法：載入限時關卡 ---
    // 載入限時關卡場景
    private void LoadTimedLevel()
    {
        // 檢查玩家選擇的難度
        if (SelectedDifficulty == 1)
        {
            // 難度 1 (小火) -> 載入原本的場景
            SceneManager.LoadScene("Escape");
        }
        else if (SelectedDifficulty == 2)
        {
            // 難度 2 (大火) -> 載入新複製的場景
            SceneManager.LoadScene("Escape_BigFire");
        }
    }

}