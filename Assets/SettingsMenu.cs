using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider volumeSlider;

    private const string VOL_KEY = "settings.volume";

    private void Awake()
    {
        float v = PlayerPrefs.GetFloat(VOL_KEY, 0.8f);
        if (volumeSlider != null)
        {
            volumeSlider.value = v;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
        ApplyVolume(v);
    }

    private void OnDestroy()
    {
        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
    }

    private void OnVolumeChanged(float v)
    {
        ApplyVolume(v);
        PlayerPrefs.SetFloat(VOL_KEY, v);
    }

    private void ApplyVolume(float v)
    {
        AudioListener.volume = Mathf.Clamp01(v);
    }
}
