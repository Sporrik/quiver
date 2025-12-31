using UnityEngine;
using UnityEngine.InputSystem;

public class KeyCardCollision : MonoBehaviour
{
    [SerializeField] private AnimationKeyCard _animationKeyCard;
    [SerializeField] private GoalManager _goalManager;
    [SerializeField] private int _keyCardIndex;

    private InputAction _interactAction;
    private bool _collected;

    private GameObject _player;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        PlayerInput input = _player.GetComponent<PlayerInput>();
        _interactAction = input.actions["Interact"];

    }


    private void OnTriggerEnter(Collider other)
    {
        //if (!other.CompareTag("Player")) return;

        //PlayerInput input = other.GetComponent<PlayerInput>();
        //if (input == null) return;

        //_interactAction = input.actions["Interact"];
    }

    private void OnTriggerStay(Collider other)
    {
        if (_collected) return;
        if (_interactAction == null) return;

        if (_interactAction.WasPressedThisFrame())
        {
            if (Vector3.Distance(_player.transform.position, transform.position) < 20)
            {
                CollectKeycard();
            }
        }
    }
    //private void OnTriggerExit(Collider other)
    //{
    //    if (!other.CompareTag("Player")) return;
    //    _interactAction = null;
    //}

    private void CollectKeycard()
    {
        _collected = true;

        Debug.Log("KEYCARD COLLECTED");

        if (_animationKeyCard != null)
        {
            _animationKeyCard.SetKeyCardActive(_keyCardIndex);
            _goalManager.ShowNextGoal();
        }
        gameObject.SetActive(false);
        //MeshRenderer renderer = GetComponent<MeshRenderer>();
        //renderer.enabled = false;
    }
}
