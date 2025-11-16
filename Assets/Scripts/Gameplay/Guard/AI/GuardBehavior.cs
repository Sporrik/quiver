using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Gameplay.Interaction;
using Gameplay.GuardCfg;


namespace Gameplay.AI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class GuardBehavior : MonoBehaviour, ITakedownTarget, IAwareness, IGuardAlertable
    {
        [Header("Config")]
        [SerializeField] private GuardConfig _guardCfg;
        [SerializeField] private TakedownConfig _takedown;

        [Header("Stability")]
        [SerializeField] private float _nextStateChangeTime;

        [Header("Scene")]
        [SerializeField] private Transform _player;
        [SerializeField] private Transform _eyes;
        [SerializeField] private List<Transform> _waypoints = new();

        [Header("Runtime - Read Only")]
        [SerializeField] private bool _seesPlayer;
        [SerializeField] private float _distanceToPlayer;
        [SerializeField] private float _alertTimeRemaining;
        [SerializeField] private bool _hadVisualLastFrame;

        public bool SeesPlayer => _seesPlayer;
        public float DistanceToPlayer => _distanceToPlayer;

        public float CatchRange => _guardCfg.Combat.AttackRange;
        public bool IsAware => _state == State.Chasing || (_state == State.Searching && _alertTimeRemaining > 0f);

        public event Action<GuardBehavior> OnPlayerSpotted;
        public event Action<GuardBehavior> OnLostPlayer;
        public event Action<GuardBehavior> OnReachedLastKnown;

        private enum State { Patrolling, Chasing, Searching }
        [SerializeField] private State _state = State.Patrolling;

        private NavMeshAgent _agent;
        private int _waypointIndex;
        private Vector3 _lastKnownPos;
        private Vector3 _scanStartForward;
        private bool _turnLeft;
        private float _takedownCooldownUntil;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();

            // Check references
            if (_guardCfg == null) { Debug.LogError($"{name}: GuardConfig missing.", this); enabled = false; return; }
            if (_eyes == null) { Debug.LogError($"{name}: Eyes missing.", this); enabled = false; return; }

            if (_player == null)
            {
                var p = GameObject.FindGameObjectWithTag(_guardCfg.PlayerTag);
                if (p) _player = p.transform;
                else Debug.LogWarning($"{name}: No object with tag '{_guardCfg.PlayerTag}' found. Guard will idle", this);
            }

            _agent.speed = _guardCfg.Movement.WalkSpeed;
            if (_waypoints.Count > 0)
                _agent.SetDestination(_waypoints[_waypointIndex].position);
        }

        private void Update()
        {
            UpdatePerception();
            TickState();
            _hadVisualLastFrame = _seesPlayer;
        }

        // ---------- Helpers ----------
        private void SetWalkSpeed() => _agent.speed = _guardCfg.Movement.WalkSpeed;

        private void SetRunSpeed() => _agent.speed = _guardCfg.Movement.RunSpeed;

        private bool HasLineOfSight(Vector3 from, Vector3 to, LayerMask mask)
        {
            Vector3 v = to - from;
            float d = v.magnitude;
            if (d <= Mathf.Epsilon) return false;
            return !Physics.Raycast(from, v / d, out _, d, mask, QueryTriggerInteraction.Ignore);
        }

        private void OnEnter(State state)
        {
            switch (state)
            {
                case State.Patrolling:
                    _agent.updateRotation = true;
                    SetWalkSpeed();
                    if (_waypoints.Count > 0)
                        _agent.SetDestination(_waypoints[_waypointIndex].position);
                    break;

                case State.Chasing:
                    _agent.updateRotation = true;
                    SetRunSpeed();
                    _alertTimeRemaining = _guardCfg.Search.AlertTime;
                    OnPlayerSpotted?.Invoke(this);
                    Chase();
                    break;

                case State.Searching:
                    _agent.updateRotation = false;
                    SetWalkSpeed();
                    _alertTimeRemaining = Mathf.Max(_alertTimeRemaining, _guardCfg.Search.AlertTime);
                    _scanStartForward = transform.forward;
                    _turnLeft = false;
                    OnLostPlayer?.Invoke(this);
                    _agent.SetDestination(_lastKnownPos);
                    break;
            }
        }

        private void OnExit(State state)
        {
            // For future: stop coroutines, clear temp flags, etc.
        }

        // ---------- Perception ---------
        private void UpdatePerception()
        {
            bool hadPrev = _hadVisualLastFrame;
            _seesPlayer = false;
            if (_player == null) return;

            _distanceToPlayer = Vector3.Distance(_player.position, transform.position);

            float sight = _guardCfg.Perception.SightRange;

            if (_state == State.Searching)
            {
                sight *= _guardCfg.Perception.SightAlertMulti;
            }

            // Early-out by distance
            if (_distanceToPlayer > sight) return;

            // Angle check from eyes
            Vector3 fromEyes = _player.position - _eyes.position;
            Vector3 dirFromEyes = fromEyes.normalized;

            float halfFov = _guardCfg.Perception.SightAngle * 0.5f;
            float extra = hadPrev ? (_guardCfg.Stability.FovExitLag * 0.5f) : 0f; // widen only when exiting

            if (Vector3.Angle(_eyes.forward, dirFromEyes) > (halfFov + extra)) return;

            // LoS
            if (HasLineOfSight(_eyes.position, _player.position, _guardCfg.LoSMask))
                _seesPlayer = true;
        }

        // ---------- FSM ----------
        private bool CanChangeState() => Time.time >= _nextStateChangeTime;     // potential to quick switching state (no reset?)

        private void TickState()
        {
            switch (_state)
            {
                case State.Patrolling:
                    if (_seesPlayer && CanChangeState()) { SetState(State.Chasing); return; }
                    Patrol();
                    break;

                case State.Chasing:
                    if (_seesPlayer) { _lastKnownPos = _player.position; Chase(); }
                    else if (CanChangeState()) { SetState(State.Searching); }
                    break;

                case State.Searching:
                    if (_seesPlayer && CanChangeState()) { SetState(State.Chasing); return; }
                    Search();
                    break;
            }
        }

        private void SetState(State next)
        {
            if (_state == next) return;
            OnExit(_state);
            _state = next;
            _nextStateChangeTime = Time.time + (_guardCfg != null ? _guardCfg.Stability.StateCooldownSeconds : 0f);
            OnEnter(_state);
        }

        private void Patrol()
        {
            if (_waypoints.Count == 0) return;

            Transform target = _waypoints[_waypointIndex];
            if (!target) return;

            SetWalkSpeed();

            if (!_agent.pathPending && _agent.remainingDistance <= _guardCfg.Movement.WaypointArriveDistance)
            {
                _waypointIndex = (_waypointIndex + 1) % _waypoints.Count;
                _agent.SetDestination(_waypoints[_waypointIndex].position);
                return;
            }

            if (!_agent.hasPath) _agent.SetDestination(target.position);
        }

        private void Chase()
        {
            if (_player == null) return;
            if (!_agent.hasPath || (_agent.destination - _player.position).sqrMagnitude > 0.25f)
                _agent.SetDestination(_player.position);
        }

        private void Search()
        {
            if (_agent.pathPending) return;

            float arrive = Mathf.Max(_guardCfg.Combat.AttackRange, _guardCfg.Movement.WaypointArriveDistance);
            if (_agent.remainingDistance > arrive) return;

            OnReachedLastKnown?.Invoke(this);

            // manual yaw oscillation
            float delta = _guardCfg.Movement.RotationSpeed * Time.deltaTime * (_turnLeft ? 1f : -1f);
            transform.Rotate(0f, delta, 0f);
            if (Vector3.Angle(_scanStartForward, transform.forward) > _guardCfg.Search.ScanMaxTurnAngle)
                _turnLeft = !_turnLeft;

            _alertTimeRemaining -= Time.deltaTime;
            if (_alertTimeRemaining <= 0f)
                SetState(State.Patrolling);
        }

        // ---------- External Alerts ----------
        public void AlertToPosition(Vector3 worldPos)
        {
            _lastKnownPos = worldPos;
            SetState(State.Searching);
        }

        public void OnCryAlert(Vector3 sourcePosition, float radius)
        {
            _lastKnownPos = sourcePosition;
            SetState(State.Searching);
            _alertTimeRemaining = Mathf.Max(_alertTimeRemaining, _guardCfg.Search.AlertTime);
        }

        // ---------- ITakedownTarget ----------
        public bool CanTakedown(Interactor interactor)
        {
            if (_takedown == null) return false;
            if (Time.time < _takedownCooldownUntil) return false;
            if (!enabled || !_agent.enabled) return false;

            if (_seesPlayer && _state == State.Chasing) return false;

            Vector3 toAttacker = interactor.Transform.position - transform.position;
            float dist = toAttacker.magnitude;
            if (dist > _takedown.Range) return false;

            float angleFromBack = Vector3.Angle(-transform.forward, toAttacker.normalized);
            if (angleFromBack > _takedown.BackAngle * 0.5f) return false;

            bool blocked = Physics.Raycast(interactor.Transform.position,
                                            (transform.position - interactor.Transform.position).normalized,
                                            dist,
                                            _takedown.LoSMask,
                                            QueryTriggerInteraction.Ignore);

            return !blocked;
        }

        public void Takedown(Interactor interactor)
        {
            _takedownCooldownUntil = Time.time + (_takedown?.CooldownSeconds ?? 0f);
            _agent.isStopped = true;
            _agent.enabled = false;
            enabled = false;
        }

        // ---------- Gizmos ----------
#if     UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_guardCfg == null || !_guardCfg.Debug.DrawGizmos) return;
            if (_eyes == null) return;

            // field of view
            Gizmos.color = Color.yellow;
            DrawCone(_eyes.position, _eyes.forward, _guardCfg.Perception.SightRange, _guardCfg.Perception.SightAngle);

            // attack range
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _guardCfg.Combat.AttackRange);

            if (_state == State.Searching)
            {
                Gizmos.color = new Color(0f, 1f, 1f, 0.25f);
                Gizmos.DrawWireSphere(transform.position, _guardCfg.Perception.SearchingNoticeRange);
            }
        }

        private static void DrawCone(Vector3 origin, Vector3 forward, float radius, float angle)
        {
            const int steps = 36;
            float half = angle * 0.5f;

            forward = forward.normalized; // in case this hasn't happened yet

            Vector3 prev = origin + Quaternion.AngleAxis(-half, Vector3.up) * forward * radius;
            for (int i = 1; i <= steps; i++)
            {
                float t = Mathf.Lerp(-half, half, i / (float)steps);
                Vector3 next = origin + Quaternion.AngleAxis(t, Vector3.up) * forward * radius;
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
            Gizmos.DrawLine(origin, origin + Quaternion.AngleAxis(-half, Vector3.up) * forward * radius);
            Gizmos.DrawLine(origin, origin + Quaternion.AngleAxis(half, Vector3.up) * forward * radius);
        }
    #endif
    }
}