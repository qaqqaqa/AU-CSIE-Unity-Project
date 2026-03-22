using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SodaPowderBox : MonoBehaviour, IUsable, IPickupableItem
{
    [Header("🧂 蘇打粉設定")]
    public ParticleSystem powderParticleSystem;
    public float extinguishPower = 30f;
    public string targetFireTag = "OilFire";
    public float effectiveRange = 2f; // 粉末噴出有效範圍（距離滅火保險）

    private bool isSpraying = false;
    private Rigidbody rb;
    private Collider col;
    private AudioSource audioSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
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
                Debug.Log("🧂 自動綁定粉末粒子系統。", this);
            else
                Debug.LogError("❌ 找不到粉末粒子系統！請在 Inspector 指定。", this);
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
            Debug.Log("🧂 開始噴灑蘇打粉！");
        }
    }

    private void StopSpray()
    {
        if (powderParticleSystem != null)
            powderParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        if (audioSource != null)
            audioSource.Stop();
    }

    // 🔥 粒子真正打到火焰
    private void OnParticleCollision(GameObject other)
    {
        TryExtinguish(other, "🧂 粒子碰撞滅火");
    }

    // 🔥 距離滅火保險：避免粒子偵測不到
    void Update()
    {
        if (!isSpraying) return;

        Collider[] hits = Physics.OverlapSphere(powderParticleSystem.transform.position, effectiveRange);
        foreach (var hit in hits)
        {
            TryExtinguish(hit.gameObject, "🧂 距離滅火保險");
        }
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
