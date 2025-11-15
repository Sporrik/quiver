using UnityEngine;
using System.Collections.Generic;

public class MoveToObject : MonoBehaviour
{
    [SerializeField] private List<Transform> targetObjects; // List of target objects
    [SerializeField] private float moveDuration = 1f;
    [SerializeField] private bool moveOnStart = false;

    private Coroutine moveCoroutine;

    private void Start()
    {
        if (moveOnStart && targetObjects.Count > 0)
        {
            MoveTo(0); // Default to the first target if moveOnStart is true
        }
    }

    // Move to the target at the specified index
    public void MoveTo(int index)
    {
        if (index < 0 || index >= targetObjects.Count)
        {
            Debug.LogWarning("Invalid index passed to MoveTo: " + index);
            return;
        }

        Transform targetObject = targetObjects[index];
        if (targetObject != null)
        {
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }

            moveCoroutine = StartCoroutine(LerpToTarget(targetObject.position, moveDuration));
            Debug.Log("Moving to target: " + targetObject.name);
        }
    }

    private System.Collections.IEnumerator LerpToTarget(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = targetPosition;
    }
}
