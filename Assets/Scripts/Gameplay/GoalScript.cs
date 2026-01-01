using UnityEngine;

public class GoalScript : MonoBehaviour
{
    [SerializeField] private GoalManager _manager;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("GOAL COMPLETED");

        _manager.ShowNextGoal();
        gameObject.SetActive(false);
    }
}

//using UnityEngine;

//public class GoalScript : MonoBehaviour
//{

//    [SerializeField] GoalManager _manager;

//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {

//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }

//    private void OnTriggerEnter(Collider other)
//    {

//        Debug.Log("NEXT");
//        _manager.ShowNextGoal();


//        if (!other.CompareTag("Goal"))
//            Destroy(gameObject);


//    }

//}
