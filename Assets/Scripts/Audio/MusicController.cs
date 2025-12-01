using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

namespace Audio
{
    public enum MusicPriority
    {
        Menu     = 0,
        Gameplay = 1,
        Chase    = 2,
        Minigame = 3
    }

    [Serializable]
    public struct MusicConfig
    {
        public SoundID menu;
        public SoundID gameplay;
        public SoundID chase;
        public SoundID minigame;

        [Header("Fade (seconds)")]
        [Range(0f, 5f)] public float fade;
    }

    /// <summary>
    /// Central music state machine with priority stack.
    /// </summary>
    public sealed class MusicController : MonoBehaviour
    {
        public static MusicController instance { get; private set; }

        [SerializeField]
        private MusicConfig _musicCfg = new MusicConfig()
        {
            fade = 05f
        };

        private readonly List<ActiveDirective> _directives = new List<ActiveDirective>();
        private SoundID? _currentlyPlaying;

        private void Awake()
        {
            if (instance != null && instance != this) { Destroy(gameObject); return; }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        #region Public API
        public void SetMenu()
        {
            UpsertBase(MusicPriority.Menu, _musicCfg.menu);
            Evaluate();
        }

        public void SetGameplay()
        {
            UpsertBase(MusicPriority.Gameplay, _musicCfg.gameplay);
            Evaluate();
        }

        public void SetChase(bool on)
        {
            ToggleOverlay(MusicPriority.Chase, _musicCfg.chase, on);
            Evaluate();
        }

        public void SetMinigame(bool on)
        {
            ToggleOverlay(MusicPriority.Minigame, _musicCfg.minigame, on);
            Evaluate();
        }
        #endregion

        #region Internals
        private void UpsertBase(MusicPriority prio, SoundID id)
        {
            var index = _directives.FindIndex(d => d.priority == prio);
            var dir = new ActiveDirective(prio, id, _musicCfg.fade, sticky:true);
            if (index >= 0) _directives[index] = dir; else _directives.Add(dir);
        }

        private void ToggleOverlay(MusicPriority prio, SoundID id, bool on)
        {
            var index = _directives.FindIndex(d => d.priority == prio);
            if (on)
            {
                var dir = new ActiveDirective(prio, id, _musicCfg.fade, sticky: false);
                if (index >= 0) _directives[index] = dir; else _directives.Add(dir);
            }
            else if (index >= 0)
            {
                _directives.RemoveAt(index);
            }
        }

        private void Evaluate()
        {
            if (_directives.Count == 0)
            {
                // nothing to play: fade out
                _currentlyPlaying = null;
                AudioBus.MusicFadeOut(_musicCfg.fade);
                return;
            }

            var chosen = _directives.OrderByDescending(d => d.priority).First();

            if (_currentlyPlaying.HasValue && _currentlyPlaying.Value.Equals(chosen.id))
                return;

            _currentlyPlaying = chosen.id;
            AudioBus.Music(chosen.id, fadeSeconds: chosen.fade, restartIfSame: false);
        }
        #endregion

        private readonly struct ActiveDirective
        {
            public readonly MusicPriority priority;
            public readonly SoundID id;
            public readonly float fade;
            public readonly bool sticky; // base tracks

            public ActiveDirective(MusicPriority p, SoundID i, float f, bool sticky)
            {
                priority = p; id = i; fade = f; this.sticky = sticky;
            }
        }
    }
}