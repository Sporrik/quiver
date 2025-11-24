using Gameplay.AI;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using TMPro.SpriteAssetUtilities;
using UnityEngine;

namespace UI
{
    [DisallowMultipleComponent]
    public sealed class BarManager : MonoBehaviour
    {
        public enum NeedType { Poop, Hungry, Pee }
        public GameObject FlashingBar;
        private FlashEffect flashBarScript;

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
        [SerializeField, UnityEngine.Range(0f, 1f)] private float _happinessBaseChance = 0.05f;
        [Tooltip("Success chance per check when needs are full (0..1).")]
        [SerializeField, UnityEngine.Range(0f, 1f)] private float _happinessChanceAtFull = 0.5f;

        [Header("Cry/Alert")]
        [SerializeField, Min(0f)] private float _cryRange = 12f;
        [SerializeField] private LayerMask _guardMask;
        [SerializeField, Min(0f)] private float _cryCooldown = 2f;



        [Header("AnimationTwitch")]

        [SerializeField] private float _progressTimerPee;
        [SerializeField] private float _progressTimerPoo;
        [SerializeField] private float _progressTimerHunger;




        [SerializeField] private TextMeshProUGUI _currentTwitchTextPoo;
        [SerializeField] private TextMeshProUGUI _currentTwitchTextPee;
        [SerializeField] private TextMeshProUGUI _currentTwitchTextHunger;

        [SerializeField] private Vector3 _startPosition;
        [SerializeField] private Vector3 _endPositionPee;
        [SerializeField] private Vector3 _endPositionPoo;
        [SerializeField] private Vector3 _endPositionHunger;


        List<string> _pooNamesList = new List<string> { };
        List<string> _hungerNamesList = new List<string> { };
        List<string> _peeNamesList = new List<string> { };

        private const float NAMEDELAY = 0.5f;
        [SerializeField] private float _addNameDelay;



        [SerializeField] private TwitchGameManager _gameManager;


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

        private void Start()
        {
            flashBarScript = FlashingBar.GetComponent<FlashEffect>();
        }

        private void Update()
        {
            GetNamesTwitch(); // gets and animates the names
            

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

        #region TwitchNameAnimation
        private void GetNamesTwitch()
        {

            _addNameDelay += Time.deltaTime;

            AddToList(_gameManager.GetUserNameHungerCommand(), _hungerNamesList);
            AddToList(_gameManager.GetUserNamePeeCommand(), _peeNamesList);
            AddToList(_gameManager.GetUserNamePoopCommand(), _pooNamesList);

            AnimateText(_endPositionPee, _peeNamesList, _progressTimerPee, _currentTwitchTextPee);
            AnimateText(_endPositionPoo, _pooNamesList, _progressTimerPoo, _currentTwitchTextPoo);
            AnimateText(_endPositionHunger, _hungerNamesList, _progressTimerHunger, _currentTwitchTextHunger);
        }

        private void AnimateText(Vector3 endPosition, List<string> list, float progressTimer, TextMeshProUGUI text)
        {
            if (list[0] != null)
            {
                float progress = progressTimer / 2;
                text.text = list[0];   // set name
                text.transform.position = Vector3.Slerp(_startPosition, endPosition, progress); // move UI

                progressTimer += Time.deltaTime;
                if (progressTimer > 2) // RESET UI
                {
                    progressTimer = 0;
                    list.RemoveAt(0);  // animation done
                }

            }
        }

        private void AddToList(string name, List<string> list)
        {
            if (name != null && _addNameDelay > NAMEDELAY) // 0.5 delay to send name in the twitch script
            {
                _addNameDelay = 0;
                list.Add(name);
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

            if (poop >= 100f)
            {
                if (!_poopCapped)
                {
                    _poopCapped = true;
                    OnNeedFilled?.Invoke(NeedType.Poop);
                }
            }
            else if (_poopCapped) _poopCapped = false;

            if (pee >= 100f)
            {
                if (!_peeCapped)
                {
                    _peeCapped = true;
                    OnNeedFilled?.Invoke(NeedType.Pee);
                }
            }
            else if (_peeCapped) _peeCapped = false;

            if (hungry >= 100f)
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
            if (_happinessTimer < _happinessTickInterval) return;
            
            _happinessTimer -= _happinessTickInterval;

            float increment = (_poopCapped ? 1 : 0) + (_peeCapped ? 1 : 0) + (_hungryCapped ? 1 : 0);

            Debug.Log(increment);
            Debug.Log(_poopCapped);

            _scriptableObject.IncrementHappiness(increment);
            
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
                    alertable.OnCryAlert(_playerController.transform.position);
                    flashBarScript.isTurnedOn = true;
                }
                else
                {
                    flashBarScript.isTurnedOn = false;
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