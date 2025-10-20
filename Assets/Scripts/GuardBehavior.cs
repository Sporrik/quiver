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

    [SerializeField] public bool _seesPlayer;

    [SerializeField] private List<GameObject> _path;
    [SerializeField] private string[] _sightBlockLayers = { "Ground", "StaticLevel" };

    [SerializeField] private float _memorizationTime = 10.0f;
    [SerializeField] private float _timeAlert;

    [SerializeField] private float _sightRange, _sightAngle, _attackRange;
    [SerializeField] private float _noticeRangeWhenAlert = 10;

    [SerializeField] private float _baseSpeed = 3.5f;
    [SerializeField] private float _runSpeed = 5f;

    private int _currentPathPoint;

    private Vector3 _lastPlayerPosition;

    private Vector3 _LastForward;
    private bool _turnLeft;

    private NavMeshAgent _agent;
    private LevelManager _levelManager;
    private bool _isPlayerInSightRange, _isPlayerInAttackRange;

    private float _distanceToPlayer;

    private void Start()
    {
        _player = GameObject.FindWithTag("Player").transform;
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _baseSpeed;

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

        float distanceToPlayer = (_player.transform.position - transform.position).magnitude;

        // Calculate left and right ray directions
        Vector3 leftRayDirection = (Quaternion.Euler(0, -angle, 0) * _eyes.transform.forward).normalized;
        Vector3 rightRayDirection = (Quaternion.Euler(0, angle, 0) * _eyes.transform.forward).normalized;

        Vector3 drawPosition = _eyes.transform.position;
        drawPosition.y = transform.position.y;

        // Set gizmo colors and draw rays

        if( _seesPlayer )
        {
            Gizmos.color = Color.green;
        }
        else if (!_seesPlayer && _timeAlert > 0.0f)
        {
            Gizmos.color = Color.blue;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        for(int degree = (int)angle; degree > -angle; --degree)
        {
            Vector3 p0 = drawPosition + (Quaternion.Euler(0, degree - 1, 0) * forward).normalized * _sightRange;
            Vector3 p1 = drawPosition + (Quaternion.Euler(0, degree, 0) * forward).normalized * _sightRange;

            Gizmos.DrawLine(p0, p1);
        }

        Gizmos.DrawRay(drawPosition, leftRayDirection * _sightRange);
        Gizmos.DrawRay(drawPosition, rightRayDirection * _sightRange);

        Gizmos.color = Color.blue;

        for (int degree = 360; degree > 0; --degree)
        {
            Vector3 p0 = drawPosition + (Quaternion.Euler(0, degree - 1, 0) * forward).normalized * _noticeRangeWhenAlert;
            Vector3 p1 = drawPosition + (Quaternion.Euler(0, degree, 0) * forward).normalized * _noticeRangeWhenAlert;

            Gizmos.DrawLine(p0, p1);
        }
    }

    private void ChasePlayer() => _agent.SetDestination(_player.transform.position);
        
    private void PlayerDetection()
    {
        _seesPlayer = false;

        if (_player == null) return;
        if (_eyes == null) return;

        Vector3 directionToPlayer = (_player.transform.position - _eyes.transform.position).normalized;
        float angleToPlayer = Vector3.Angle(_eyes.transform.forward, directionToPlayer);

        _distanceToPlayer = (_player.transform.position - transform.position).magnitude;

        _isPlayerInSightRange = _distanceToPlayer <= _sightRange;
        _isPlayerInAttackRange = _distanceToPlayer <= _attackRange;

        Ray ray = new Ray(_eyes.transform.position, directionToPlayer);

        //Debug.Log($"in FOV: {angleToPlayer <= _sightAngle / 2}, distance to player: {distanceToPlayer}, " +
        //          $"is behind wall: {Physics.Raycast(ray, _sightRange, LayerMask.GetMask(_sightBlockLayers))}");

        // check if player is in range
        if (_isPlayerInSightRange)
        {
            if (angleToPlayer <= _sightAngle / 2)
            {
                if (!Physics.Raycast(ray, _distanceToPlayer, LayerMask.GetMask(_sightBlockLayers)))
                {
                    _seesPlayer = true;
                    _agent.speed = _runSpeed;
                }
            }
        }

        if (!_seesPlayer && _timeAlert > 0.0f)
        {
            const float distanceMargin = 1.0f;

            float distanceToLastPosition = (_lastPlayerPosition - transform.position).magnitude;            

            if (distanceToLastPosition <= _attackRange + distanceMargin)
            {
                Debug.Log($"Guard has reached the last seen player position!");

                _timeAlert -= Time.deltaTime;

                LookAround();
                _agent.speed = _baseSpeed;
            }
            else
            {
                _LastForward = transform.forward;

                _seesPlayer = false;
            }

            if (_distanceToPlayer < _noticeRangeWhenAlert)
            {
                //_lastPlayerPosition = _player.transform.position;
                if (!Physics.Raycast(ray, _distanceToPlayer, LayerMask.GetMask(_sightBlockLayers)))
                {
                    _seesPlayer = true;
                }
            }
            else
            {
                _agent.SetDestination(_lastPlayerPosition);
            }
        }
        else if (_seesPlayer)
        {
            _lastPlayerPosition = _player.transform.position;

            ChasePlayer();

            _timeAlert = _memorizationTime;
        }
    }

    private void FollowPath()
    {
        if (_timeAlert > 0.0f) return;

        if (_path.Count == 0) return;

        if((_path[_currentPathPoint].transform.position - transform.position).magnitude <= _attackRange)
        {
            ++_currentPathPoint;

            if (_currentPathPoint > _path.Count - 1) _currentPathPoint = 0;
        }

        _agent.SetDestination(_path[_currentPathPoint].transform.position);

    }

    private void LookAround()
    {
        const float maxTurnAngle = 90f;
        float rotationSpeed = 75f;

        if(_turnLeft)
        {
            transform.Rotate(0, Time.deltaTime * rotationSpeed, 0);
        }
        else
        {
            transform.Rotate(0, -Time.deltaTime * rotationSpeed, 0);
        }

        if (Vector3.Angle(_LastForward, transform.forward) > maxTurnAngle)
        {
            _turnLeft = !_turnLeft;
        }
    }
    public bool CanGetCaught(Vector3 position)
    {
        return _isPlayerInAttackRange && _seesPlayer;
    }
    public void AlertGuardsToPosition(float distanceToAlert)
    {
        if(_distanceToPlayer < distanceToAlert)
        {
            Debug.Log($" {gameObject.name} has seen player with {_distanceToPlayer} distance ");
            _lastPlayerPosition = _player.transform.position;
            _timeAlert = _memorizationTime;
        }
    }
}
