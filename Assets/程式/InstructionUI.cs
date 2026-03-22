// InstructionUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro; // 如果您使用TextMeshPro

public class InstructionUI : MonoBehaviour
{
    public static InstructionUI Instance;
    public GameObject panel;        // 拖曳InstructionPanel進來
    public TextMeshProUGUI messageText; // 拖曳InstructionText進來

    private void Awake()
    {
        Instance = this;
    }

    public static void ShowMessage(string message)
    {
        if (Instance != null)
        {
            Instance.panel.SetActive(true);
            Instance.messageText.text = message;
        }
    }

    public static void Hide()
    {
        if (Instance != null)
        {
            Instance.panel.SetActive(false);
        }
    }
}
