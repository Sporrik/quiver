using UnityEngine;
using UnityEngine.UI;
using Audio;

public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider _master;    // 0..1 linear
    [SerializeField] private Slider _music;     // 0..1 linear
    [SerializeField] private Slider _sfx;       // 0..1 linear
    [SerializeField] private Toggle _mute;

    private void Start()
    {
        _master.value = PlayerPrefs.GetFloat("vol_master", 1f);
        _music.value  = PlayerPrefs.GetFloat("vol_music", 1f);
        _sfx.value    = PlayerPrefs.GetFloat("vol_sfx", 1f);
        _mute.isOn    = PlayerPrefs.GetInt("vol_mute", 0) == 1;

        ApplyAll();
        _master.onValueChanged.AddListener(_ => ApplyAll());
        _music.onValueChanged.AddListener(_ => ApplyAll());
        _sfx.onValueChanged.AddListener(_ => ApplyAll());
        _mute.onValueChanged.AddListener(_ => ApplyAll());
    }

    private void OnDestroy()
    {
        _master.onValueChanged.RemoveAllListeners();
        _music.onValueChanged.RemoveAllListeners();
        _sfx.onValueChanged.RemoveAllListeners();
        _mute.onValueChanged.RemoveAllListeners();
    }

    private void ApplyAll()
    {
        if (AudioManager.instance == null) return;

        AudioManager.instance.SetMasterVolumeDb(LinearToDb(_mute.isOn ? 0f : _master.value));
        AudioManager.instance.SetMusicVolumeDb(LinearToDb(_music.value));
        AudioManager.instance.SetSfxVolumeDb(LinearToDb(_sfx.value));

        PlayerPrefs.SetFloat("vol_master", _master.value);
        PlayerPrefs.SetFloat("vol_music", _music.value);
        PlayerPrefs.SetFloat("vol_sfx", _sfx.value);
        PlayerPrefs.SetInt("vol_mute", _mute.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private static float LinearToDb(float value)
    {
        if (value <= 0.001f) return -80f;
        return Mathf.Log10(value) * 20f;
    }
}