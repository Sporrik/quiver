using System;
using System.Collections.Generic;
using UnityEngine;

public class CubePlacer : MonoBehaviour
{
    private Grid _grid;
    [SerializeField] private GameObject wallObject;

    private void Awake()
    {
        _grid = FindFirstObjectByType<Grid>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo))
            {
                PlaceCubeNear(hitInfo.point);
            }
        }
    }

    private void PlaceCubeNear(Vector3 nearPoint)
    {
        //if there is an instance on the position from nearPoint return, otherwise continue
        var finalPosition = _grid.GetNearestPointOnGrid(nearPoint);

        for (int idx = 0; idx < transform.childCount; idx++)
        {
            if (transform.GetChild(idx).position == finalPosition)
            {
                return;
            }
        }

        //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject cube = Instantiate(wallObject);
        cube.transform.position = finalPosition;
        cube.transform.parent = transform;
        Vector3 test = new Vector3(0, 1, 0);
        cube.transform.localScale += test;
    }
}
