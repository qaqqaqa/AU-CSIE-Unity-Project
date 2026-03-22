using UnityEngine;

// 附加音效播放器和 Animator 組件
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))] // 確保有 Animator 組件
public class OpenDown : MonoBehaviour
{
    public AudioClip openAudio; // 開啟音效
    public AudioClip downAudio; // 關閉音效

    private bool isOpen = false; // 使用更清晰的變數名表示門的開啟狀態
    public float interactionCooldown = 0.5f; // 互動冷卻時間 (稍微縮短一些，你也可以調整)
    private float lastInteractionTime = -Mathf.Infinity; // 上次互動的時間，初始設為負無窮大確保第一次可以互動

    Animator anim;
    AudioSource source;
    

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        source = GetComponent<AudioSource>();

        // 檢查組件是否存在
        if (anim == null)
        {
            Debug.LogError("OpenDown: Animator component not found on the door! Please add an Animator component.", this);
        }
        if (source == null)
        {
            Debug.LogError("OpenDown: AudioSource component not found on the door! Please add an AudioSource component.", this);
        }

        // 確保 AudioSource 停在原點，避免 Start 時播放預設音效
        if (source != null)
        {
            source.Stop();
        }
    }

    // 這個方法會被 PlayerMoveWithInteraction 腳本呼叫
    public void Switch() // 將方法設為 public
    {
        // 檢查是否在冷卻中
        if (Time.time < lastInteractionTime + interactionCooldown)
        {
            // Debug.Log("Door interaction on cooldown."); // 如果需要，可以取消註解這行來除錯冷卻
            return;
        }

        // 切換門的開關狀態
        isOpen = !isOpen;
        lastInteractionTime = Time.time; // 更新上次互動時間

        Debug.Log($"Door is now attempting to {(isOpen ? "open" : "close")}"); // 除錯訊息，顯示嘗試開關

        // 觸發動畫
        // 推薦使用 Animator Controller 中的 boolean 參數來控制狀態轉換
        if (anim != null)
        {
            // 假設你的 Animator Controller 中有一個名為 "IsOpen" 的 boolean 參數
            anim.SetBool("IsOpen", isOpen);
            Debug.Log($"Setting Animator parameter 'IsOpen' to {isOpen}"); // 除錯訊息，顯示參數設定

            // 如果你沒有使用 boolean 參數，而是直接 Play 動畫片段，請使用你動畫片段的名稱
            // 注意：直接 Play() 可能會中斷當前動畫，推薦使用參數和轉換
            // if (isOpen) anim.Play("你的開啟動畫片段名稱");
            // else anim.Play("你的關閉動畫片段名稱");
        }

        // 播放音效
        if (source != null)
        {
            if (isOpen)
            {
                if (openAudio != null) source.PlayOneShot(openAudio);
                else Debug.LogWarning("OpenDown: Open audio clip is not assigned!", this); // 警告：開啟音效未指定
            }
            else
            {
                if (downAudio != null) source.PlayOneShot(downAudio);
                else Debug.LogWarning("OpenDown: Close audio clip is not assigned!", this); // 警告：關閉音效未指定
            }
        }
    }

    // 你可能需要一個 Update 方法來根據動畫狀態或時間更新門的位置/旋轉
    // 如果你完全依賴 Animator 控制門的 Transform，則可能不需要這個 Update 方法
    /*
    void Update()
    {
        // 範例：如果不用 Animator Controller 控制 Transform，而是在腳本中控制
        // 這部分需要根據你的門的具體動畫實現來編寫
    }
    */
}