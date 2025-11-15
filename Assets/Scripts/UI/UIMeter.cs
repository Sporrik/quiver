using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [DisallowMultipleComponent]
    public sealed class UIMeter : MonoBehaviour
    {
        [Header("Binding")]
        [SerializeField] private UIScriptableObject _uiData;
        [SerializeField] private MeterType _meterType;

        [Header("Target")]
        [SerializeField] private Image _image;
        [SerializeField] private bool _invert = true;
    
        private void Reset()
        {
            _image = GetComponent<Image>();
        }
    
        private void Awake()
        {
            if (_uiData = null) Debug.LogError($"{nameof(UIMeter)}: UIScriptableObject not assigned.", this);
            if (_image == null) _image = GetComponent<Image>();
            if (_image = null) Debug.LogError($"{nameof(UIMeter)}: Image  not assigned.", this);

            if (_image != null && _image.type != Image.Type.Filled)
            {
                _image.type = Image.Type.Filled;
                Debug.LogWarning($"{nameof(UIMeter)}: Image.type was not 'Filled'. Auto-set to Filled.", this);
            }
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
            float v01 = Mathf.Clamp01(value01_100 * 0.01f);
            _image.fillAmount = _invert ? (1f - v01) : v01;
        }
    }
}
