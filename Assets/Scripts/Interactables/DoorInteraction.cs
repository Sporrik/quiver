using UnityEngine;

public class DoorInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform _doorHinge;
    [SerializeField] private float _openAngle = 85f;
    private bool _isOpen = false;
    private Quaternion _closedRotation;
    private Quaternion _openRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_doorHinge == null)
        {
            _doorHinge = transform;
        }

        _closedRotation = _doorHinge.localRotation;
        _openRotation = Quaternion.Euler(_doorHinge.localEulerAngles + new Vector3(0, _openAngle, 0));
    }

    public void Interact()
    {
        if (_isOpen)
        {
            _doorHinge.localRotation = _closedRotation;
            _isOpen = false;
        } 
        else
        {
            _doorHinge.localRotation = _openRotation;
            _isOpen = true;
        }
    }
}
