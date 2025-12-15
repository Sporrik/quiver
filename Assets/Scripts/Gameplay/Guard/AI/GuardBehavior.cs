using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Gameplay.Interaction;
using Gameplay.GuardCfg;
using Audio;


namespace Gameplay.AI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class GuardBehavior : MonoBehaviour, ITakedownTarget, IAwareness, IGuardAlertable
    {
        [Serializable]
        public class Waypoint
        {
            public Transform waypoint;
            [Min(0f)] public float waitSeconds = 0f;
        }

        #region Inspector
        [Header("Config")]
        [SerializeField] private GuardConfig _guardCfg;
        [SerializeField] private TakedownConfig _takedown;

        [Header("Stability")]
        [SerializeField] private float _nextStateChangeTime;

        [Header("Scene")]
        [SerializeField] private Transform _player;
        [SerializeField] private Transform _eyes;
        [SerializeField] private List<Waypoint> _waypoints = new();

        [Header("Runtime - Read Only")]
        [SerializeField] private bool _seesPlayer;
        [SerializeField] private float _distanceToPlayer;
        [SerializeField] private float _alertTimeRemaining;
        [SerializeField] private bool _hadVisualLastFrame;
        #endregion

        #region Getters/Events
        public bool SeesPlayer => _seesPlayer;
        public float DistanceToPlayer => _distanceToPlayer;

        public float CatchRange => _guardCfg.Combat.AttackRange;
        public bool IsAware => _state == State.Chasing || (_state == State.Searching && _alertTimeRemaining > 0f);

        public event Action<GuardBehavior> OnPlayerSpotted;
        public event Action<GuardBehavior> OnLostPlayer;
        public event Action<GuardBehavior> OnReachedLastKnown;
        public event Action<GuardBehavior> OnPlayerCaught;
        #endregion

        #region Private Vars
        private enum State { Patrolling, Chasing, Searching, Caught, Dead }
        [SerializeField] private State _state = State.Patrolling;

        private PlayerController _playerController = null;

        private NavMeshAgent _agent;
        private int _waypointIndex;
        private bool _isWaiting;
        private float _resumeAt;
        private Vector3 _lastKnownPos;
        private Vector3 _scanStartForward;
        private bool _turnLeft;
        private float _takedownCooldownUntil;
        private float _nextShoutTime;
        private static readonly Collider[] _overlapCache = new Collider[10];

        private PlayerInputRelay _inputRelay;
        private readonly object _caughtBlockToken = new object();

        // Stuff for animations
        private Animator _animator;
        private static readonly int ChaseState = Animator.StringToHash("IsChasing");
        private static readonly int SearchingState = Animator.StringToHash("IsSearching");
        private static readonly int DeathState = Animator.StringToHash("IsDead");
        #endregion

        #region Lifecycle
        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();

            // Check references
            if (_guardCfg == null) { Debug.LogError($"{name}: GuardConfig missing.", this); enabled = false; return; }
            if (_eyes == null) { Debug.LogError($"{name}: Eyes missing.", this); enabled = false; return; }

            if (_player == null)
            {
                var p = GameObject.FindGameObjectWithTag(_guardCfg.PlayerTag);

                if (p)
                {
                    _player = p.transform;
                }

                else Debug.LogWarning($"{name}: No object with tag '{_guardCfg.PlayerTag}' found. Guard will idle", this);
                _inputRelay = p.GetComponent<PlayerInputRelay>();
            }

            _agent.speed = _guardCfg.Movement.WalkSpeed;
            if (_waypoints.Count > 0)
                _agent.SetDestination(_waypoints[_waypointIndex].waypoint.position);

            if (_player)
            {
                _playerController = _player.GetComponent<PlayerController>();
            }
        }

        private void Start()
        {
            // avoids lag
            MusicController.instance.SetChase(true);
            MusicController.instance.SetChase(false);
            MusicController.instance.SetMinigame(true);
            MusicController.instance.SetMinigame(false);
        }

        private void Update()
        {
            UpdatePerception();
            TickState();
            TickAnimator();

            _hadVisualLastFrame = _seesPlayer;
        }

        private void OnDisable()
        {
            if (_inputRelay != null) _inputRelay.EndBlock(_caughtBlockToken);
        }
        #endregion

        #region Helpers
        private void SetWalkSpeed() => _agent.speed = _guardCfg.Movement.WalkSpeed;

        private void SetRunSpeed() => _agent.speed = _guardCfg.Movement.RunSpeed;

        private bool HasLineOfSight(Vector3 from, Vector3 to, LayerMask mask)
        {
            Vector3 v = to - from;
            float d = v.magnitude;
            if (d <= Mathf.Epsilon) return false;
            return !Physics.Raycast(from, v / d, out _, d, mask, QueryTriggerInteraction.Ignore);
        }

        private bool TryCatchHelper()
        {
            if (_player == null) return false;
            if (_distanceToPlayer > CatchRange) return false;

            SetState(State.Caught);
            return true;
        }

        private void AlertNearbyGuards(Vector3 playerPos)
        {
            if (Time.time < _nextShoutTime) return;
            _nextShoutTime = Time.time + _guardCfg.SocialAggro.ShoutTime;

            int hits = Physics.OverlapSphereNonAlloc(
                        transform.position,
                        _guardCfg.SocialAggro.ShoutRadius,
                        _overlapCache,
                        _guardCfg.SocialAggro.AllyLayerMask,
                        QueryTriggerInteraction.Ignore);

            for (int i = 0; i < hits; i++)
            {
                var overlap = _overlapCache[i];
                if (!overlap) continue;

                var guardScript = overlap.GetComponent<GuardBehavior>();
                if (guardScript == null || guardScript == this) continue;
                if (!guardScript.enabled || !guardScript.isActiveAndEnabled) continue;

                var myEyes = _eyes ? _eyes.position : transform.position + Vector3.up * 1.6f;
                var theirEyes = guardScript._eyes ? guardScript._eyes.position : guardScript.transform.position + Vector3.up * 1.6f;
                if (!HasLineOfSight(myEyes, theirEyes, _guardCfg.LoSMask)) continue;

                if (!guardScript.IsAware)
                {
                    guardScript.OnCryAlert(playerPos);
                    guardScript._nextShoutTime = Time.time + _guardCfg.SocialAggro.ShoutTime;
                }
            }
        }

        private void OnEnter(State state)
        {
            switch (state)
            {
                case State.Patrolling:
                    _agent.updateRotation = true;
                    SetWalkSpeed();
                    if (_waypoints.Count > 0)
                        _agent.SetDestination(_waypoints[_waypointIndex].waypoint.position);
                    break;

                case State.Chasing:
                    MusicController.instance.SetChase(true);
                    AlertNearbyGuards(_player.position);
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
                case State.Caught:
                    MusicController.instance.SetDeath();
                    _agent.updateRotation = false;
                    _agent.ResetPath();
                    _agent.isStopped = true;
                    OnPlayerCaught?.Invoke(this);

                    if (_player != null)
                    {
                        Vector3 to = (_player.position - transform.position);
                        to.y = 0f;

                        if (to.sqrMagnitude > 0.0001f)
                        {
                            transform.rotation = Quaternion.LookRotation(to.normalized, Vector3.up);
                        }
                    }

                    if (_inputRelay != null) _inputRelay.EndBlock(_caughtBlockToken);
                    break;
                case State.Dead:
                    _agent.isStopped = true;
                    _agent.enabled = false;
                    break;
            }
        }

        private void OnExit(State state)
        {
            if (state == State.Searching)
            {
                MusicController.instance.SetGameplay();
            }

            if (state == State.Caught)
            {
                if (_inputRelay != null) _inputRelay.EndBlock(_caughtBlockToken);
                _agent.isStopped = false;
                _agent.updateRotation = true;
            }
        }
        #endregion

        #region Perception
        private void UpdatePerception()
        {
            bool hadPrev = _hadVisualLastFrame;
            _seesPlayer = false;
            if (_player == null) return;

            _distanceToPlayer = Vector3.Distance(_player.position, transform.position);

            float sight = _guardCfg.Perception.SightRange;
            float radius = _guardCfg.Perception.CloseSightRadius;

            // Apply multipliers based on states
            if (_state == State.Chasing || _state == State.Searching)
            {
                sight *= _guardCfg.Perception.SightAlertMulti;
                radius *= _guardCfg.Perception.SightAlertMulti;

            }

            if (_player.GetComponent<PlayerController>().IsSprinting)
            {
                sight *= _guardCfg.Perception.SightSprintMulti;
                radius *= _guardCfg.Perception.SightSprintMulti;
            }

            // Early-out by distance
            if (_distanceToPlayer > sight) return;

            // Close-range vision check
            if (_playerController.IsSprinting && _distanceToPlayer <= radius)
            {
                bool okLoS = _guardCfg.Perception.CloseSightIgnoreLoS
                    || !Physics.Raycast(_eyes.position,
                    (_player.position - _eyes.position).normalized,
                    _distanceToPlayer,
                    _guardCfg.LoSMask,
                    QueryTriggerInteraction.Ignore);

                if (okLoS)
                {
                    if (_guardCfg.Perception.CloseSightIgnoreFoV)
                    {
                        _seesPlayer = true;
                        return;
                    }
                }
            }

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
        #endregion

        #region States
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
                    if (TryCatchHelper()) return;
                    if (_seesPlayer) { _lastKnownPos = _player.position; Chase(); }
                    else if (CanChangeState()) { SetState(State.Searching); }
                    break;

                case State.Searching:
                    if (TryCatchHelper()) return;
                    if (_seesPlayer && CanChangeState()) { SetState(State.Chasing); return; }
                    Search();
                    break;
                case State.Caught:
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

            Waypoint wp = _waypoints[_waypointIndex];
            if (wp == null || !wp.waypoint) return;

            SetWalkSpeed();

            if (_isWaiting)
            {
                if (Time.time < _resumeAt) { _agent.isStopped = true; return; }
                _isWaiting = false;
                _agent.isStopped = false;
                AdvanceToNextWaypoint();
                return;
            }

            if ( !_agent.pathPending && _agent.remainingDistance <= _guardCfg.Movement.WaypointArriveDistance )
            {
                if (wp.waitSeconds > 0f)
                {
                    _isWaiting = true;
                    _resumeAt = Time.time + wp.waitSeconds;
                    _agent.isStopped = true;
                    return;
                }

                AdvanceToNextWaypoint();
                return;
            }

            if (!_agent.hasPath || _agent.isPathStale)
            {
                _agent.isStopped = false;
                _agent.SetDestination(wp.waypoint.position);
            }
        }

        private void AdvanceToNextWaypoint()
        {
            _waypointIndex = (_waypointIndex + 1) % _waypoints.Count;
            var nextWP = _waypoints[_waypointIndex];
            if (nextWP != null && nextWP.waypoint)
            {
                _agent.isStopped = false;
                _agent.SetDestination(nextWP.waypoint.position);
            }
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
        #endregion

        #region External Alerts
        public void OnCryAlert(Vector3 sourcePosition)
        {
            _lastKnownPos = sourcePosition;
            _nextShoutTime = Time.time + _guardCfg.SocialAggro.ShoutTime;
            SetState(State.Chasing);
        }
        #endregion

        #region ITakedownTarget
        public bool CanTakedown(Interactor interactor)
        {
            if (_takedown == null) return false;
            if (Time.time < _takedownCooldownUntil) return false;
            if (!enabled || !_agent.enabled) return false;
            if (_state != State.Patrolling) return false;

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
            SetState(State.Dead);
        }

        private void TickAnimator()
        {
            if (_animator == null) return;

            _animator.SetBool(ChaseState, _state == State.Chasing);
            _animator.SetBool(SearchingState, _state == State.Searching);
            _animator.SetBool(DeathState, _state == State.Dead);
        }

        public bool CaughtPlayer()
        {
            return _state == State.Caught;
        }

        #endregion

        // ---------- Gizmos ----------
#if UNITY_EDITOR
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

            // close sight radius
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.25f);
            Gizmos.DrawWireSphere(transform.position, _guardCfg.Perception.CloseSightRadius);

            // social aggro
            Gizmos.color = new Color(1f, 0f, 0f, 0.15f);
            Gizmos.DrawWireSphere(transform.position, _guardCfg.SocialAggro.ShoutRadius);

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