using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FireExtinguisher : MonoBehaviour, IUsable, IPickupableItem
{
    [Header("效果設定")]
    [Tooltip("滅火泡沫的粒子系統")]
    public ParticleSystem foamParticleSystem;

    [Header("滅火參數")]
    public float extinguishRange = 3f;      // 泡沫能影響的範圍
    public float extinguishPower = 50f;     // 每秒滅火強度
    public LayerMask fireLayerMask;         // 限定只打到火焰

    private bool isSpraying = false;
    private Rigidbody rb;
    private Collider col;
    private AudioSource audioSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();

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
        // ✅ 關鍵修正：重新綁定粒子系統與 Audio
        ValidateFoamSystem();
        StopFoam();
        isSpraying = false;
    }

    private void ValidateFoamSystem()
    {
        // 如果粒子系統引用遺失，嘗試重新尋找
        if (foamParticleSystem == null)
        {
            foamParticleSystem = GetComponentInChildren<ParticleSystem>();
            if (foamParticleSystem != null)
                Debug.Log("🧯 FireExtinguisher: 自動重新綁定泡沫粒子系統。", this);
            else
                Debug.LogError("❌ FireExtinguisher: 找不到泡沫粒子系統！", this);
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
            ExtinguishNearbyFires();
        }
    }

    public void Use()
    {
        if (foamParticleSystem == null)
        {
            ValidateFoamSystem(); // 🔧 確保重新拿出後仍能噴
        }

        isSpraying = !isSpraying;
        if (isSpraying)
            StartFoam();
        else
            StopFoam();
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
                Debug.Log($"💧 泡沫滅火中: {hit.name}");
            }
        }
    }

    public void StartFoam()
    {
        if (foamParticleSystem != null && !foamParticleSystem.isPlaying)
        {
            foamParticleSystem.Play();
            if (audioSource != null) audioSource.Play();
            Debug.Log("🧯 滅火器開始噴射。");
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
        ValidateFoamSystem(); // ✅ 確保重新生成後仍能噴
    }
}
