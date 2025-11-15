using Gameplay.AI;
using System;
using UnityEngine;

namespace Gameplay
{
    [DisallowMultipleComponent]
    public sealed class BarManager : MonoBehaviour
    {
        #region Inspector
        [Header("Player")]
        [SerializeField] private PlayerController _playerController;

        [Header("UI/Data")]
        [SerializeField] private UIScriptableObject _scriptableObject;

        [Header("Random Singleplayer Ticks")]
        [SerializeField, Min(0f)] private float _timeToGetRandomEvent = 1f;
        [SerializeField, Min(0f)] private float _amountToIncreaseBar = 5f;
        [SerializeField] private bool _singlePlayerOverride = false;

        [Header("Unhappiness Gain")]
        [SerializeField, Min(0f)] private float _timeToGetAngry = 3f;
        [SerializeField, Min(0f)] private float _amountToIncreaseHappiness = 5f;

        [Header("Cry/Alert")]
        [SerializeField, Min(0f)] private float _cryRange = 12f;
        [SerializeField] private LayerMask _guardMask;
        [SerializeField, Min(0f)] private float _cryCooldown = 2f;
        #endregion

        #region Events
        public event Action<BarManager> OnBabyCrying;
        #endregion

        #region State
        private float _eventTimer;
        private float _angryTimer;
        private float _cryCooldownTimer;
        private bool _isSinglePlayer;
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
            _angryTimer += dt;
            if (_cryCooldownTimer > 0f) _cryCooldownTimer -= dt;

            if (_isSinglePlayer && _eventTimer >= _timeToGetRandomEvent)
            {
                _eventTimer -= _timeToGetRandomEvent;
                RandomBarIncrease();
            }

            bool anyNeedMaxed = _scriptableObject.GetHungry() >= 100f
                             || _scriptableObject.GetPoop() >= 100f
                             || _scriptableObject.GetPee() >= 100f;

            if (anyNeedMaxed && _angryTimer >= _timeToGetAngry)
            {
                _angryTimer = 0f;
                _scriptableObject.IncrementHapiness(_amountToIncreaseHappiness);
            }

            if (_scriptableObject.GetHapiness() >= 100f && _cryCooldownTimer <= 0f)
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

        private void AlertGuardsInRange()
        {
            int count = Physics.OverlapSphereNonAlloc(transform.position,
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
            Gizmos.DrawWireSphere(transform.position, _cryRange);
        }
#endif
    }
}