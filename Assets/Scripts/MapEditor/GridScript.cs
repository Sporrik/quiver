using System;
using UnityEngine;

public class GridScript : MonoBehaviour
{
    [SerializeField] private float size = 1.0f;
    [SerializeField] private float sizeGrid = 10.0f;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (float x = transform.position.x; x < (sizeGrid - MathF.Abs(transform.position.x)-1); x++)
        {
            for (float z = transform.position.z; z < (sizeGrid - MathF.Abs(transform.position.z)-1); z++)
            {
                var point = GetNearestPointOnGrid(new Vector3(x, 0.0f, z));
                Gizmos.DrawSphere(point,0.1f);
            }
        }
    }
}
