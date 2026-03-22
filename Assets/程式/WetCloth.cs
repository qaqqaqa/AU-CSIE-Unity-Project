using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class WetCloth : MonoBehaviour, IUsable, IPickupableItem
{
    [Header("滅火設定")]
    public float extinguishPower = 30f;
    public string targetFireTag = "NormalFire";
    public LayerMask fireLayerMask;
    public AudioSource extinguishSound;

    private Rigidbody rb;
    private Collider col;
    private bool isHeld = false;

    private bool hasInitialized = false;

    void Awake()
    {
        Initialize();
    }

    void OnEnable()
    {
        Initialize();

        // 🔧 確保重新啟用（例如從背包生成）時也能滅火
        rb.isKinematic = isHeld;
        rb.useGravity = !isHeld;
        col.isTrigger = false;

        // 🔥 自動找 Fire layer（避免 prefab 或 runtime 生成時遺失）
        if (fireLayerMask == 0)
        {
            int fireLayer = LayerMask.NameToLayer("Fire");
            if (fireLayer >= 0)
                fireLayerMask = 1 << fireLayer;
        }

        Debug.Log($"🧻 WetCloth 啟用完成（LayerMask={fireLayerMask.value}）");
    }

    private void Initialize()
    {
        if (hasInitialized) return;

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        rb.useGravity = true;
        rb.isKinematic = false;
        col.isTrigger = false;

        hasInitialized = true;
    }

    void FixedUpdate()
    {
        if (!isHeld)
            ExtinguishNearbyFires();
    }

    private void ExtinguishNearbyFires()
    {
        if (fireLayerMask == 0)
        {
            int fireLayer = LayerMask.NameToLayer("Fire");
            if (fireLayer >= 0)
                fireLayerMask = 1 << fireLayer;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, 1.5f, fireLayerMask);

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag(targetFireTag)) continue;

            FireScript fire = hit.GetComponent<FireScript>();
            if (fire != null && !fire.IsFullyExtinguished())
            {
                fire.ExtinguishStep(extinguishPower);

                if (extinguishSound != null && !extinguishSound.isPlaying)
                    extinguishSound.Play();

                Debug.Log($"🧯 抹布正在滅火：{hit.name}");
            }
        }
    }

    public void Use()
    {
        // 抹布沒有主動使用功能
    }

    public void OnPickup()
    {
        isHeld = true;

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    public void OnDrop()
    {
        isHeld = false;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }
}