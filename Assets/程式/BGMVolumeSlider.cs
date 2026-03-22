using UnityEngine;
using UnityEngine.UI;

public class BGMVolumeSlider : MonoBehaviour
{
    public Slider volumeSlider;

    void Start()
    {
        if (volumeSlider != null && SoundManager.Instance != null)
        {
            // 初始化滑桿值
            volumeSlider.value = SoundManager.Instance.GetBGMVolume();
            // 監聽滑桿變化
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
    }

    private void OnVolumeChanged(float value)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetBGMVolume(value);
        }
    }
}
