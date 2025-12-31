using UnityEngine;

public class PulseMovement : MonoBehaviour
{
    [Header("Pulse Settings")]
    [SerializeField] private bool pulseX = false;
    [SerializeField] private bool pulseY = false;
    [SerializeField] private bool pulseZ = false;
    [SerializeField] private float amount = 1f;
    [SerializeField] private float speed = 1f;
    [SerializeField] private bool startPositive = true;

    private Vector3 _initialPosition;
    private float _offset;

    private void Start()
    {
        _initialPosition = transform.localPosition;

        _offset = startPositive ? 0f : amount;
    }

    private void Update()
    {
        float pulse = Mathf.PingPong(Time.time * speed, amount);

        pulse = startPositive ? pulse : amount - pulse;

        float x = pulseX ? pulse : 0f;
        float y = pulseY ? pulse : 0f;
        float z = pulseZ ? pulse : 0f;

        transform.localPosition = _initialPosition + new Vector3(x, y, z);
    }
}
