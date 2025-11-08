using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private float _attackRange;
    [SerializeField] private LayerMask _enemyLayer;

    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactionRange = 2f;
    [SerializeField] private LayerMask _interactionLayer;

    private IInteractable _currentInteractable;

    public void Interact(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        //interaction stuff here
        if (_currentInteractable != null)
        {
            _currentInteractable.Interact();
        }
        else
        {
            Collider[] hits = Physics.OverlapSphere(_interactionPoint.position, _interactionRange, _interactionLayer);
            foreach (Collider hit in hits)
            {
                IInteractable interactable = hit.GetComponent<IInteractable>();
                if(interactable != null)
                {
                    _currentInteractable = interactable;
                    _currentInteractable.Interact();
                    break;
                }
            }
        }
    }

     public void Takedown(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        Collider[] hitEnemies = Physics.OverlapSphere(_attackPoint.position, _attackRange, _enemyLayer);
        foreach (Collider hit in hitEnemies)
        {
            GameObject hitGO = hit.transform.parent.parent.gameObject;

            if (!hitGO.GetComponent<GuardBehavior>()._seesPlayer)
            {
                Destroy(hitGO);
                break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //This line creates a mask and checks whether that layer is included in the interactionLayer Mask
        //If so, the object is on the allowed interactable layer
        if (((1 << other.gameObject.layer) & _interactionLayer) != 0)
        {
            IInteractable interactable = other.GetComponent<IInteractable>();
            if (interactable != null)
            {
                _currentInteractable = interactable;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(_currentInteractable != null && other.GetComponent<IInteractable>() == _currentInteractable)
        {
            _currentInteractable = null;
        }
    }
}