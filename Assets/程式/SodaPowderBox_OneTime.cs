using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SodaPowderBox_OneTime : MonoBehaviour, IUsable, IPickupableItem
{
    [Header("🧂 蘇打粉設定")]
    public ParticleSystem powderParticleSystem;
    public float extinguishPower = 30f;
    public string targetFireTag = "OilFire";
    public float effectiveRange = 2f;

    [Header("限時模式設定")]
    [Tooltip("可以持續倒粉的總時間 (秒)")]
    public float maxUsageTime = 3.0f;
    private float currentUsageTimer;
    private bool isDepleted = false;

    private bool isSpraying = false;
    private Rigidbody rb;
    private Collider col;
    private AudioSource audioSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();

        currentUsageTimer = maxUsageTime; // 初始化

        ValidateParticleSystem();
    }

    void OnEnable()
    {
        ValidateParticleSystem();
        StopSpray();
        isSpraying = false;
    }

    private void ValidateParticleSystem()
    {
        if (powderParticleSystem == null)
        {
            powderParticleSystem = GetComponentInChildren<ParticleSystem>();
            if (powderParticleSystem != null)
                Debug.Log("🧂 (限時版) 自動綁定粉末粒子系統。", this);
            else
                Debug.LogError("❌ (限時版) 找不到粉末粒子系統！", this);
        }

        if (powderParticleSystem != null)
        {
            var main = powderParticleSystem.main;
            main.playOnAwake = false;
            powderParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var collision = powderParticleSystem.collision;
            collision.enabled = true;
            collision.sendCollisionMessages = true;
            collision.type = ParticleSystemCollisionType.World;
        }
    }

    public void Use()
    {
        if (isDepleted)
        {
            Debug.Log("❌ 蘇打粉已倒完！無法再使用。");
            return;
        }

        if (powderParticleSystem == null)
            ValidateParticleSystem();

        isSpraying = !isSpraying;
        if (isSpraying)
            StartSpray();
        else
            StopSpray();
    }

    private void StartSpray()
    {
        if (powderParticleSystem != null && !powderParticleSystem.isPlaying)
        {
            powderParticleSystem.Play();
            if (audioSource != null) audioSource.Play();
            Debug.Log("🧂 (限時版) 開始噴灑！");
        }
    }

    private void StopSpray()
    {
        if (powderParticleSystem != null)
            powderParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        if (audioSource != null)
            audioSource.Stop();
    }

    private void OnParticleCollision(GameObject other)
    {
        TryExtinguish(other, "🧂 粒子碰撞滅火");
    }

    void Update()
    {
        if (!isSpraying) return;

        // --- 【新增】計時邏輯 ---
        currentUsageTimer -= Time.deltaTime;
        if (currentUsageTimer <= 0)
        {
            RunOutOfAmmo();
            return;
        }
        // --- 結束 ---

        Collider[] hits = Physics.OverlapSphere(powderParticleSystem.transform.position, effectiveRange);
        foreach (var hit in hits)
        {
            TryExtinguish(hit.gameObject, "🧂 距離滅火保險");
        }
    }

    // --- 【新增】用盡時的處理 ---
    private void RunOutOfAmmo()
    {
        isSpraying = false;
        isDepleted = true;
        StopSpray();
        Debug.Log("⚠️ 蘇打粉已倒完，銷毀物件！");
        Destroy(gameObject);
    }

    private void TryExtinguish(GameObject target, string source)
    {
        if (!target.CompareTag(targetFireTag)) return;

        FireScript fire = target.GetComponent<FireScript>();
        if (fire != null && !fire.IsFullyExtinguished())
        {
            fire.ExtinguishStep(extinguishPower);
            Debug.Log($"{source}: {target.name} → 剩餘火勢更新中");
        }
    }

    public void OnPickup()
    {
        if (rb != null) { rb.isKinematic = true; rb.detectCollisions = false; }
        if (col != null) col.enabled = false;
        isSpraying = false;
        StopSpray();
    }

    public void OnDrop()
    {
        if (rb != null) { rb.isKinematic = false; rb.detectCollisions = true; }
        if (col != null) col.enabled = true;
        isSpraying = false;
        StopSpray();
        ValidateParticleSystem();
    }
}
