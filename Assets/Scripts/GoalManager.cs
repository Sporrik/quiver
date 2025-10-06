using UnityEngine;

public class GoalManager : MonoBehaviour
{

    [SerializeField] GameObject[] Goals;
    public int _goalScore = 0;

    void Start()
    {
        foreach (GameObject go in Goals)
        {
            go.SetActive(false);
        }
        Goals[0].SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ShowNextGoal()
    {
        _goalScore++;
        if (_goalScore >= Goals.Length)
        {
            Debug.Log("YOU WIN");
            return;
        }
        Goals[_goalScore].SetActive(true);
    }

}
