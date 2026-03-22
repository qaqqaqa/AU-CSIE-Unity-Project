using UnityEngine;
using UnityEngine.SceneManagement; // 引入場景管理命名空間

public class SceneLoader : MonoBehaviour
{
    // --- 原有功能 ---

    // 回主選單 (通常接在 "關閉" 按鈕)
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // 去第一頁 (滅火器)
    public void GoToItemInfo1()
    {
        SceneManager.LoadScene("ItemInfo1");
    }

    // 去第二頁 (抹布 / 蘇打粉)
    public void GoToItemInfo2()
    {
        SceneManager.LoadScene("ItemInfo2");
    }

    // --- 【新增】去第三頁 (緊急出口) ---
    public void GoToItemInfo3()
    {
        // 載入您剛剛建立的新場景名稱，請確認名稱完全一致
        SceneManager.LoadScene("ItemInfo3");
    }
}
