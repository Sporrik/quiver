using UnityEngine;

public class GoalManager : MonoBehaviour
{

    [SerializeField] public GameObject[] Goals;
    [SerializeField] GameObject _winFeedback;
    public int _goalScore = 0;
    [SerializeField] private ObjectiveUIManager _objectiveUI;

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

        _objectiveUI.OnGoalCompleted();

        if (_goalScore >= Goals.Length)
        {
            _winFeedback.SetActive(true);
            Debug.Log("YOU WIN");
            return;
        }
        Goals[_goalScore].SetActive(true);
    }

}
