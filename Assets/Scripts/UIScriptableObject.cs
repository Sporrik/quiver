using UnityEngine;

[CreateAssetMenu(fileName = "UIData", menuName = "Scriptable Objects/UIData")]

public class UIScriptableObject : ScriptableObject
{
    [SerializeField] private float _hapinessMeter;
    [SerializeField] private float _poopMeter;
    [SerializeField] private float _hungryMeter;
    [SerializeField] private float _peeMeter;
    [SerializeField] private float _staminaMeter;

   // [SerializeField] PlayerController _playerController;
    

    public float GetPoop()
    {

        _poopMeter = Mathf.Max(_poopMeter, 0);
        return Mathf.Min(_poopMeter, 100);
    }
    public float GetHapiness()
    {
        _hapinessMeter = Mathf.Max(_hapinessMeter, 0);
        return Mathf.Min(_hapinessMeter, 100);
    }
    public float GetHungry()
    {
        _hungryMeter = Mathf.Max(_hungryMeter, 0);
        return Mathf.Min(_hungryMeter, 100);
    }
    public float GetPee()
    {
        _peeMeter = Mathf.Max(_peeMeter, 0);
        return Mathf.Min(_peeMeter, 100);
    }
    public float GetStamina()
    {
        _staminaMeter = Mathf.Max(_staminaMeter, 0);
        return Mathf.Min(_staminaMeter, 100);
    }
}
