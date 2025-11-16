using UnityEngine;

namespace Infrastructure
{
    /// Keep the GameSystems object alive across scene loads.
    [DisallowMultipleComponent]
    public sealed class GameSystemsAnchor : MonoBehaviour
    {
        [Tooltip("If true, ensure only one GameSystemsAnchor survives across the app.")]
        [SerializeField] private bool _enforceSingleton = true;

        private static GameSystemsAnchor _instance;

        private void Awake()
        {
            if (_enforceSingleton)
            {
                if (_instance != null && _instance != this)
                {
                    Destroy(gameObject);
                    return;
                }
                _instance = this;
            }

            if (transform.parent != null) transform.SetParent(null, worldPositionStays: true);

            DontDestroyOnLoad(gameObject);
        }
    }
}