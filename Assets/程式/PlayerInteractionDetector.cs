using UnityEngine;

public class PlayerInteractionDetector : MonoBehaviour
{
    [Header("偵測設定")]
    [Tooltip("最遠可以偵測到多遠的火焰")]
    public float detectRange = 5f;
    [Tooltip("偵測頻率 (秒/次)，避免每幀偵測太耗效能")]
    public float detectInterval = 0.1f;
    public LayerMask detectLayer; // 建議設定為只偵測 "Interactable" 或 "Fire" 圖層

    private float timer;
    private FireScript currentLookingAtFire;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= detectInterval)
        {
            DetectFire();
            timer = 0f;
        }
    }

    void DetectFire()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // 發射射線偵測前方
        if (Physics.Raycast(ray, out hit, detectRange, detectLayer))
        {
            // 嘗試獲取 FireScript 元件
            FireScript fire = hit.collider.GetComponent<FireScript>();

            if (fire != null && !fire.IsFullyExtinguished())
            {
                // 如果看到的是新的火焰，或者之前沒在看火焰
                if (currentLookingAtFire != fire)
                {
                    currentLookingAtFire = fire;
                    // 顯示提示
                    string hint = fire.GetExtinguishHint();
                    if (InteractionHintUI.Instance != null)
                    {
                        InteractionHintUI.Instance.ShowHint(hint);
                    }
                }
                return; // 偵測到火焰就返回，保持顯示
            }
        }

        // 如果射線沒打到東西，或者打到的不是火焰
        if (currentLookingAtFire != null)
        {
            currentLookingAtFire = null;
            // 隱藏提示
            if (InteractionHintUI.Instance != null)
            {
                InteractionHintUI.Instance.HideHint();
            }
        }
    }
}
