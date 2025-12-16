using UnityEngine;

public class GoalManager : MonoBehaviour
{
    [SerializeField] private ObjectiveUI _objectiveUIText;
    [SerializeField] public GameObject[] Goals;
  //  [SerializeField] GameObject _winFeedback;
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
        Goals[_goalScore].SetActive(false);
        _goalScore++;

        _objectiveUIText.OnGoalCompleted();

        if (_goalScore < Goals.Length)
        {
            Goals[_goalScore].SetActive(true);

        }

        //if (_goalScore >= Goals.Length)
        //{
        //    _winFeedback.SetActive(true);
        //    Debug.Log("YOU WIN");
        //    return;
        //}
    }

}
