using UnityEngine;
using TMPro;

public class InteractionHintUI : MonoBehaviour
{
    public static InteractionHintUI Instance { get; private set; }

    [Header("UI 元件")]
    public TextMeshProUGUI hintText;
    public GameObject hintPanel; // 如果您有背景圖，可以控制整個 Panel

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        HideHint(); // 遊戲開始時先隱藏
    }

    public void ShowHint(string message)
    {
        if (hintText != null)
        {
            hintText.text = message;
            if (hintPanel != null) hintPanel.SetActive(true);
            else hintText.gameObject.SetActive(true);
        }
    }

    public void HideHint()
    {
        if (hintPanel != null) hintPanel.SetActive(false);
        else if (hintText != null) hintText.gameObject.SetActive(false);
    }
}

