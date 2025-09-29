using UnityEngine;
using UnityEngine.AI;

public class GuardBehavior : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private LayerMask playerLayer;
    private NavMeshAgent _agent;
    [SerializeField]  private float _sightRange, _attackRange;
    private bool _isPlayerInSightRange, _isPlayerInAttackRange;

    private void Start()
    {
        _player = GameObject.FindWithTag("Player").transform;
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        _isPlayerInSightRange = Physics.CheckSphere(transform.position, _sightRange, playerLayer);
        _isPlayerInAttackRange = Physics.CheckSphere(transform.position, _attackRange, playerLayer);

        if (_isPlayerInSightRange && !_isPlayerInAttackRange)
        {
            ChasePlayer();
        }
        else
        {
            _agent.SetDestination(transform.position);
        } 
    }

    private void ChasePlayer() => _agent.SetDestination(_player.position);
}
