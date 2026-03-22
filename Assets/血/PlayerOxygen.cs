using UnityEngine;

public class PlayerOxygen : MonoBehaviour
{
    [Header("🌬️ 氧氣設定")]
    public float maxOxygen = 600f;
    public float oxygenDecreaseRate = 2f;

    private float currentOxygen;
    private PlayerHealth playerHealth;

    public delegate void OnOxygenChanged(float current, float max);
    public event OnOxygenChanged OxygenChanged;

    void Start()
    {
        currentOxygen = maxOxygen;
        playerHealth = GetComponent<PlayerHealth>();
        OxygenChanged?.Invoke(currentOxygen, maxOxygen);
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded()) return;

        // 消耗氧氣
        if (currentOxygen > 0)
        {
            currentOxygen -= oxygenDecreaseRate * Time.deltaTime;
        }

        // 確保數值不小於 0
        currentOxygen = Mathf.Max(currentOxygen, 0);
        OxygenChanged?.Invoke(currentOxygen, maxOxygen);

        // --- 【核心修改】氧氣歸零後的邏輯 ---
        if (currentOxygen <= 0)
        {
            // 情況 A：大火模式 (難度 2) -> 直接失敗，顯示「氧氣歸零」
            if (MainMenuController.SelectedDifficulty == 2)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.EndGame(false, "氧氣歸零");
                }
                return; // 結束，不再扣血
            }

            // 情況 B：其他模式 -> 持續扣血 (保持舊邏輯)
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(5f * Time.deltaTime);
            }
        }
        // --- 修改結束 ---
    }

    public float GetOxygenRatio()
    {
        return currentOxygen / maxOxygen;
    }
}
