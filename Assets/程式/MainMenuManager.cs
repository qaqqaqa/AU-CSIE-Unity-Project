using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public static bool IsTimedMode = false;  // 🔥 用來判斷是否為限時模式

    public void LoadTutorial()
    {
        IsTimedMode = false; // 教學模式
        SceneManager.LoadScene("Tutorial");
    }

    public void LoadEscape()
    {
        IsTimedMode = true; // 🔥 限時模式
        SceneManager.LoadScene("Escape");
    }

    public void LoadItemInfo()
    {
        IsTimedMode = false;
        SceneManager.LoadScene("ItemInfo");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("遊戲關閉");
    }
}
