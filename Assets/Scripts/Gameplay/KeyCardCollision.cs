using UnityEngine;

public class KeyCardCollision : MonoBehaviour
{
    [SerializeField] private AnimationKeyCard _animationKeyCard;
    [SerializeField] private GoalManager _goalManager;
    [SerializeField] private int _keyCardIndex;

    private bool _collected;

    private void OnTriggerEnter(Collider other)
    {
        if (_collected) return;
        if (!other.CompareTag("Player")) return;

        CollectKeycard();
    }

    private void CollectKeycard()
    {
        _collected = true;

        Debug.Log("KEYCARD COLLECTED");

        if (_animationKeyCard != null)
        {
            _animationKeyCard.SetKeyCardActive(_keyCardIndex);
        }

        _goalManager.ShowNextGoal();
        gameObject.SetActive(false);
    }
}

//using System.Data.Common;
//using UnityEngine;
//using UnityEngine.InputSystem;

//public class KeyCardCollision : MonoBehaviour
//{
//    [SerializeField] private AnimationKeyCard _animationKeyCard;
//    [SerializeField] private GoalManager _goalManager;
//    [SerializeField] private int _keyCardIndex;

//    private InputAction _interactAction;
//    private bool _collected;

//    private GameObject _player;

//    private void Awake()
//    {
//        _player = GameObject.FindGameObjectWithTag("Player");
//        PlayerInput input = _player.GetComponent<PlayerInput>();
//        _interactAction = input.actions["Interact"];

//    }


//    //private void OnTriggerEnter(Collider other)
//    //{
//    //    Debug.Log("TRIGGER ENTERED");
//    //    if (other.gameObject.CompareTag("KeyCard"))
//    //    {
//    //        Debug.Log("KEYCARD DETECTED IN TRIGGER");
//    //    }

//    //        //if (!other.CompareTag("Player")) return;

//    //        //PlayerInput input = other.GetComponent<PlayerInput>();
//    //        //if (input == null) return;

//    //        //_interactAction = input.actions["Interact"];
//    //    }

//    private void OnTriggerStay(Collider other)
//    {
//        if (other.gameObject.CompareTag("Player"))
//        {
//            if (Vector3.Distance(_player.transform.position, transform.position) < 20)
//            {

//                // Debug.Log("1");

//                if (_collected) return;

//                // Debug.Log("2");

//                if (_interactAction == null) return;

//                // Debug.Log("3");

//                if (_interactAction.WasPressedThisFrame())
//                {

//                    CollectKeycard();
//                }
//            }
//            //else
//            //{
//            //    Debug.Log("NOT KEYCARD");
//            //}

//        }
//    }
//    //private void OnTriggerExit(Collider other)
//    //{
//    //    if (!other.CompareTag("Player")) return;
//    //    _interactAction = null;
//    //}

//    private void CollectKeycard()
//    {
//        _collected = true;

//        Debug.Log("KEYCARD COLLECTED");

//        if (_animationKeyCard != null)
//        {
//            _animationKeyCard.SetKeyCardActive(_keyCardIndex);
//            _goalManager.ShowNextGoal();
//        }
//        gameObject.SetActive(false);
//        //MeshRenderer renderer = GetComponent<MeshRenderer>();
//        //renderer.enabled = false;
//    }
//}
