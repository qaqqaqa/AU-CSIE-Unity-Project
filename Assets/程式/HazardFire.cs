using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(Collider))]
public class HazardFire : MonoBehaviour
{
    [Header("🔥 惡意火焰設定")]
    public float damagePerSecond = 40f; // 每秒扣 40 血 (如果總血量是 600，約 15 秒燒死)

    private void OnTriggerStay(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }

    // 空方法，防止滅火器報錯
    public void ExtinguishStep(float amount) { }
    public bool IsFullyExtinguished() { return false; }
}