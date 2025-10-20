using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private float _attackRange;
    [SerializeField] private LayerMask _enemyLayer;

    public void Interact(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        //interaction suff here
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
}