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
        float angle = _sightAngle / 2;
        Vector3 forward = transform.forward;

        // Calculate left and right ray directions
        Vector3 leftRayDirection = (Quaternion.Euler(0, -angle, 0) * forward).normalized;
        Vector3 rightRayDirection = (Quaternion.Euler(0, angle, 0) * forward).normalized;

        // Set gizmo colors and draw rays

        if( _isPlayerInSightRange )
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        for(int degree = (int)angle; degree > -angle; degree--)
        {
            Vector3 p0 = transform.position + (Quaternion.Euler(0, degree - 1, 0) * forward).normalized * _sightRange;
            Vector3 p1 = transform.position + (Quaternion.Euler(0, degree, 0) * forward).normalized * _sightRange;

            Gizmos.DrawLine(p0, p1);

        }

        Gizmos.DrawRay(transform.position, leftRayDirection * _sightRange);
        Gizmos.DrawRay(transform.position, rightRayDirection * _sightRange);
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
