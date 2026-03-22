using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FireExtinguisher_OneTime : MonoBehaviour, IUsable, IPickupableItem
{
    [Header("效果設定")]
    [Tooltip("滅火泡沫的粒子系統")]
    public ParticleSystem foamParticleSystem;

    [Header("滅火參數")]
    public float extinguishRange = 3f;      // 泡沫能影響的範圍
    public float extinguishPower = 50f;     // 每秒滅火強度
    public LayerMask fireLayerMask;         // 限定只打到火焰

    [Header("限時模式設定")]
    [Tooltip("可以持續噴射的總時間 (秒)")]
    public float maxUsageTime = 5.0f;
    private float currentUsageTimer;
    private bool isDepleted = false; // 是否已用盡

    private bool isSpraying = false;
    private Rigidbody rb;
    private Collider col;
    private AudioSource audioSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();

        currentUsageTimer = maxUsageTime; // 初始化計時器

        // ✅ 若沒設 layerMask，自動指定 "Fire" layer
        if (fireLayerMask == 0)
        {
            int fireLayer = LayerMask.NameToLayer("Fire");
            if (fireLayer >= 0)
                fireLayerMask = 1 << fireLayer;
            else
                Debug.LogWarning("⚠️ Fire layer 不存在，請確認是否有建立 'Fire' layer");
        }

        ValidateFoamSystem();
    }

    void OnEnable()
    {
        ValidateFoamSystem();
        StopFoam();
        isSpraying = false;
    }

    private void ValidateFoamSystem()
    {
        if (foamParticleSystem == null)
        {
            foamParticleSystem = GetComponentInChildren<ParticleSystem>();
            if (foamParticleSystem != null)
                Debug.Log("🧯 (限時版) 自動重新綁定泡沫粒子系統。", this);
            else
                Debug.LogError("❌ (限時版) 找不到泡沫粒子系統！", this);
        }

        if (foamParticleSystem != null)
        {
            var main = foamParticleSystem.main;
            main.playOnAwake = false;
            foamParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    void Update()
    {
        if (isSpraying)
        {
            // --- 【新增】計時邏輯 ---
            currentUsageTimer -= Time.deltaTime;
            if (currentUsageTimer <= 0)
            {
                RunOutOfAmmo();
                return; // 用盡後停止後續滅火邏輯
            }
            // --- 結束 ---

            ExtinguishNearbyFires();
        }
    }

    public void Use()
    {
        if (isDepleted)
        {
            Debug.Log("❌ 滅火器已用盡！無法再使用。");
            return;
        }

        if (foamParticleSystem == null)
        {
            ValidateFoamSystem();
        }

        isSpraying = !isSpraying;
        if (isSpraying)
            StartFoam();
        else
            StopFoam();
    }

    // --- 【新增】用盡時的處理 ---
    private void RunOutOfAmmo()
    {
        isSpraying = false;
        isDepleted = true;
        StopFoam();
        Debug.Log("⚠️ 滅火器噴射時間結束，已銷毀！");

        // 這裡選擇直接銷毀物件，讓手變空
        Destroy(gameObject);
    }

    private void ExtinguishNearbyFires()
    {
        if (foamParticleSystem == null) return;

        Vector3 origin = foamParticleSystem.transform.position;
        Collider[] hits = Physics.OverlapSphere(origin, extinguishRange, fireLayerMask);

        foreach (Collider hit in hits)
        {
            FireScript fire = hit.GetComponent<FireScript>();
            if (fire != null && !fire.IsFullyExtinguished())
            {
                fire.ExtinguishStep(extinguishPower);
                Debug.Log($"💧 (限時版) 泡沫滅火中: {hit.name}");
            }
        }
    }

    public void StartFoam()
    {
        if (foamParticleSystem != null && !foamParticleSystem.isPlaying)
        {
            foamParticleSystem.Play();
            if (audioSource != null) audioSource.Play();
            Debug.Log("🧯 (限時版) 開始噴射。");
        }
    }

    public void StopFoam()
    {
        if (foamParticleSystem != null)
            foamParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        if (audioSource != null)
            audioSource.Stop();
    }

    public void OnPickup()
    {
        if (rb != null) { rb.isKinematic = true; rb.detectCollisions = false; }
        if (col != null) col.enabled = false;
        isSpraying = false;
        StopFoam();
    }

    public void OnDrop()
    {
        if (rb != null) { rb.isKinematic = false; rb.detectCollisions = true; }
        if (col != null) col.enabled = true;
        isSpraying = false;
        StopFoam();
        ValidateFoamSystem();
    }
}
