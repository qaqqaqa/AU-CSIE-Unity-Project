using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("🎵 背景音樂")]
    public AudioSource bgmSource;

    [Range(0f, 1f)]
    public float bgmVolume = 1f; // 當前音量

    [Header("🎚️ 滾輪控制設定")]
    public float scrollVolumeStep = 0.05f; // 每次滾輪滾動改變的音量量
    public bool enableScrollControl = true; // 可切換是否啟用

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 讀取上次的音量設定
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);

        if (bgmSource != null)
        {
            bgmSource.volume = bgmVolume;
            bgmSource.loop = true;
            bgmSource.playOnAwake = true;

            if (!bgmSource.isPlaying)
                bgmSource.Play();
        }
    }

    void Update()
    {
        if (enableScrollControl)
            HandleScrollVolume();
    }

    /// <summary>
    /// 處理滑鼠滾輪音量控制
    /// </summary>
    private void HandleScrollVolume()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.01f) // 確保真的有滾動
        {
            bgmVolume += scroll > 0 ? scrollVolumeStep : -scrollVolumeStep;
            bgmVolume = Mathf.Clamp01(bgmVolume);
            ApplyVolumeChange();
        }
    }

    /// <summary>
    /// 套用音量變化並保存
    /// </summary>
    private void ApplyVolumeChange()
    {
        if (bgmSource != null)
            bgmSource.volume = bgmVolume;

        PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        PlayerPrefs.Save();

        Debug.Log($"🎚️ 當前音量: {Mathf.RoundToInt(bgmVolume * 100)}%");
    }

    /// <summary>
    /// 設定背景音樂音量（0~1）
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        ApplyVolumeChange();
    }

    /// <summary>
    /// 取得目前音量
    /// </summary>
    public float GetBGMVolume()
    {
        return bgmVolume;
    }
}
