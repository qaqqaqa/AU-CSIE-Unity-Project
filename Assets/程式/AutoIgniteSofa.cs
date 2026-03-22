using UnityEngine;

public class AutoIgniteSofa : MonoBehaviour
{
    [Header("沙發模型")]
    public GameObject initialSofa; // 初始沙發模型
    public GameObject burnedSofa;  // 燒毀沙發模型

    [Header("火焰效果")]
    public GameObject fireEffect;  // 火焰效果
    public GameObject smokeEffect; // 煙霧效果

    [Header("起火設置")]
    public float igniteDelay = 10f; // 自動起火延遲時間 (已修改為 10 秒)

    private bool isBurning = false; // 標記是否正在燃燒

    void Start()
    {
        // 初始化沙發狀態
        initialSofa.SetActive(true);
        burnedSofa.SetActive(false);

        // 確保火焰和煙霧效果初始是關閉的
        if (fireEffect != null) fireEffect.SetActive(false);
        if (smokeEffect != null) smokeEffect.SetActive(false);

        // 設置自動起火，使用 Invoke 在 igniteDelay 秒後呼叫 StartBurning 方法
        Invoke("StartBurning", igniteDelay);
    }

    // 當達到延遲時間時呼叫此方法開始燃燒
    void StartBurning()
    {
        // 只有當沙發還沒有在燃燒時才開始燃燒
        if (!isBurning)
        {
            isBurning = true;

            // 切換沙發模型：隱藏初始沙發，顯示燒毀沙發
            initialSofa.SetActive(false);
            burnedSofa.SetActive(true);

            // 開啟火焰和煙霧效果
            if (fireEffect != null) fireEffect.SetActive(true);
            if (smokeEffect != null) smokeEffect.SetActive(true);

            Debug.Log("沙發起火了！"); // 可選：加入 Debug 訊息
        }
    }

    // --- 新增的功能：讓玩家可以呼叫此方法來熄滅火焰 ---
    // 這個方法可以從其他腳本呼叫，例如玩家的互動腳本
    public void ExtinguishFire()
    {
        // 只有當沙發正在燃燒時才執行熄滅操作
        if (isBurning)
        {
            Debug.Log("玩家正在嘗試熄滅火焰..."); // 可選：加入 Debug 訊息

            // 關閉火焰和煙霧效果
            if (fireEffect != null) fireEffect.SetActive(false);
            if (smokeEffect != null) smokeEffect.SetActive(false);

            isBurning = false; // 標記為不再燃燒

            // 注意：這裡沒有再切換回初始沙發模型，保持燒毀狀態。
            // 如果需要切換回去或其他行為，可以在這裡添加。

            Debug.Log("火焰已熄滅。"); // 可選：加入 Debug 訊息
        }
        else
        {
            Debug.Log("沙發沒有在燃燒。"); // 可選：加入 Debug 訊息
        }
    }

    // 原本的 StopBurning 方法現在由 ExtinguishFire 呼叫或直接替換
    // private void StopBurning()
    // {
    //     // 關閉火焰和煙霧效果
    //     if (fireEffect != null) fireEffect.SetActive(false);
    //     if (smokeEffect != null) smokeEffect.SetActive(false);
    //     isBurning = false;
    //     Debug.Log("火焰自動熄滅了 (此功能已移除)。");
    // }
}