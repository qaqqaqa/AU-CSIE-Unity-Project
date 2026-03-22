using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("❤️ 血量設定")]
    public float maxHealth = 600f;
    private float currentHealth;

    // 🔔 通知 UI 更新
    public delegate void OnHealthChanged(float current, float max);
    public event OnHealthChanged HealthChanged;

    void Start()
    {
        currentHealth = maxHealth;
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        // 如果遊戲已經結束，就不再扣血
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded()) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);
        HealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        // --- 【核心修改】判斷死亡原因 ---
        string reason = "玩家已死亡"; // 預設原因

        // 如果是大火模式 (難度 2)，原因顯示為「血量歸零」
        if (MainMenuController.SelectedDifficulty == 2)
        {
            reason = "血量歸零";
        }
        // --- 修改結束 ---

        Debug.Log($"🔥 玩家死亡！原因：{reason}");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.EndGame(false, reason);
        }
    }

    public float GetHealthRatio()
    {
        return currentHealth / maxHealth;
    }
}
