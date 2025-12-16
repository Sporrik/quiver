using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public sealed class UIMeter : MonoBehaviour
    {
        [Tooltip("Auto: infer type from GameObject name (poop/pee/hungry/happiness/stamina).")]
        [SerializeField] private MeterType _meterType;
        
        [SerializeField] private bool _invert = true;

        private bool _resetWorld = true;

        //[Header("DifficultyValue")]
        //[SerializeField] private float _balance = 1f;

        [Header("Animation")]
        [SerializeField] private float _delayAddSeconds = 2f;
       // [SerializeField] private TwitchGameManager _twitchGameManager;


        private UIScriptableObject _uiData;
        private Image _image;
    
        private void Awake()
        {
            if (_image == null) _image = GetComponent<Image>();
            if (_image != null && _image.type != Image.Type.Filled)
                _image.type = Image.Type.Filled;

            _uiData = UIMeterDataProvider.Shared;
            if (_uiData == null)
                Debug.LogError($"{nameof(UIMeter)} on '{name}': No UIData found. " +
                               "Add UIMeterDataProvider in scene.", this);
        }

        private void OnEnable()
        {
            if (_uiData == null) return;

            switch (_meterType)
            {
                case MeterType.Poop:      _uiData.PoopChanged      += OnValue; break;
                case MeterType.Pee:       _uiData.PeeChanged       += OnValue; break;
                case MeterType.Hungry:    _uiData.HungryChanged    += OnValue; break;
                case MeterType.Happiness: _uiData.HappinessChanged += OnValue; break;
                case MeterType.Stamina:   _uiData.StaminaChanged   += OnValue; break;
                default:
                    _meterType = InferTypeFromName(gameObject.name);
                    goto case MeterType.Poop;
            }

            OnValue(ReadNow());
        }

        private void OnDisable()
        {
            if (_uiData == null) return;

            switch (_meterType)
            {
                case MeterType.Poop:      _uiData.PoopChanged      -= OnValue; break;
                case MeterType.Pee:       _uiData.PeeChanged       -= OnValue; break;
                case MeterType.Hungry:    _uiData.HungryChanged    -= OnValue; break;
                case MeterType.Happiness: _uiData.HappinessChanged -= OnValue; break;
                case MeterType.Stamina:   _uiData.StaminaChanged   -= OnValue; break;
            }
        }

        private float ReadNow()
        {
            return _meterType switch
            {
                MeterType.Poop      => _uiData.GetPoop(),
                MeterType.Pee       => _uiData.GetPee(),
                MeterType.Hungry    => _uiData.GetHungry(),
                MeterType.Happiness => _uiData.GetHappiness(),
                MeterType.Stamina   => _uiData.GetStamina(),
                _ => 0f
            };
        }

        private void OnValue(float value01_100)
        {
            if (_image == null) return;

            if (_uiData.GetGameModeSinglePlayer() == false && !_resetWorld) // if twitch enabled animate delay in adding value
            {
                
                if(gameObject.name.Contains("HappinessBar") || gameObject.name.Contains("StaminaBar"))
                {
                    Debug.Log("Add without delay: " + gameObject.name);
                    AddValue(value01_100);
                    return;
                }
                StartCoroutine(WaitAndPrint(value01_100)); // delay for game feel ?!
            }
            else
            {
                _resetWorld = false;
                float v01 = Mathf.Clamp01(value01_100 * 0.01f);
                _image.fillAmount = _invert ? (1f - v01) : v01;
            }
        }
        private IEnumerator WaitAndPrint(float value01_100)
        {
            //Debug.Log("Add1");
            yield return new WaitForSeconds(_delayAddSeconds);  // delay 2 seconds for animation
            AddValue(value01_100);

        }
        private void AddValue(float value)
        {
            float v01 = Mathf.Clamp01(value * 0.01f);
            _image.fillAmount = _invert ? (1f - v01) : v01;
        }

        private static MeterType InferTypeFromName(string goName)
        {
            string n = goName.ToLowerInvariant();
            if (n.Contains("poop"))         return MeterType.Poop;
            if (n.Contains("pee"))          return MeterType.Pee;
            if (n.Contains("hungry") ||
                n.Contains("hunger"))       return MeterType.Hungry;
            if (n.Contains("happiness") ||
                n.Contains("happy"))        return MeterType.Happiness;
            if (n.Contains("stamina"))      return MeterType.Stamina;
            return MeterType.Poop;
        }
    }
}
