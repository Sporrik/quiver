using UnityEngine;
using System.Collections;


public class IncorrectFood : MonoBehaviour
{
    [SerializeField] private Renderer _targetRenderer;
    [SerializeField] private Color _flashColor = Color.red;
    [SerializeField] private float _flashDuration = 0.1f;
    [SerializeField] private float _opacity = 0.1f;
    [SerializeField] private float _shakeIntensity = 0.1f; // Intensity of the shake
    [SerializeField] private int _shakeFrequency = 10; // Number of shakes per second
    [SerializeField] private bool _enableShake = false;
    [SerializeField] private bool _flashingFlash = false;

    private Material[] _materials;
    private Color[] _originalColors;
    private Vector3 _originalPosition;
    private bool _hasBeenInitialized = false;

    void Start()
    {
        // Creates individual instances of ALL materials
        _materials = _targetRenderer.materials;

        _originalColors = new Color[_materials.Length];
        for (int i = 0; i < _materials.Length; i++)
            _originalColors[i] = _materials[i].color;

        // Store the original position of the object
        _originalPosition = transform.position;
    }

    public void TriggerFlash()
    {
        StopAllCoroutines();
        StartCoroutine(Flash());

        if (_enableShake)
            StartCoroutine(Shake());
    }

    IEnumerator Flash()
    {
        if(_hasBeenInitialized == false)
            _originalPosition = transform.position;


        if (!_flashingFlash)
        {
            _hasBeenInitialized = true;
            // Single blended flash, then restore
            for (int i = 0; i < _materials.Length; i++)
            {
                // Interpolate between the original color and the flash color
                _materials[i].color = Color.Lerp(_originalColors[i], _flashColor, _opacity);
            }

            yield return new WaitForSeconds(_flashDuration);

            // Restore the original color
            for (int i = 0; i < _materials.Length; i++)
            {
                _materials[i].color = _originalColors[i];
            }

            yield break;
        }

        // Flashing flash: toggle back and forth between flash color and original for the duration
        float elapsed = 0f;

        // Use up to ~10 toggles across the duration, but not faster than 0.05s.
        float interval = Mathf.Clamp(_flashDuration / 5f, 0.05f, _flashDuration);

        bool showFlash = true;

        while (elapsed < _flashDuration)
        {
            // Apply either the blended flash color or the original color
            for (int i = 0; i < _materials.Length; i++)
            {
                _materials[i].color = showFlash
                    ? Color.Lerp(_originalColors[i], _flashColor, _opacity)
                    : _originalColors[i];
            }

            float wait = Mathf.Min(interval, _flashDuration - elapsed);
            elapsed += wait;
            yield return new WaitForSeconds(wait);

            showFlash = !showFlash;
        }

        // Ensure original colors are restored
        for (int i = 0; i < _materials.Length; i++)
        {
            _materials[i].color = _originalColors[i];
        }
    }

    IEnumerator Shake()
    {
        float elapsedTime = 0f;

        while (elapsedTime < _flashDuration)
        {
            // Calculate a small offset for the Y position
            float offsetY = Mathf.Sin(elapsedTime * _shakeFrequency * Mathf.PI * 2) * _shakeIntensity;

            // Apply the offset to the object's position
            transform.position = _originalPosition + new Vector3(0, offsetY, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Restore the original position
        transform.position = _originalPosition;
    }
}
