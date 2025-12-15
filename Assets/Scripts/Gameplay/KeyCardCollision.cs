using UnityEngine;

public class KeyCardCollision : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private AnimationKeyCard _animationKeyCard;
    [SerializeField] private GoalManager _goalManager;
    [SerializeField] private int _keyCardIndex;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("COLLIDED WITH KEY CARD");
        if (other.CompareTag("Player"))
        {
            Debug.Log("KEYCARD COLLECTED");
            if (_animationKeyCard != null)
            {
                _animationKeyCard.SetKeyCardActive(_keyCardIndex); // Assuming single keycard for simplicity
                _goalManager.ShowNextGoal();
            }
        }
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("COLLIDED WITH KEY CARD");
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        Debug.Log("KEYCARD COLLECTED");
    //        if (_animationKeyCard != null)
    //        {
    //            _animationKeyCard.SetKeyCardActive(0); // Assuming single keycard for simplicity
    //            _goalManager.ShowNextGoal();
    //        }
    //    }
    //}
}
