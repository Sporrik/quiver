using Unity.Collections;
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
        if (hitEnemies != null) hitEnemies[0].gameObject.SetActive(false);
    }
}