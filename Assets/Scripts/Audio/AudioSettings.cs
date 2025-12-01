using UnityEngine;
using System;

namespace Audio
{
    /// <summary>
    /// Centralized acces to audio prefs. Fires Changed on updates.
    /// </summary>
    public static class AudioSettings
    {
        public static event Action Changed;

        public const string KeyMaster = "vol_master";
        public const string KeyMusic  = "vol_music";
        public const string KeySfx    = "vol_sfx";
        public const string KeyMute   = "vol_mute";

        public static float Master01
        {
            get => PlayerPrefs.GetFloat(KeyMaster, 1f);
            set { PlayerPrefs.SetFloat(KeyMaster, Mathf.Clamp01(value)); Changed?.Invoke(); }
        }

        public static float Music01
        {
            get => PlayerPrefs.GetFloat(KeyMusic, 1f);
            set { PlayerPrefs.SetFloat(KeyMusic, Mathf.Clamp01(value)); Changed?.Invoke(); }
        }

        public static float Sfx01
        {
            get => PlayerPrefs.GetFloat(KeySfx, 1f);
            set { PlayerPrefs.SetFloat(KeySfx, Mathf.Clamp01(value)); Changed?.Invoke(); }
        }

        public static bool Mute
        {
            get => PlayerPrefs.GetInt(KeyMute, 0) == 1;
            set { PlayerPrefs.SetInt(KeyMute, value ? 1 : 0); Changed?.Invoke(); }
        }

        public static float LinearToDb(float x)
        {
            if (x <= 0.0001f) return -80f;
            return Mathf.Log10(Mathf.Clamp01(x)) * 20f;
        }

        public static void ApplyToMixer(AudioManager mgr)
        {
            if (mgr == null) return;
            mgr.SetMasterVolumeDb(LinearToDb(Mute ? 0f : Master01));
            mgr.SetMusicVolumeDb(LinearToDb(Music01));
            mgr.SetSfxVolumeDb(LinearToDb(Sfx01));
        }
    }
}