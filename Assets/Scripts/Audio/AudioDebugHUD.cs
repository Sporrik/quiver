using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Audio;

[DefaultExecutionOrder(10000)]
public sealed class AudioDebugHUD : MonoBehaviour
{
    // ----- Config -----
    public KeyCode toggleKey = KeyCode.F9;

    // ----- State -----
    private bool _visible;
    private Rect _win = new Rect(20, 20, 520, 640);
    private Vector2 _scroll;
    private string _search = "";
    private float _master01, _music01, _sfx01;
    private bool _mute;

    // Cache
    private AudioManager _audioManager;
    private SoundLibrary _library;
    private Camera _cam;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (FindFirstObjectByType<AudioDebugHUD>() != null) return;
        var go = new GameObject("AudioDebugHUD");
        DontDestroyOnLoad(go);
        go.AddComponent<AudioDebugHUD>();
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        RefreshRefs();
        LoadPrefsIntoUI();
    }

    private void OnEnable() => RefreshRefs();

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey)) _visible = !_visible;

        if (_audioManager == null) _audioManager = AudioManager.instance;
        if (_library == null) _library = GetLibraryFromManager(_audioManager);
        if (_cam == null) _cam = Camera.main;
    }

    private void OnGUI()
    {
        if (!_visible) return;
        GUI.depth = 0;
        _win = GUILayout.Window(GetInstanceID(), _win, DrawWindow, "Audio Debug HUD");
    }

    private void DrawWindow(int id)
    {
        if (_audioManager == null)
        {
            GUILayout.Label("AudioManager not found. Add it to the scene.");
            if (GUILayout.Button("Retry")) RefreshRefs();
            GUI.DragWindow();
            return;
        }

        if (_library == null)
        {
            GUILayout.Label("SoundLibrary not assigned on AudioManager.");
            if (GUILayout.Button("Retry")) RefreshRefs();
            GUI.DragWindow();
            return;
        }

        // Global Controls
        GUILayout.Label("Global");
        GUILayout.BeginHorizontal();
        var newMute = GUILayout.Toggle(_mute, "Mute");
        if (newMute != _mute)
        {
            _mute = newMute;
            ApplyVolumesToManager();
        }

        if (GUILayout.Button("Save Volumes")) SavePrefsFromUI();
        if (GUILayout.Button("Reload Volumes")) { LoadPrefsIntoUI(); ApplyVolumesToManager(); }
        GUILayout.EndHorizontal();
        SliderLabeled("Master", ref _master01);
        SliderLabeled("Music", ref _music01);
        SliderLabeled("SFX", ref _sfx01);

        if (GUI.changed) ApplyVolumesToManager();

        GUILayout.Space(6);
        GUILayout.Label("Search");
        _search = GUILayout.TextField(_search);

        GUILayout.Space(6);
        GUILayout.Label("Entries");
        _scroll = GUILayout.BeginScrollView(_scroll, GUILayout.Height(420));

        var entries = _library.entries;
        IEnumerable<SoundLibrary.SoundEntry> filtered = entries;
        if (!string.IsNullOrWhiteSpace(_search))
        {
            string s = _search.Trim();
            filtered = entries.Where(e => e.id.ToString().IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        foreach (var e in filtered)
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label($"{e.id}  {(e.loop ? "[loop]" : "")}  vol:{e.volume:0.##}");

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Play SFX (2D)", GUILayout.Width(120)))
                _audioManager.PlaySFX(e.id);

            if (GUILayout.Button("Play SFX (3D)", GUILayout.Width(120)))
            {
                var pos = _cam ? _cam.transform.position + _cam.transform.forward * 5f : Vector3.zero;
                _audioManager.PlaySFX(e.id, pos);
            }

            if (GUILayout.Button("Play Music", GUILayout.Width(120)))
            {
                // Assumes the entry is a music clip if you choose to. If not, it will still play as BGM.
                _audioManager.PlayMusic(e.id, fadeSeconds: 0.75f);
            }

            if (GUILayout.Button("FadeOut Music", GUILayout.Width(120)))
                _audioManager.FadeOutCurrentMusic(0.5f);

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        GUILayout.EndScrollView();

        GUILayout.Space(6);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Stop Music"))
            _audioManager.FadeOutCurrentMusic(0.2f);
        if (GUILayout.Button("Ping Manager"))
            UnityEditorPing(_audioManager);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Close")) _visible = false;
        GUILayout.EndHorizontal();

        GUI.DragWindow();
    }

    // ----- Helpers -----

    private void SliderLabeled(string label, ref float v01)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label, GUILayout.Width(60));
        v01 = GUILayout.HorizontalSlider(v01, 0f, 1f);
        GUILayout.Label($"{Mathf.RoundToInt(v01 * 100f)}%", GUILayout.Width(50));
        GUILayout.EndHorizontal();
    }

    private void RefreshRefs()
    {
        _audioManager = AudioManager.instance;
        _library = GetLibraryFromManager(_audioManager);
        _cam = Camera.main;
    }

    private static SoundLibrary GetLibraryFromManager(AudioManager mgr)
    {
        if (mgr == null) return null;
        // Reflect private field _library; avoids adding a public getter.
        var fi = typeof(AudioManager).GetField("_library", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return fi?.GetValue(mgr) as SoundLibrary;
    }

    private void LoadPrefsIntoUI()
    {
        _master01 = PlayerPrefs.GetFloat("vol_master", 1f);
        _music01 = PlayerPrefs.GetFloat("vol_music", 1f);
        _sfx01 = PlayerPrefs.GetFloat("vol_sfx", 1f);
        _mute = PlayerPrefs.GetInt("vol_mute", 0) == 1;
    }

    private void SavePrefsFromUI()
    {
        PlayerPrefs.SetFloat("vol_master", _master01);
        PlayerPrefs.SetFloat("vol_music", _music01);
        PlayerPrefs.SetFloat("vol_sfx", _sfx01);
        PlayerPrefs.SetInt("vol_mute", _mute ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void ApplyVolumesToManager()
    {
        if (_audioManager == null) return;
        _audioManager.SetMasterVolumeDb(LinearToDb(_mute ? 0f : _master01));
        _audioManager.SetMusicVolumeDb(LinearToDb(_music01));
        _audioManager.SetSfxVolumeDb(LinearToDb(_sfx01));
    }

    private static float LinearToDb(float x)
    {
        if (x <= 0.0001f) return -80f;
        return Mathf.Log10(Mathf.Clamp01(x)) * 20f;
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private static void UnityEditorPing(UnityEngine.Object o)
    {
#if UNITY_EDITOR
        if (o != null) UnityEditor.EditorGUIUtility.PingObject(o);
#endif
    }
}