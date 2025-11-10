using System;
using UnityEngine;

[CreateAssetMenu(fileName = "UIData", menuName = "Scriptable Objects/UIData")]

public class UIScriptableObject : ScriptableObject
{
    [SerializeField] private float _happinessMeter;
    [SerializeField] private float _poopMeter;
    [SerializeField] private float _hungryMeter;
    [SerializeField] private float _peeMeter;
    [SerializeField] private float _staminaMeter;


    public float GetPoop()
    {
        _poopMeter = Mathf.Max(_poopMeter, 0);
        return Mathf.Min(_poopMeter, 100);
    }
    public float GetHapiness()
    {
        _happinessMeter = Mathf.Max(_happinessMeter, 0);
        return Mathf.Min(_happinessMeter, 100);
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

    //setters (changes by Warre)
    public void IncrementPoop(float incrementValue)
    {
        _poopMeter += incrementValue;
    }
    public void IncrementHapiness(float incrementValue)
    {
        _happinessMeter += incrementValue;
    }
    public void IncrementHungry(float incrementValue)
    {
        _hungryMeter += incrementValue;
    }
    public void IncrementPee(float incrementValue)
    {
        _peeMeter += incrementValue;
    }
    public void SetStamina(float setValue)
    {
        _staminaMeter = setValue;
    }

    public void ResetPoop()
    {
        _poopMeter = 0;
    }
    public void ResetHapiness()
    {
        _happinessMeter = 0;
    }
    public void ResetHungry()
    {
        _hungryMeter = 0;
    }
    public void ResetPee()
    {
        _peeMeter = 0;
    }
    public void ResetStamina()
    {
        _staminaMeter = 100;
    }
}
