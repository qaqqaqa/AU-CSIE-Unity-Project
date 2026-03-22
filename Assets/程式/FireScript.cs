using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(Collider))]
public class FireScript : MonoBehaviour
{
    private ParticleSystem fireParticleSystem;
    private Collider fireCollider;

    // --- 定義火災類型 ---
    public enum FireType
    {
        General, // 一般火災
        Oil,     // 油鍋火災
        Electric // 電器火災
    }

    [Header("🔥 火災類型設定")]
    [Tooltip("設定這是哪種類型的火災，會影響提示文字")]
    public FireType fireType = FireType.General;

    [Header("🔥 火焰設定")]
    public float damagePerSecond = 20f;

    [Header("滅火設定")]
    public float extinguishDuration = 10f;
    public float regainDuration = 5f;
    public float totalFireHealth = 100f;
    private float currentFireHealth;

    private bool isBeingExtinguishedThisFrame = false;
    private bool isFullyExtinguished = false;
    private float initialEmissionRate;

    void Awake()
    {
        fireParticleSystem = GetComponent<ParticleSystem>();
        fireCollider = GetComponent<Collider>();

        if (fireCollider != null)
            fireCollider.isTrigger = true;

        if (fireParticleSystem != null)
        {
            var emission = fireParticleSystem.emission;
            initialEmissionRate = emission.rateOverTime.constant > 0 ? emission.rateOverTime.constant : 50f;
            fireParticleSystem.Play();
        }

        currentFireHealth = totalFireHealth;
        isFullyExtinguished = false;
    }

    void Update()
    {
        if (isFullyExtinguished) return;

        // 🔥 自動恢復火勢
        if (!isBeingExtinguishedThisFrame && currentFireHealth < totalFireHealth)
        {
            currentFireHealth += (totalFireHealth / regainDuration) * Time.deltaTime;
            currentFireHealth = Mathf.Min(currentFireHealth, totalFireHealth);
            UpdateFireVisuals();
        }

        // ✅ 火勢降到 0 → 熄滅
        if (currentFireHealth <= 0 && !isFullyExtinguished)
        {
            ExtinguishCompletely();
        }

        isBeingExtinguishedThisFrame = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (isFullyExtinguished) return;

        // --- 【修改】只檢查 PlayerHealth，移除導致錯誤的 PlayerStatus ---
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damagePerSecond * Time.deltaTime);
        }
        // --- 修改結束 ---
    }

    public bool IsFullyExtinguished() => isFullyExtinguished;

    public void ExtinguishStep(float amount)
    {
        if (isFullyExtinguished) return;

        isBeingExtinguishedThisFrame = true;
        currentFireHealth -= amount * Time.deltaTime;
        currentFireHealth = Mathf.Max(0, currentFireHealth);

        UpdateFireVisuals();

        if (currentFireHealth <= 0 && !isFullyExtinguished)
        {
            ExtinguishCompletely();
        }
    }

    public void ExtinguishCompletely()
    {
        if (isFullyExtinguished) return;
        isFullyExtinguished = true;

        // 通知 GameManager 勝利條件
        if (GameManager.Instance != null)
        {
            GameManager.Instance.FireExtinguished(this);
        }

        Debug.Log($"🔥 {gameObject.name} 完全熄滅！");

        // 停止粒子
        if (fireParticleSystem != null)
            fireParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        // 關閉碰撞器
        if (fireCollider != null)
            fireCollider.enabled = false;

        // 延遲銷毀
        Destroy(gameObject, 1.5f);
    }

    private void UpdateFireVisuals()
    {
        if (fireParticleSystem == null) return;

        float healthRatio = Mathf.Clamp01(currentFireHealth / totalFireHealth);
        var emission = fireParticleSystem.emission;
        emission.rateOverTime = initialEmissionRate * healthRatio;
    }

    public float GetCurrentFireHealth()
    {
        return currentFireHealth;
    }

    // --- 獲取提示文字的方法 ---
    public string GetExtinguishHint()
    {
        switch (fireType)
        {
            case FireType.General:
                return "【一般火災】\n建議使用：滅火器";
            case FireType.Oil:
                return "【廚房油類火災】\n建議使用：滅火器、濕抹布、蘇打粉\n<color=red>禁止用水！</color>";
            case FireType.Electric:
                return "【電器火災】\n建議使用：滅火器\n<color=red>禁止用水！</color>";
            default:
                return "未知火災";
        }
    }
}