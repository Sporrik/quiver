using UnityEngine;
using UnityEngine.AI;

public class GuardBehavior : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject _eyes;

    private NavMeshAgent _agent;
    [SerializeField] private float _sightRange, _attackRange;
    private bool _isPlayerInSightRange, _isPlayerInAttackRange;

    private void Start()
    {
        _player = GameObject.FindWithTag("Player").transform;
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        PlayerDetection();
    }

    private void ChasePlayer() => _agent.SetDestination(_player.position);

    static readonly string[] SIGHT_BLOCK_MASK = { "Ground", "StaticLevel" };
    void PlayerDetection()
    {
        _isPlayerInSightRange = Physics.CheckSphere(transform.position, _sightRange, playerLayer);
        _isPlayerInAttackRange = Physics.CheckSphere(transform.position, _attackRange, playerLayer);

        // check if player is in range
        if (_isPlayerInSightRange && !_isPlayerInAttackRange)
        {
            if (_eyes == null) return;

            // check if something is obstruction the guards line of sight
            Ray ray = new Ray(_eyes.transform.position, (_player.transform.position - _eyes.transform.position).normalized);

            if (!Physics.Raycast(ray, _sightRange, LayerMask.GetMask(SIGHT_BLOCK_MASK)))
            {
                ChasePlayer();
            }
        }
        else
        {
            // there should be something extra here later on, so it can feel more natural (maybe a delay)
            _agent.SetDestination(transform.position);
        }
    }
}
