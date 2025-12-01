using UnityEngine;
using UnityEngine.UI;
using Audio;
using System.Collections;

public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider _master;    // 0..1 linear
    [SerializeField] private Slider _music;     // 0..1 linear
    [SerializeField] private Slider _sfx;       // 0..1 linear
    [SerializeField] private Toggle _mute;
    [SerializeField] private float _saveDebounceSeconds = 0.5f;

    private Coroutine _saveCo;

    private void Start()
    {
        // Init UI from prefs
        _master.SetValueWithoutNotify(Audio.AudioSettings.Master01);
        _music.SetValueWithoutNotify(Audio.AudioSettings.Music01);
        _sfx.SetValueWithoutNotify(Audio.AudioSettings.Sfx01);
        _mute.SetIsOnWithoutNotify(Audio.AudioSettings.Mute);

        // Live apply on change
        _master.onValueChanged.AddListener(OnChanged);
        _music.onValueChanged.AddListener(OnChanged);
        _sfx.onValueChanged.AddListener(OnChanged);
        _mute.onValueChanged.AddListener(_ => OnChanged(0f)); // reuse

        // Initial apply to mixer
        Audio.AudioSettings.ApplyToMixer(AudioManager.instance);
    }

    private void OnDestroy()
    {
        _master.onValueChanged.RemoveListener(OnChanged);
        _music.onValueChanged.RemoveListener(OnChanged);
        _sfx.onValueChanged.RemoveListener(OnChanged);
        _mute.onValueChanged.RemoveAllListeners();
    }

    private void OnChanged(float _)
    {
        // Update prefs immediately
        Audio.AudioSettings.Master01 = _master.value;
        Audio.AudioSettings.Music01 = _music.value;
        Audio.AudioSettings.Sfx01 = _sfx.value;
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