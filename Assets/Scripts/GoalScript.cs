using UnityEngine;

public class GoalScript : MonoBehaviour
{

    [SerializeField] GoalManager _manager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        
        Debug.Log("NEXT");
        _manager.ShowNextGoal();
        Destroy(gameObject);
    }

}
