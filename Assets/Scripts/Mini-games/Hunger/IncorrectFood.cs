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

    private Material[] _materials;
    private Color[] _originalColors;
    private Vector3 _originalPosition;

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
        StartCoroutine(Shake());
    }

    IEnumerator Flash()
    {
        // Blend the flash color with the original color
        for (int i = 0; i < _materials.Length; i++)
        {
            // Interpolate between the original color and the flash color
            _materials[i].color = Color.Lerp(_originalColors[i], _flashColor, _opacity); // 0.5f is the blend factor
        }

        yield return new WaitForSeconds(_flashDuration);

        // Restore the original color
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
