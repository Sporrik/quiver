using Autodesk.Fbx;
using System;
using UnityEngine;

namespace Gameplay.GuardCfg
{
    [CreateAssetMenu(menuName = "Configs/Guard", fileName = "Guard_Default")]
    public sealed class GuardConfig : ScriptableObject
    {
        [Header("Targets & Layers")]
        [SerializeField, Tooltip("Optional fallback if GuardBehavior._player is not wired")]
        private string _playerTag = "Player";
        [SerializeField, Tooltip("Geometry that blocks guard vision")]
        private LayerMask _losMask;

        [Serializable]
        public sealed class GuardPerception
        {
            [Header("Vision")]
            [SerializeField, Min(0f)] private float _sightRange = 10f;
            [SerializeField, Range(0f, 180f)] private float _sightAngle = 120f;
            [SerializeField, Min(1f), Tooltip("Range multiplier when target is sprinting")]
            private float _sightSprintMulti = 1.2f;
            [SerializeField, Min(1f), Tooltip("Range multiplior when guard is alert")]
            private float _sightAlertMulti = 1.2f;
            [SerializeField, Min(0f), Tooltip("Forgiving notice radius outside of FOV, only while Searching")]
            private float _searchingNoticeRange;
            [SerializeField, Min(0f)] private float _closeSightRadius = 1.2f;
            [SerializeField] private bool _closeSightIgnoreFoV = true;         //see outside cone
            [SerializeField] private bool _closeSightIgnoreLoS = false;

            public float SightRange => _sightRange;
            public float SightAngle => _sightAngle;
            public float SightSprintMulti => _sightSprintMulti;
            public float SightAlertMulti => _sightAlertMulti;
            public float SearchingNoticeRange => _searchingNoticeRange;
            public float CloseSightRadius => _closeSightRadius;
            public bool CloseSightIgnoreFoV => _closeSightIgnoreFoV;
            public bool CloseSightIgnoreLoS => _closeSightIgnoreLoS;
        }

        [Serializable]
        public sealed class GuardMovement
        {
            [Header("Speed")]
            [SerializeField, Min(0f)] private float _walkSpeed = 3.5f;
            [SerializeField, Min(0f)] private float _runSpeed = 5f;

            [Header("Navigation")]
            [SerializeField, Min(0f)] private float _waypointArriveDistance = 1f;
            [SerializeField, Min(0f)] private float _rotationSpeed = 360f;

            public float WalkSpeed => _walkSpeed;
            public float RunSpeed => _runSpeed;
            public float WaypointArriveDistance => _waypointArriveDistance;
            public float RotationSpeed => _rotationSpeed;
        }

        [Serializable]
        public sealed class GuardSearch
        {
            [SerializeField, Min(0f), Tooltip("How long the guard searches after losing sight of target")]
            private float _alertTime = 8f;
            [SerializeField, Min(0f), Tooltip("Yaw degrees/second while scanning")]
            private float _scanYawSpeed = 75f;
            [SerializeField, Range(0f, 180f), Tooltip("Max deviation from forward while scanning")]
            private float _scanMaxTurnAngle = 90f;

            public float AlertTime => _alertTime;
            public float ScanYawSpeed => _scanYawSpeed;
            public float ScanMaxTurnAngle => _scanMaxTurnAngle;
        }

        [Serializable]
        public sealed class GuardCombat
        {
            [SerializeField, Min(0f)] private float _attackRange = 1.25f;

            public float AttackRange => _attackRange;
        }

        [Serializable]
        public sealed class GuardDebug
        {
            [SerializeField] private bool _drawGizmos = true;

            public bool DrawGizmos => _drawGizmos;
        }

        [Serializable]
        public sealed class GuardStability
        {
            [SerializeField, Min(0f), Tooltip("Minimum time between state switches (Patrol/Chase/Search).")]
            private float _stateCooldownSeconds = 0.35f;
            [SerializeField, Range(0f, 30f), Tooltip("Extra degrees added when LoS was true last frame")]
            private float _fovExitLag = 5f;

            public float StateCooldownSeconds => _stateCooldownSeconds;
            public float FovExitLag => _fovExitLag;
        }

        [Header("Sections")]
        [SerializeField] private GuardPerception _perception = new();
        [SerializeField] private GuardMovement _movement = new();
        [SerializeField] private GuardSearch _search = new();
        [SerializeField] private GuardCombat _combat = new();
        [SerializeField] private GuardDebug _debug = new();
        [SerializeField] private GuardStability _stability = new();

        public string PlayerTag => _playerTag;
        public LayerMask LoSMask => _losMask;

        public GuardPerception Perception => _perception;
        public GuardMovement Movement => _movement;
        public GuardSearch Search => _search;
        public GuardCombat Combat => _combat;
        public GuardDebug Debug => _debug;
        public GuardStability Stability => _stability;

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Avoids designer typos
            _perception ??= new GuardPerception();
            _movement ??= new GuardMovement();
            _search ??= new GuardSearch();
            _combat ??= new GuardCombat();
            _debug ??= new GuardDebug();
            _stability ??= new GuardStability();
        }
#endif
    }
}