using System;
using UnityEngine;

namespace UI
{
    [CreateAssetMenu(fileName = "UIData", menuName = "Scriptable Objects/UIData")]
    public class UIScriptableObject : ScriptableObject
    {
        [Header("Meters (0..100)")]
        [SerializeField] private float _poopMeter = 0f;
        [SerializeField] private float _peeMeter = 0f;
        [SerializeField] private float _hungryMeter = 0f;
        [SerializeField] private float _happinessMeter = 0f;
        [SerializeField] private float _staminaMeter = 100f;

        [Header("Mode")]
        [SerializeField] private bool _gameModeSinglePlayer;

        // ---------- Events ----------
        public event Action Changed;                        //any changes
        public event Action<float> PoopChanged, PeeChanged, HungryChanged, HappinessChanged, StaminaChanged;

        // ----- Getters (clamped) -----
        public bool GetGameModeSinglePlayer() => _gameModeSinglePlayer;

        public float GetPoop()      => Clamp(_poopMeter);
        public float GetPee()       => Clamp(_peeMeter);
        public float GetHungry()    => Clamp(_hungryMeter);
        public float GetHappiness() => Clamp(_happinessMeter);
        public float GetStamina()   => Clamp(_staminaMeter);

        // --- Increments / Setters ---
        public void IncrementPoop(float v)      => SetPoop(_poopMeter + v);
        public void IncrementPee(float v)       => SetPee(_peeMeter + v);
        public void IncrementHungry(float v)    => SetHungry(_hungryMeter + v);
        public void IncrementHappiness(float v) => SetHappiness(_happinessMeter + v);
        public void SetStamina(float v)         => SetStaminaInternal(v);

        public void SetSinglePlayer(bool gameMode) => _gameModeSinglePlayer = gameMode;

        // ---------- Resets ----------
        public void ResetPoop()      => SetPoop(0f);
        public void ResetPee()       => SetPee(0f);
        public void ResetHungry()    => SetHungry(0f);
        public void ResetHappiness() => SetHappiness(0f);
        public void ResetStamina()   => SetStaminaInternal(100f);

        // -------- Internals --------
        private static float Clamp(float value) => value < 0f ? 0f : (value > 100f ? 100f : value);

        private void SetPoop(float value)
        {
            float newValue = Clamp(value);
            if (Mathf.Approximately(newValue, _poopMeter)) return;
            _poopMeter = newValue;
            PoopChanged?.Invoke(_poopMeter);
            Changed?.Invoke();
        }

        private void SetPee(float value)
        {
            float newValue = Clamp(value);
            if (Mathf.Approximately(newValue, _peeMeter)) return;
            _peeMeter = newValue;
            PeeChanged?.Invoke(_peeMeter);
            Changed?.Invoke();
        }

        private void SetHungry(float value)
        {
            float newValue = Clamp(value);
            if (Mathf.Approximately(newValue, _hungryMeter)) return;
            _hungryMeter = newValue;
            HungryChanged?.Invoke(_hungryMeter);
            Changed?.Invoke();
        }

        private void SetHappiness(float value)
        {
            float newValue = Clamp(value);
            if (Mathf.Approximately(newValue, _happinessMeter)) return;
            _happinessMeter = newValue;
            HappinessChanged?.Invoke(_happinessMeter);
            Changed?.Invoke();
        }

        private void SetStaminaInternal(float value)
        {
            float newValue = Clamp(value);
            if (Mathf.Approximately(newValue, _staminaMeter)) return;
            _staminaMeter = newValue;
            StaminaChanged?.Invoke(_staminaMeter);
            Changed?.Invoke();
        }
    }
}