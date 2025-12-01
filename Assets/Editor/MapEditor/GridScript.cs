using System;
using UnityEngine;

public class GridScript : MonoBehaviour
{
    [SerializeField] private float size = 1.0f;
    [SerializeField] private float yChangeTest;

    public Vector3 GetNearestPointOnGrid(Vector3 position)
    {
        position -= transform.position;

        int xCount = Mathf.RoundToInt(position.x / size);
        int yCount = Mathf.RoundToInt(position.y / size);
        int zCount = Mathf.RoundToInt(position.z / size);

        Vector3 result = new Vector3((float)xCount * size, transform.position.y, (float)zCount * size);

        result += transform.position;

        return result;
    }
}
