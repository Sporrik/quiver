using System;
using TwitchIntegration;
using UnityEngine;

namespace UI
{
    [CreateAssetMenu(fileName = "UIData", menuName = "Scriptable Objects/UIData")]
    public class UIScriptableObject : ScriptableObject
    {
        // ------- Vieuwer balance Settings -------
        [Header("Balance - Scaling")]
        [SerializeField] private float _balancePeePooHunger = 1f;
        private float _viewerCount = 0f;

        // ------------------------------

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

        //[Header("Animation")]
        //[SerializeField] protected float _delay = 2f;


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



        public void IncrementPoop(float v)
        {
            if (_gameModeSinglePlayer)
                SetPoop(_poop + v);
            else
            {
                SetPoop(_poop + (v * _balancePeePooHunger / (_viewerCount + 1)));
                Debug.Log("TotalPoop: " + (_poop + (v * _balancePeePooHunger / (_viewerCount + 1))));
                Debug.Log("Increment: " + (v * _balancePeePooHunger / (_viewerCount + 1)));
                Debug.Log("ViewerCount: " + _viewerCount + "Balance: " + _balancePeePooHunger + "v: " + v) ;

            }
        }

        public void IncrementPee(float v)
        {
            if (_gameModeSinglePlayer)
                SetPee(_pee + v);
            else
                SetPee(_pee + (v * _balancePeePooHunger / (_viewerCount + 1)));
        }
        public void IncrementHungry(float v)
        {
            if (_gameModeSinglePlayer)
                SetHungry(_hungry + v);
            else
                SetHungry(_hungry + (v * _balancePeePooHunger / (_viewerCount + 1)));
        }

        public void IncrementHappiness(float v) => SetHappiness(_happiness + v);
        public void SetStamina(float v) => SetStaminaInternal(v);

        public void SetSinglePlayer(bool gameMode) => _gameModeSinglePlayer = gameMode;

        // ---------- Resets ----------
        public void ResetPoop() => SetPoop(0f);
        public void ResetPee() => SetPee(0f);
        public void ResetHungry() => SetHungry(0f);
        public void ResetHappiness() => SetHappiness(0f);
        public void ResetStamina() => SetStaminaInternal(100f);

        public void SetTwitchVieuwerCount(float value)
        {
            _viewerCount = value;
        }
        public void SetBalancePeePooHunger(float value)
        {
            _balancePeePooHunger = value;
        }
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

        private void SetPoop(float value) { if (SetPoopSilent(value)) Changed?.Invoke(); }
        private void SetPee(float value) { if (SetPeeSilent(value)) Changed?.Invoke(); }
        private void SetHungry(float value) { if (SetHungrySilent(value)) Changed?.Invoke(); }
        private void SetHappiness(float value) { if (SetHappinessSilent(value)) Changed?.Invoke(); }
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

