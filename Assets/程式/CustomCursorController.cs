using UnityEngine;

public class CustomCursorController : MonoBehaviour
{
    [Header("Cursor Settings")]
    // 改成 Texture2D 陣列以儲存多個游標圖片
    public Texture2D[] mouse_pic;

    // 使用 Vector2 來設定游標的寬度和高度
    public Vector2 cursorSize = new Vector2(64f, 64f);

    // 當前要顯示的游標索引 (0 = 預設, 1 = 互動, etc.)
    // 這個變數可以被其他腳本修改 (例如角色互動腳本)
    [Tooltip("Index of the cursor texture in the mouse_pic array to display. Can be changed by other scripts.")]
    public int currentCursorIndex = 0;

    [Header("Advanced Settings")]
    // 可選：設定游標的繪製深度（數字越小越靠後）
    public int guiDepth = 0;

    void Start()
    {
        // 核心設定：隱藏作業系統的游標
        Cursor.visible = false;
        // 為了讓滑鼠輸入能控制視角，系統游標必須被鎖定
        // 如果 PlayerMovement 或其他腳本已經設置了 Locked，這裡可以選擇不重複設置
        // 但為了確保，這裡再設置一次也無妨，只要沒有其他腳本隨後解除鎖定
        Cursor.lockState = CursorLockMode.Locked;

        // 注意：由於 Cursor.lockState = CursorLockMode.Locked;
        // Input.mousePosition 將不再代表游標的實際螢幕位置，
        // 它會保持在螢幕的邏輯中心。
        // 這正是我們將自訂游標繪製到畫面中心所需的行為。
    }

    void OnGUI()
    {
        // 只有當游標被鎖定時才繪製自訂游標
        // 因為當游標解除鎖定時（例如打開菜單），我們通常希望看到系統游標或者不繪製自訂游標
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            // 如果游標未鎖定，確保系統游標是可見的
            if (!Cursor.visible) Cursor.visible = true;
            return; // 不繪製自訂游標
        }
        else
        {
            // 如果游標被鎖定，確保系統游標是隱藏的
            if (Cursor.visible) Cursor.visible = false;
        }

        // --- 檢查游標陣列和索引的有效性 ---
        bool canDraw = false;
        Texture2D cursorToDraw = null; // 將要繪製的游標圖片

        if (mouse_pic == null || mouse_pic.Length == 0)
        {
            Debug.LogError("CustomCursorController: Mouse Pic array is not assigned or is empty in the Inspector!");
        }
        else if (currentCursorIndex < 0 || currentCursorIndex >= mouse_pic.Length)
        {
            // 如果索引無效，嘗試使用索引 0 作為後備
            Debug.LogWarning($"CustomCursorController: currentCursorIndex ({currentCursorIndex}) is out of bounds. Attempting to use index 0.");
            if (mouse_pic.Length > 0 && mouse_pic[0] != null)
            {
                currentCursorIndex = 0; // 重設為 0
                cursorToDraw = mouse_pic[0];
                canDraw = true;
            }
            else
            {
                Debug.LogError("CustomCursorController: Cannot draw cursor, index 0 is also invalid or array is empty.");
            }
        }
        else if (mouse_pic[currentCursorIndex] == null)
        {
            Debug.LogError($"CustomCursorController: Texture at index {currentCursorIndex} is null! Attempting to fall back to index 0.");
            // 如果當前索引的貼圖為 null，嘗試使用索引 0 作為後備
            if (currentCursorIndex != 0 && mouse_pic.Length > 0 && mouse_pic[0] != null)
            {
                currentCursorIndex = 0; // 重設為 0
                cursorToDraw = mouse_pic[0];
                canDraw = true;
            }
            else
            {
                Debug.LogError("CustomCursorController: Cannot draw cursor, index 0 is also invalid or array is empty when current is null.");
            }
        }
        else
        {
            // 一切正常，允許繪製
            cursorToDraw = mouse_pic[currentCursorIndex];
            canDraw = true;
        }

        // 如果不能繪製，直接返回，不執行後續繪圖程式碼
        if (!canDraw || cursorToDraw == null)
        {
            return;
        }

        // --- 繪製游標到畫面中心 ---
        // 設定繪製深度
        GUI.depth = guiDepth;

        // 計算繪製區域 (Rect)，將游標圖片繪製到螢幕中心
        // 螢幕中心點是 (Screen.width / 2, Screen.height / 2)
        // 為了讓圖片中心對齊螢幕中心，需要將其寬度和高度的一半減去
        Rect cursorRect = new Rect(
            (Screen.width / 2f) - (cursorSize.x / 2f), // X 位置 (螢幕寬度的一半減去游標寬度的一半)
            (Screen.height / 2f) - (cursorSize.y / 2f),// Y 位置 (螢幕高度的一半減去游標高度的一半)
            cursorSize.x,                               // 寬度
            cursorSize.y                                // 高度
        );

        // 在計算好的中心位置繪製指定索引的游標貼圖
        GUI.DrawTexture(cursorRect, cursorToDraw);
    }

    // 當腳本被禁用時，恢復預設游標和鎖定狀態
    void OnDisable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None; // 恢復為自由模式
    }

    // 應用程式退出時，確保游標恢復正常
    void OnApplicationQuit()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // --- 公開方法 (供其他腳本呼叫來改變游標) ---
    /// <summary>
    /// Sets the index for the cursor texture to display.
    /// </summary>
    /// <param name="index">The index in the mouse_pic array.</param>
    public void SetCursorIndex(int index)
    {
        // 基本的邊界檢查，防止設置無效索引
        if (mouse_pic != null && index >= 0 && index < mouse_pic.Length)
        {
            currentCursorIndex = index;
        }
        else
        {
            Debug.LogWarning($"Attempted to set invalid cursor index: {index}. Valid range is 0 to {(mouse_pic != null ? mouse_pic.Length - 1 : -1)}");
            // 可以選擇設置為預設值 0
            if (mouse_pic != null && mouse_pic.Length > 0 && mouse_pic[0] != null)
            {
                currentCursorIndex = 0; // 強制設置為 0
                Debug.LogWarning("Falling back to cursor index 0 due to invalid index.");
            }
        }
    }
}