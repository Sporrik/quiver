using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    [CreateAssetMenu(fileName = "SoundLibrary", menuName = "Scriptable Objects/SoundLibrary")]
    public class SoundLibrary : ScriptableObject
    {
        [Serializable]
        public struct SoundEntry
        {
            public SoundID id;
            public AudioClip clip;
            [Range(0f, 2f)] public float volume;        // default 1.0
            public bool loop;
            public AudioMixerGroup mixerGroup;
        }

        [Header("Entries")]
        public SoundEntry[] entries;

        public bool TryGet(SoundID id, out SoundEntry entry)
        {
            for (int i = 0; i < entries.Length; i++)
            {
                if (entries[i].id == id)
                {
                    entry = entries[i];
                    return true;
                }
            }

            entry = default;
            return false;
        }
    }
}
