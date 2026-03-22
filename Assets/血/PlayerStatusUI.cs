using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    [Header("🔹 UI 元件連結")]
    public Slider healthSlider;
    public Slider oxygenSlider;
    private PlayerHealth playerHealth;
    private PlayerOxygen playerOxygen;

    void Start()
    {
        // --- 【核心修改點】 ---
        // 使用 Unity 推薦的、效能更好的新方法來尋找物件
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        playerOxygen = FindFirstObjectByType<PlayerOxygen>();
        // --- 修改結束 ---

        // 初始化血條
        if (playerHealth != null)
        {
            healthSlider.maxValue = playerHealth.maxHealth;
            healthSlider.value = playerHealth.maxHealth;
            playerHealth.HealthChanged += OnHealthChanged;
        }

        // 初始化氧氣條
        if (playerOxygen != null)
        {
            oxygenSlider.maxValue = playerOxygen.maxOxygen;
            oxygenSlider.value = playerOxygen.maxOxygen;
            playerOxygen.OxygenChanged += OnOxygenChanged;
        }
    }

    private void OnHealthChanged(float current, float max)
    {
        healthSlider.value = current;
    }

    private void OnOxygenChanged(float current, float max)
    {
        oxygenSlider.value = current;
    }

    // 建議在物件被摧毀時，取消事件訂閱，避免潛在錯誤
    void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.HealthChanged -= OnHealthChanged;
        }
        if (playerOxygen != null)
        {
            playerOxygen.OxygenChanged -= OnOxygenChanged;
        }
    }
}
