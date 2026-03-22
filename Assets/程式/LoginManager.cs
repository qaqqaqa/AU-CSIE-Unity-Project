using UnityEngine;
using UnityEngine.SceneManagement;
// 不需要 TMPro 命名空間了，因為不再使用 TMP_InputField

public class LoginManager : MonoBehaviour
{
    // 不再需要這個變數
    // public TMP_InputField playerNameInput; 

    public void StartGame()
    {
        // 移除獲取玩家名字和檢查是否為空的部分
        // string playerName = playerNameInput.text;
        // if (string.IsNullOrEmpty(playerName))
        // {
        //     Debug.Log("請輸入名字！");
        //     return;
        // }

        // 移除儲存玩家名字的部分
        // PlayerPrefs.SetString("PlayerName", playerName); 

        // 直接載入主選單場景
        SceneManager.LoadScene("MainMenu");

        // 如果你的主選單場景名稱不是 "MainMenu"，請將上面的 "MainMenu" 替換為你實際的場景名稱
    }
}

