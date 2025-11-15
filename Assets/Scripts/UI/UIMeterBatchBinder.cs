using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// Batch-wires child bar Images as event-driven meters in one go.
    [DisallowMultipleComponent]
    public sealed class UIMeterBatchBinder : MonoBehaviour
    {
        [Header("Shared")]
        [SerializeField] private UIScriptableObject _uiData;

        [Header("Auto-Detect (name contains)")]
        [SerializeField] private string _poopKey = "poop";
        [SerializeField] private string _peeKey = "pee";
        [SerializeField] private string _hungryKey = "hungry";
        [SerializeField] private string _happinessKey = "happiness";
        [SerializeField] private string _staminaKey = "stamina";

        [Header("Defaults")]
        [SerializeField] private bool _invertDefault = false;
        [SerializeField] private Image.FillMethod _fillMethod = Image.FillMethod.Horizontal;
        [SerializeField] private int _fillOrigin = 0; // 0 = Left/Bottom

        [Header("Explicit Overrides (optional)")]
        [SerializeField] private List<ExplicitBind> _overrides = new();

        [Serializable]
        public struct ExplicitBind
        {
            public MeterType Type;
            public Image Image;
            public bool Invert;
            public bool UseCustomInvert;
            public Image.FillMethod FillMethod;
            public bool UseCustomFillMethod;
            public int FillOrigin;
            public bool UseCustomFillOrigin;
        }

        /// Run this after setting fields to apply to children
        [ContextMenu("Apply Now")]
        public void ApplyNow()
        {
            if (_uiData == null)
            {
                Debug.LogError($"{nameof(UIMeterBatchBinder)}: UIData missing.", this);
                return;
            }

            // Build explicit map
            var explicitImgs = new Dictionary<MeterType, ExplicitBind>();
            foreach (var e in _overrides)
                if (e.Image != null) explicitImgs[e.Type] = e;

            // Scan children
            var images = GetComponentsInChildren<Image>(includeInactive: true);

            // Pass 1: explicit binds
            foreach (var kv in explicitImgs)
                Wire(kv.Value.Image, kv.Key,
                    kv.Value.UseCustomInvert ? kv.Value.Invert : _invertDefault,
                    kv.Value.UseCustomFillMethod ? kv.Value.FillMethod : _fillMethod,
                    kv.Value.UseCustomFillOrigin ? kv.Value.FillOrigin : _fillOrigin);

            // Pass 2: auto-detect remaining by name
            foreach (var img in images)
            {
                if (img == null) continue;
                if (IsAlreadyWired(img)) continue;
                var name = img.gameObject.name.ToLowerInvariant();

                if (!TryAutoType(name, out var type)) continue;
                Wire(img, type, _invertDefault, _fillMethod, _fillOrigin);
            }

            Debug.Log($"{nameof(UIMeterBatchBinder)}: Apply complete.", this);
        }

        private bool TryAutoType(string name, out MeterType type)
        {
            if (!string.IsNullOrEmpty(_poopKey)      && name.Contains(_poopKey.ToLower()))      { type = MeterType.Poop; return true; }
            if (!string.IsNullOrEmpty(_peeKey)       && name.Contains(_peeKey.ToLower()))       { type = MeterType.Pee; return true; }
            if (!string.IsNullOrEmpty(_hungryKey)    && name.Contains(_hungryKey.ToLower()))    { type = MeterType.Hungry; return true; }
            if (!string.IsNullOrEmpty(_happinessKey) && name.Contains(_happinessKey.ToLower())) { type = MeterType.Happiness; return true; }
            if (!string.IsNullOrEmpty(_staminaKey)   && name.Contains(_staminaKey.ToLower()))   { type = MeterType.Stamina; return true; }
            type = default;
            return false;
        }

        private static bool IsAlreadyWired(Image img)
        {
            return img.GetComponent<UIMeter>() != null;
        }

        private void Wire(Image img, MeterType type, bool invert, Image.FillMethod fillMethod, int fillOrigin)
        {
            if (img == null) return;

            // Ensure proper Image setup
            img.type = Image.Type.Filled;
            img.fillMethod = fillMethod;
            img.fillOrigin = Mathf.Clamp(fillOrigin, 0, 3);
            img.fillAmount = invert ? 1f : 0f;

            // Add/update meter
            var meter = img.GetComponent<UIMeter>();
            if (meter == null) meter = img.gameObject.AddComponent<UIMeter>();

            // Set private fields via serialized to keep it simple
            SetMeterFields(meter, _uiData, type, img, invert);
        }

        private static void SetMeterFields(UIMeter meter, UIScriptableObject data, MeterType type, Image img, bool invert)
        {
            // Direct field access assumes these are [SerializeField] private in UIMeter
            var t = typeof(UIMeter);
            t.GetField("_uiData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(meter, data);
            t.GetField("_type", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(meter, type);
            t.GetField("_image", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(meter, img);
            t.GetField("_invert", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(meter, invert);
        }
    }
}
