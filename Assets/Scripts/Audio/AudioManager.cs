using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    /// <summary>
    /// Centralized audio facade. Keeps global control and avoids duplicated AudioSource logic
    /// </summary>
    public sealed class AudioManager : MonoBehaviour
    {
        public static AudioManager instance {  get; private set; }

        [Header("Setup")]
        [SerializeField] private SoundLibrary _library;
        [SerializeField] private AudioMixer _mixer;
        [SerializeField] private AudioMixerGroup _mixerMusic;
        [SerializeField] private AudioMixerGroup _mixerSfx;

        [Header("SFX Pool")]
        [SerializeField, Min(1f)] private int _prewarmSfxSources = 8;
        [SerializeField, Min(1f)] private int _maxSfxSources = 24;

        [Header("Music")]
        [SerializeField] private float defaultMusicFade = 0.5f;

        private readonly Queue<AudioSource> _available = new Queue<AudioSource>();
        private readonly HashSet<AudioSource> _inUse = new HashSet<AudioSource>();
        private AudioSource _musicA;
        private AudioSource _musicB;
        private readonly HashSet<SoundID> _missingLogged = new HashSet<SoundID>();

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            // Prewarm SFX Pool
            for (int i = 0; i < _prewarmSfxSources; i++)
                _available.Enqueue(CreatePooledSource());

            // Music sources (dedicated, never pooled)
            _musicA = CreateDedicatedSource("MusicA", _mixerMusic, spatialBlend: 0f);
            _musicB = CreateDedicatedSource("MusicB", _mixerMusic, spatialBlend: 0f);
        }

        private AudioSource CreatePooledSource()
        {
            var go = new GameObject("SFX_AudioSource");
            go.transform.SetParent(transform);
            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            src.spatialBlend = 0f;
            src.outputAudioMixerGroup = _mixerSfx;
            return src;
        }

        private AudioSource CreateDedicatedSource(string name, AudioMixerGroup group, float spatialBlend)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform);
            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            src.spatialBlend = spatialBlend;
            src.outputAudioMixerGroup = group;
            return src;
        }

        private AudioSource GetSfxSource()
        {
            if (_available.Count > 0)
            {
                var source = _available.Dequeue();
                _inUse.Add(source);
                return source;
            }

            if (_inUse.Count < _maxSfxSources)
            {
                var source = CreatePooledSource();
                _inUse.Add(source);
                return source;
            }

            return CreatePooledSource();
        }

        private IEnumerator ReturnAfterFinish(AudioSource src)
        {
            while (src != null && src.isPlaying) yield return null;

            if (src == null) yield break;

            if (_inUse.Contains(src))
            {
                Destroy(src.gameObject);
                yield break;
            }

            src.clip = null;
            src.outputAudioMixerGroup = _mixerSfx;
            src.transform.SetParent(transform);
            _inUse.Remove(src);
            _available.Enqueue(src);
        }

        #region Public API
        public void PlaySFX(SoundID id, Vector3? worldPos = null, float volumeScale = 1f)
        {
            if (_library == null)
            {
                Debug.LogWarning($"[AudioManager] No SoundLibrary assigned.");
                return;
            }

            if (!_library.TryGet(id, out var entry) || entry.clip == null)
            {
                if (!_missingLogged.Contains(id))
                {
                    Debug.LogWarning($"[AudioManager] Missing clip for {id}");
                    _missingLogged.Add(id);
                }
                return;
            }

            var src = GetSfxSource();
            ApplyWorldPos(src, worldPos);
            ConfigureAndPlay(src, entry, volumeScale);
            if (!entry.loop) StartCoroutine(ReturnAfterFinish(src));
        }

        public void PlaySFXClip(AudioClip clip, Vector3? worldPos = null, float volume = 1f, AudioMixerGroup groupOverride = null)
        {
            if (clip == null) return;
            var src = GetSfxSource();
            ApplyWorldPos(src, worldPos);
            src.clip = clip;
            src.loop = false;
            src.outputAudioMixerGroup = groupOverride != null ? groupOverride : _mixerSfx;

            src.volume = Mathf.Clamp01(volume);
            src.spatialBlend = worldPos.HasValue ? 1f : 0f;
            src.minDistance = 1f;
            src.maxDistance = 30f;

            src.Play();
            StartCoroutine(ReturnAfterFinish(src));
        }

        public void PlayMusic(SoundID id, float fadeSeconds = -1f, bool restartIfSame = false)
        {
            if (!_library.TryGet(id, out var entry) || entry.clip == null)
            {
                if (!_missingLogged.Contains(id))
                {
                    Debug.LogWarning($"[AudioManager] Missing music clip for {id}");
                    _missingLogged.Add(id);
                }
                return;
            }

            fadeSeconds = fadeSeconds < 0 ? defaultMusicFade : fadeSeconds;

            // If same track already active, optionally skip
            if (!restartIfSame && (_musicA.clip == entry.clip && _musicA.isPlaying ||
                                   _musicB.clip == entry.clip && _musicB.isPlaying))
                return;

            // Swap: pick inactive as target-in
            var inSrc = !_musicA.isPlaying ? _musicA : _musicB;
            var outSrc = inSrc == _musicA ? _musicB : _musicA;

            inSrc.clip = entry.clip;
            inSrc.loop = true;
            inSrc.outputAudioMixerGroup = entry.mixerGroup != null ? entry.mixerGroup : _mixerMusic;
            inSrc.volume = 0f;
            inSrc.Play();

            StopCoroutine(nameof(CoCrossfade));
            StartCoroutine(CoCrossfade(inSrc, outSrc, fadeSeconds, targetVol: Mathf.Clamp01(entry.volume)));
        }

        public void FadeOutCurrentMusic(float fadeSeconds = 0.25f)
        {
            StopCoroutine(nameof(CoCrossfade));
            if (_musicA.isPlaying) StartCoroutine(CoFade(_musicA, 0f, fadeSeconds));
            if (_musicB.isPlaying) StartCoroutine(CoFade(_musicB, 0f, fadeSeconds));

        }

        public void SetMasterVolumeDb(float db) => SetDb("MasterVolume", db);
        public void SetMusicVolumeDb(float db)  => SetDb("MusicVolume", db);
        public void SetSfxVolumeDb(float db)    => SetDb("SFXVolume", db);

        public void Mute(bool muted)
        {
            _mixer.SetFloat("MasterVolume", muted ? -80f : 0f);
        }
        #endregion

        #region Helpers
        private void ConfigureAndPlay(AudioSource src, SoundLibrary.SoundEntry entry, float volumeScale)
        {
            src.clip = entry.clip;
            src.loop = entry.loop;
            src.outputAudioMixerGroup = entry.mixerGroup != null ? entry.mixerGroup : _mixerSfx;
            src.volume = Mathf.Clamp01(entry.volume * volumeScale);

            if (src.spatialBlend > 0f)
            {
                src.minDistance = 1f;
                src.maxDistance = 30f;
            }

            src.Play();
        }

        private void ApplyWorldPos(AudioSource src, Vector3? pos)
        {
            if (pos.HasValue)
            {
                src.transform.position = pos.Value;
                src.spatialBlend = 1f;
            }
            else
            {
                src.transform.localPosition = Vector3.zero;
                src.spatialBlend = 0f;
            }
        }

        private IEnumerator CoCrossfade(AudioSource inSrc, AudioSource outSrc, float seconds, float targetVol)
        {
            float t = 0f;
            float startOut = outSrc.isPlaying ? outSrc.volume : 0f;

            while (t < seconds)
            {
                t += Time.unscaledDeltaTime;
                float k = seconds <= 0f ? 1f : Mathf.Clamp01(t / seconds);
                inSrc.volume = Mathf.Lerp(0f, targetVol, k);
                if (outSrc.isPlaying) outSrc.volume = Mathf.Lerp(startOut, 0f, k);
                yield return null;
            }

            inSrc.volume = targetVol;
            if (outSrc.isPlaying)
            {
                outSrc.Stop();
                outSrc.clip = null;
            }
        }

        private IEnumerator CoFade(AudioSource src, float target, float seconds)
        {
            float t = 0f;
            float start = src.volume;

            while (t < seconds)
            {
                t += Time.unscaledDeltaTime;
                float k = seconds <= 0f ? 1 : Mathf.Clamp01(t / seconds);
                src.volume = Mathf.Lerp(start, target ,k);
                yield return null;
            }

            src.volume = target;
            if (Mathf.Approximately(target, 0f) && src.isPlaying) src.Stop();
        }

        private void SetDb(string param, float db)
        {
            db = Mathf.Clamp(db, -80f, 6f);
            _mixer.SetFloat(param, db);
        }
        #endregion
    }
}