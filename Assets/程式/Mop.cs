using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Mop : MonoBehaviour, IUsable, IPickupableItem
{
    private Rigidbody rb;
    private Collider col;

    [Header("Mop Settings")]
    public int usesRemaining = 5;
    public float effectiveRange = 1.5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void OnPickup()
    {
        rb.isKinematic = true;
        rb.detectCollisions = false;
        col.enabled = false;
        Debug.Log("🧽 Mop picked up!");
    }

    public void OnDrop()
    {
        rb.isKinematic = false;
        rb.detectCollisions = true;
        col.enabled = true;
        Debug.Log("🧽 Mop dropped!");
    }

    public void Use()
    {
        if (usesRemaining == 0)
        {
            Debug.Log("🧽 Mop is used up!");
            return;
        }

        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("❌ Main Camera not found!");
            return;
        }

        // 建立從鏡頭前方發射的射線
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, effectiveRange))
        {
            // 🔍 檢查射線到底打到什麼物件
            Debug.Log("🧽 射線擊中: " + hit.collider.name);

            FireScript fire = hit.collider.GetComponent<FireScript>();

            if (fire != null && !fire.IsFullyExtinguished())
            {
                // 🔹 改成漸進式滅火（而不是瞬間消失）
                fire.ExtinguishStep(50f); // 數值越高 → 滅火越快
                if (usesRemaining > 0) usesRemaining--;

                Debug.Log($"💧 Mop used on fire! Remaining uses: {usesRemaining}");
            }
            else
            {
                Debug.Log("🧽 Mop used, but no active fire found (沒有 FireScript 或已熄滅).");
            }
        }
        else
        {
            Debug.Log("🧽 Mop used, but nothing hit within range (沒有擊中任何物件).");
        }
    }
}

