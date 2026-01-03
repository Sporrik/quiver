using UnityEngine;
using UnityEngine.UI;
using Audio;
using System.Collections;

public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider _master;    // 0..1 linear
    [SerializeField] private Slider _music;     // 0..1 linear
    [SerializeField] private Slider _sfx;       // 0..1 linear
    [SerializeField] private Slider _ambience;  // 0..1 linear
    [SerializeField] private Toggle _mute;
    [SerializeField] private float _saveDebounceSeconds = 0.5f;

    private Coroutine _saveCo;

    private void Start()
    {
        // Init UI from prefs
        _master.SetValueWithoutNotify(Audio.AudioSettings.Master01);
        _music.SetValueWithoutNotify(Audio.AudioSettings.Music01);
        _sfx.SetValueWithoutNotify(Audio.AudioSettings.Sfx01);
        _ambience.SetValueWithoutNotify(Audio.AudioSettings.Ambience01);
        _mute.SetIsOnWithoutNotify(Audio.AudioSettings.Mute);

        // Live apply on change
        _master.onValueChanged.AddListener(OnChanged);
        _music.onValueChanged.AddListener(OnChanged);
        _sfx.onValueChanged.AddListener(OnChanged);
        _ambience.onValueChanged.AddListener(OnChanged);
        _mute.onValueChanged.AddListener(_ => OnChanged(0f)); // reuse

        // Initial apply to mixer
        Audio.AudioSettings.ApplyToMixer(AudioManager.instance);
    }

    private void OnDestroy()
    {
        if (_master != null)   _master.onValueChanged.RemoveListener(OnChanged);
        if (_music != null)    _music.onValueChanged.RemoveListener(OnChanged);
        if (_sfx != null)      _sfx.onValueChanged.RemoveListener(OnChanged);
        if (_ambience != null) _ambience.onValueChanged.RemoveListener(OnChanged);
        if (_mute != null)     _mute.onValueChanged.RemoveAllListeners();
    }

    private void OnChanged(float _)
    {
        // Update prefs immediately
        Audio.AudioSettings.Master01 = _master.value;
        Audio.AudioSettings.Music01 = _music.value;
        Audio.AudioSettings.Sfx01 = _sfx.value;
        Audio.AudioSettings.Ambience01 = _ambience.value;
        Audio.AudioSettings.Mute = _mute.isOn;

        // Apply to mixer live
        Audio.AudioSettings.ApplyToMixer(AudioManager.instance);

        // Debounced disk save
        if (_saveCo != null) StopCoroutine(_saveCo);
        _saveCo = StartCoroutine(CoDebouncedSave());
    }

    private IEnumerator CoDebouncedSave()
    {
        yield return new WaitForSeconds(_saveDebounceSeconds);
        PlayerPrefs.Save();
        _saveCo = null;
    }
}