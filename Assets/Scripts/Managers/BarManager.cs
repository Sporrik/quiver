using Gameplay.AI;
using System;
using UnityEngine;

namespace UI
{
    [DisallowMultipleComponent]
    public sealed class BarManager : MonoBehaviour
    {
        public enum NeedType { Poop, Hungry, Pee }

        #region Inspector
        [Header("Player")]
        [SerializeField] private PlayerController _playerController;

        [Header("UI/Data")]
        [SerializeField] private UIScriptableObject _scriptableObject;

        [Header("Random Singleplayer Ticks")]
        [SerializeField, Min(0f)] private float _timeToGetRandomEvent = 1f;
        [SerializeField, Min(0f)] private float _amountToIncreaseBar = 5f;
        [SerializeField] private bool _singlePlayerOverride = false;

        [Header("Happiness (probabilistic)")]
        [Tooltip("Seconds between happiness RNG checks.")]
        [SerializeField, Min(0.05f)] private float _happinessTickInterval = 1f;
        [Tooltip("How Much Happiness increases when RNG check succeeds.")]
        [SerializeField, Min(0f)] private float _happinessIncrement = 3f;
        [Tooltip("Base success chance per check when needs are empty (0..1).")]
        [SerializeField, Range(0f, 1f)] private float _happinessBaseChance = 0.05f;
        [Tooltip("Success chance per check when needs are full (0..1).")]
        [SerializeField, Range(0f, 1f)] private float _happinessChanceAtFull = 0.5f;

        [Header("Cry/Alert")]
        [SerializeField, Min(0f)] private float _cryRange = 12f;
        [SerializeField] private LayerMask _guardMask;
        [SerializeField, Min(0f)] private float _cryCooldown = 2f;
        #endregion

        #region Events
        public event Action<BarManager> OnBabyCrying;
        public event Action<NeedType> OnNeedFilled;
        #endregion

        #region State
        private float _eventTimer;
        private float _cryCooldownTimer;
        private bool _isSinglePlayer;

        private float _happinessTimer;

        private bool _poopCapped;
        private bool _peeCapped;
        private bool _hungryCapped;

        private readonly Collider[] _guardHits = new Collider[16];
        #endregion

        #region Unity
        private void Awake()
        {
            if (_playerController == null) Debug.LogError($"{nameof(BarManager)}: PlayerController missing.", this);
            if (_scriptableObject == null) Debug.LogError($"{nameof(BarManager)}: UIScriptableObject missing.", this);

            _isSinglePlayer = _singlePlayerOverride || (_scriptableObject != null && _scriptableObject.GetGameModeSinglePlayer());
        }

        private void OnEnable()
        {
            if (_playerController != null)
            {
                _playerController.OnStaminaChanged += HandleStaminaChanged;
            }
        }

        private void OnDisable()
        {
            if (_playerController != null)
            {
                _playerController.OnStaminaChanged -= HandleStaminaChanged;
            }
        }

        private void Update()
        {
            if (_scriptableObject == null) return;

            float dt = Time.deltaTime;
            _eventTimer += dt;
            _happinessTimer += dt;

            if (_cryCooldownTimer > 0f) _cryCooldownTimer -= dt;

            if (_isSinglePlayer && _eventTimer >= _timeToGetRandomEvent)
            {
                _eventTimer -= _timeToGetRandomEvent;
                RandomBarIncrease();
            }

            TickHappiness();
            DebouncedNeedEvents();

            if (_scriptableObject.GetHappiness() >= 100f && _cryCooldownTimer <= 0f)
            {
                _cryCooldownTimer = _cryCooldown;
                OnBabyCrying?.Invoke(this);
                AlertGuardsInRange();
            }
        }
        #endregion

        #region Handlers
        private void HandleStaminaChanged(float stamina, float max)
        {
            _scriptableObject?.SetStamina(stamina);
        }
        #endregion

        #region Logic
        private void RandomBarIncrease()
        {
            int pick = UnityEngine.Random.Range(1, 4);
            switch (pick)
            {
                case 1: _scriptableObject.IncrementPoop(_amountToIncreaseBar); break;
                case 2: _scriptableObject.IncrementHungry(_amountToIncreaseBar); break;
                case 3: _scriptableObject.IncrementPee(_amountToIncreaseBar); break;
                default: Debug.LogWarning("RandomBarIncrease: unexpected branch."); break;
            }
        }

        private void DebouncedNeedEvents()
        {
            float poop   = _scriptableObject.GetPoop();
            float pee    = _scriptableObject.GetPee();
            float hungry = _scriptableObject.GetHungry();

            if (poop > 100f)
            {
                if (!_poopCapped)
                {
                    _poopCapped = true;
                    OnNeedFilled?.Invoke(NeedType.Poop);
                }
            }
            else if (_poopCapped) _poopCapped = false;

            if (pee > 100f)
            {
                if (!_peeCapped)
                {
                    _peeCapped = true;
                    OnNeedFilled?.Invoke(NeedType.Pee);
                }
            }
            else if (_peeCapped) _peeCapped = false;

            if (hungry > 100f)
            {
                if (!_hungryCapped)
                {
                    _hungryCapped = true;
                    OnNeedFilled?.Invoke(NeedType.Hungry);
                }
            }
            else if (_hungryCapped) _hungryCapped = false;
        }

        private void TickHappiness()
        {
            if (_happinessTimer >= _happinessTickInterval)
            {
                _happinessTimer -= _happinessTickInterval;

                float poop01   = _scriptableObject.GetPoop() * 0.01f;
                float hungry01 = _scriptableObject.GetHungry() * 0.01f;
                float pee01    = _scriptableObject.GetPee() * 0.01f;

                float needsAvg = (poop01 + hungry01 + pee01) / 3f;
                float chance = Mathf.Lerp(_happinessBaseChance, _happinessChanceAtFull, needsAvg);

                if (UnityEngine.Random.value < Mathf.Clamp01(chance)) _scriptableObject.IncrementHappiness(_happinessIncrement);
            }
        }

        private void AlertGuardsInRange()
        {
            int count = Physics.OverlapSphereNonAlloc(_playerController.transform.position,
                                                      _cryRange,
                                                      _guardHits,
                                                      _guardMask,
                                                      QueryTriggerInteraction.Ignore);

            for (int i = 0; i < count; i++)
            {
                var c = _guardHits[i];
                if (!c) continue;

                if (c.TryGetComponent<IGuardAlertable>(out var alertable))
                {
                    alertable.OnCryAlert(transform.position, _cryRange);
                }
            }
        }
        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(_playerController.transform.position, _cryRange);
        }
#endif
    }
}