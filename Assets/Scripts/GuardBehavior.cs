using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class GuardBehavior : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject _eyes;
    [SerializeField] private bool _seesPlayer;

    [SerializeField] private List<GameObject> _path;
    private int _currentPathPoint;

    private Vector3 _lastPlayerPosition;

    [SerializeField] private float _memorizationTime = 10.0f;
    [SerializeField] private float _timeAlert;

    private NavMeshAgent _agent;
    [SerializeField] private float _sightRange, _sightAngle, _attackRange;
    private bool _isPlayerInSightRange, _isPlayerInAttackRange;

    private void Start()
    {
        _player = GameObject.FindWithTag("Player").transform;
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        PlayerDetection();
        FollowPath();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 t0 = (transform.position + new Vector3(_sightRange * MathF.Cos(_sightAngle / 2), 0.0f, _sightRange * MathF.Sin(_sightAngle / 2)));
        Vector3 t1 = (transform.position + new Vector3(_sightRange * MathF.Cos(-_sightAngle / 2), 0.0f, _sightRange * MathF.Sin(-_sightAngle / 2)));

        //Gizmos.DrawLine(transform.position, t0);
        //Gizmos.DrawLine(transform.position, t1);

        //Gizmos.DrawSphere(transform.position, _sightRange);
    }

    private void ChasePlayer() => _agent.SetDestination(_lastPlayerPosition);

    static readonly string[] SIGHT_BLOCK_MASK = { "Ground", "StaticLevel" };
    private void PlayerDetection()
    {
        _isPlayerInSightRange = Physics.CheckSphere(transform.position, _sightRange, playerLayer);
        _isPlayerInAttackRange = Physics.CheckSphere(transform.position, _attackRange, playerLayer);

        // check if player is in range
        if (_isPlayerInSightRange && !_isPlayerInAttackRange)
        {
            if (_eyes == null) return;

            Vector3 directionToPlayer = (_player.transform.position - _eyes.transform.position).normalized;
            float angleToPlayer = Vector3.Angle(_eyes.transform.forward, directionToPlayer);

            if (angleToPlayer <= _sightAngle / 2f)
            {
                Ray ray = new Ray(_eyes.transform.position, directionToPlayer);

                if (!Physics.Raycast(ray, _sightRange, LayerMask.GetMask(SIGHT_BLOCK_MASK)))
                {
                    _timeAlert = _memorizationTime;

                    _lastPlayerPosition = _player.position;
                }
            }

        }

        _seesPlayer = _timeAlert > 0.0f;

        if (_seesPlayer )
        {
            if((_lastPlayerPosition - transform.position).magnitude <= _attackRange * 1.5f)
            {
                _timeAlert -= Time.deltaTime;

                LookAround();         
            }

            ChasePlayer();
        }
    }

    private void FollowPath()
    {
        if (_seesPlayer) return;

        if (_path[0] == null) return;

        if((_path[_currentPathPoint].transform.position - transform.position).magnitude <= _attackRange)
        {
            ++_currentPathPoint;

            if (_currentPathPoint > _path.Count - 1) _currentPathPoint = 0;
        }

        _agent.SetDestination(_path[_currentPathPoint].transform.position);

    }

    private void LookAround()
    {

    }
}
