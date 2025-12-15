using UnityEngine;
using UnityEngine.UIElements;

public class MapNextGoal : MonoBehaviour
{
    Vector3 _currentGoalPos;
    [SerializeField] private GoalManager _goalManager;
    [SerializeField] private float _rotationSpeed;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(_goalManager._goalScore >= _goalManager.Goals.Length)
            return;
        // error out of array bounds


        _currentGoalPos = _goalManager.Goals[_goalManager._goalScore].transform.position;
        Vector3 direction = _currentGoalPos - transform.position;

        // Create the rotation we want to have
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        targetRotation *= Quaternion.Euler(0, -90, 0);

        // Rotate smoothly
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            _rotationSpeed * Time.deltaTime
        );
    }
}
