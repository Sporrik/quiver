using System;
using UnityEngine;

namespace UI
{
    [CreateAssetMenu(fileName = "UIData", menuName = "Scriptable Objects/UIData")]
    public class UIScriptableObject : ScriptableObject
    {
        [Serializable]
        public struct Defaults
        {
            [Range(0, 100)] public float Poop;
            [Range(0, 100)] public float Pee;
            [Range(0, 100)] public float Hungry;
            [Range(0, 100)] public float Happiness;
            [Range(0, 100)] public float Stamina;
        }

        [Header("Defaults")]
        [SerializeField]
        private Defaults _defaults = new Defaults
        {
            Poop = 0f,
            Pee = 0f,
            Hungry = 0f,
            Happiness = 0f,
            Stamina = 100f,

        };

        [Header("Mode")]
        [SerializeField] private bool _gameModeSinglePlayer;

        // ------- Live Values -------
        private float _poop;
        private float _pee;
        private float _hungry;
        private float _happiness;
        private float _stamina;

        // ---------- Events ----------
        public event Action Changed;                        //any changes
        public event Action<float> PoopChanged, PeeChanged, HungryChanged, HappinessChanged, StaminaChanged;

        // ---- Getters (clamped) ----
        public bool GetGameModeSinglePlayer() => _gameModeSinglePlayer;

        public float GetPoop() => Clamp(_poop);
        public float GetPee() => Clamp(_pee);
        public float GetHungry() => Clamp(_hungry);
        public float GetHappiness() => Clamp(_happiness);
        public float GetStamina() => Clamp(_stamina);

        // --- Increments / Setters ---
        public void IncrementPoop(float v) => SetPoop(_poop + v);
        public void IncrementPee(float v) => SetPee(_pee + v);
        public void IncrementHungry(float v) => SetHungry(_hungry + v);
        public void IncrementHappiness(float v) => SetHappiness(_happiness + v);
        public void SetStamina(float v) => SetStaminaInternal(v);

        public void SetSinglePlayer(bool gameMode) => _gameModeSinglePlayer = gameMode;

        // ---------- Resets ----------
        public void ResetPoop() => SetPoop(0f);
        public void ResetPee() => SetPee(0f);
        public void ResetHungry() => SetHungry(0f);
        public void ResetHappiness() => SetHappiness(0f);
        public void ResetStamina() => SetStaminaInternal(100f);

        public void ResetAllToDefaults()
        {
            bool any = false;
            any |= SetPoopSilent(_defaults.Poop);
            any |= SetPeeSilent(_defaults.Pee);
            any |= SetHungrySilent(_defaults.Hungry);
            any |= SetHappinessSilent(_defaults.Happiness);
            any |= SetStaminaSilent(_defaults.Stamina);
        }

        // -------- helpers --------
        private static float Clamp(float value) => value < 0f ? 0f : (value > 100f ? 100f : value);

        private void SetPoop(float value)            { if (SetPoopSilent(value)) Changed?.Invoke(); }
        private void SetPee(float value)             { if (SetPeeSilent(value)) Changed?.Invoke(); }
        private void SetHungry(float value)          { if (SetHungrySilent(value)) Changed?.Invoke(); }
        private void SetHappiness(float value)       { if (SetHappinessSilent(value)) Changed?.Invoke(); }
        private void SetStaminaInternal(float value) { if (SetStaminaSilent(value)) Changed?.Invoke(); }


        private bool SetPoopSilent(float value)
        {
            float newValue = Clamp(value);
            if (Mathf.Approximately(newValue, _poop)) return false;
            _poop = newValue;
            PoopChanged?.Invoke(_poop);
            return true;
        }

        private bool SetPeeSilent(float value)
        {
            float newValue = Clamp(value);
            if (Mathf.Approximately(newValue, _pee)) return false;
            _pee = newValue;
            PeeChanged?.Invoke(_pee);
            return true;
        }

        private bool SetHungrySilent(float value)
        {
            float newValue = Clamp(value);
            if (Mathf.Approximately(newValue, _hungry)) return false;
            _hungry = newValue;
            HungryChanged?.Invoke(_hungry);
            return true;
        }

        private bool SetHappinessSilent(float value)
        {
            float newValue = Clamp(value);
            if (Mathf.Approximately(newValue, _happiness)) return false;
            _happiness = newValue;
            HappinessChanged?.Invoke(_happiness);
            return true;
        }

        private bool SetStaminaSilent(float value)
        {
            float newValue = Clamp(value);
            if (Mathf.Approximately(newValue, _stamina)) return false;
            _stamina = newValue;
            StaminaChanged?.Invoke(_stamina);
            return true;
        }
#if UNITY_EDITOR
        [ContextMenu("Apply Defaults Now (Editor)")]
        private void EditorApplyDefaults() => ResetAllToDefaults();
#endif
    }
}

