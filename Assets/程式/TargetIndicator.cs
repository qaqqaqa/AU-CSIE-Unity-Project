using UnityEngine;
using UnityEngine.UI;

public class TargetIndicator : MonoBehaviour
{
    public Transform target; // 要指向的目標 (火焰)
    public float borderSize = 100f; // 箭頭距離螢幕邊緣的距離

    private RectTransform arrowRectTransform;
    private Camera mainCamera;
    private Image arrowImage; // 我們在 Awake 中獲取 Image 元件

    void Awake()
    {
        // --- 【終極偵錯日誌】 ---
        Debug.Log("【日誌 A】TargetIndicator.cs 的 Awake() 方法已成功執行！腳本已喚醒。");

        arrowRectTransform = GetComponent<RectTransform>();
        arrowImage = GetComponent<Image>(); // 獲取 Image 元件
        mainCamera = Camera.main;

        // 檢查攝影機是否存在
        if (mainCamera == null)
        {
            Debug.LogError("【錯誤】在 TargetIndicator 中找不到主攝影機！請確認您的攝影機標籤 (Tag) 是否設為 'MainCamera'。");
        }
    }

    void Update()
    {
        // 如果沒有目標或目標已被摧毀，就隱藏箭頭並停止
        if (target == null)
        {
            if (arrowImage != null && arrowImage.enabled)
            {
                arrowImage.enabled = false;
            }
            return;
        }

        // 確保 mainCamera 不是空的
        if (mainCamera == null) return;

        // --- 核心邏輯 ---
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);
        bool isOffScreen = screenPos.z < 0 || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height;

        // --- 偵錯日誌 ---
        Debug.Log($"【日誌 5】TargetIndicator 狀態：目標 '{target.name}' 是否在畫面外? -> {isOffScreen} (螢幕座標: {screenPos})");

        if (isOffScreen)
        {
            if (arrowImage != null && !arrowImage.enabled)
            {
                arrowImage.enabled = true;
            }

            // ... (後續的位置和旋轉計算)
            Vector3 centerScreen = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            Vector3 direction = (screenPos - centerScreen).normalized;
            if (screenPos.z < 0) direction *= -1; // 如果在攝影機後面，方向要反轉

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            arrowRectTransform.localEulerAngles = new Vector3(0, 0, angle);

            Vector3 cappedScreenPos = screenPos;
            if (cappedScreenPos.z < 0) cappedScreenPos *= -1;

            cappedScreenPos.x = Mathf.Clamp(cappedScreenPos.x, borderSize, Screen.width - borderSize);
            cappedScreenPos.y = Mathf.Clamp(cappedScreenPos.y, borderSize, Screen.height - borderSize);

            arrowRectTransform.position = cappedScreenPos;
        }
        else
        {
            if (arrowImage != null && arrowImage.enabled)
            {
                arrowImage.enabled = false;
            }
        }
    }
}