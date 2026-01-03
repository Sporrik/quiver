using UnityEngine;

namespace Audio
{
    public static class AudioBus
    {
        private static AudioManager _audioManager => AudioManager.instance;

        public static void Sfx(SoundID id, float volume = 1f)
        {
            if (_audioManager == null) return;
            _audioManager.PlaySFX(id, null, volume);
        }

        public static void SfxAt(SoundID id, Vector3 position, float volume = 1f)
        {
            if (_audioManager == null) return;
            _audioManager.PlaySFX(id, position, volume);
        }

        public static void Music(SoundID id, float fadeSeconds = 0.5f, bool restartIfSame = false)
        {
            if (_audioManager == null) return;
            _audioManager.PlayMusic(id, fadeSeconds, restartIfSame);
        }

        public static void MusicFadeOut(float fadeSeconds = 0.5f)
        {
            if (_audioManager == null) return;
            _audioManager.FadeOutCurrentMusic(fadeSeconds);
        }

        public static void Ambience(SoundID id, float fadeSeconds = 0.5f, bool restartIfSame = false)
        {
            if (_audioManager == null) return;
            _audioManager.PlayAmbience(id, fadeSeconds, restartIfSame);
        }
        
        public static void AmbienceStop(float fadeSeconds = 0.25f)
        {
            if (_audioManager == null) return;
            _audioManager.StopAmbience(fadeSeconds);
        }
    }
}