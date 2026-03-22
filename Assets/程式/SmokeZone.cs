using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SmokeZone : MonoBehaviour
{
    [Header("煙霧設定")]
    [Tooltip("煙霧的顏色 (建議使用深灰色)")]
    public Color smokeColor = new Color(0.1f, 0.1f, 0.1f, 1f); // 預設為深灰色

    [Tooltip("煙霧的濃度 (0.1 = 薄霧, 1.0 = 幾乎看不見)")]
    [Range(0.01f, 1f)]
    public float smokeDensity = 0.8f; // 【關鍵】調整這個數值來控制能見度

    // --- 【修改】移除攝影機背景相關變數 ---
    // private Camera playerCamera;
    // private CameraClearFlags originalClearFlags;
    // private Color originalBackgroundColor;

    // --- 【保留】儲存原始的全域霧氣設定 ---
    private float originalFogDensity;
    private bool originalFogState;
    private Color originalFogColor;
    private FogMode originalFogMode;
    // ---

    void Awake()
    {
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
            Debug.LogWarning("SmokeZone: " + gameObject.name + " 的 Collider 已被自動設為 'Is Trigger'。");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 檢查是不是玩家進入了
        if (other.CompareTag("Player"))
        {
            EnterSmoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 檢查是不是玩家離開了
        if (other.CompareTag("Player"))
        {
            ExitSmoke();
        }
    }

    // 進入煙霧時執行的效果
    private void EnterSmoke()
    {
        // 1. 儲存「原始」的全域霧氣設定
        originalFogState = RenderSettings.fog;
        originalFogDensity = RenderSettings.fogDensity;
        originalFogColor = RenderSettings.fogColor;
        originalFogMode = RenderSettings.fogMode;

        // 2. 套用「濃煙」設定
        RenderSettings.fog = true; // 強制開啟霧氣
        RenderSettings.fogColor = smokeColor; // 設定為您指定的煙霧顏色
        RenderSettings.fogMode = FogMode.Exponential; // 使用指數模式 (效果最濃)
        RenderSettings.fogDensity = smokeDensity; // 設定為您指定的超高濃度
    }

    // 離開煙霧時執行的效果
    private void ExitSmoke()
    {
        // 1. 恢復「原始」的全域霧氣設定
        RenderSettings.fog = originalFogState;
        RenderSettings.fogDensity = originalFogDensity;
        RenderSettings.fogColor = originalFogColor;
        RenderSettings.fogMode = originalFogMode;
    }
}