using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.ProBuilder.Shapes;

public class CubePlacer : MonoBehaviour
{
    [SerializeField] private GameObject prefabsObject;
    [SerializeField] private GameObject deletionObject;
    [SerializeField] private Material blueprintMaterial;
    [SerializeField] private Material prefabMaterial;

    [SerializeField] private GameObject loadScriptObject;

    private LoadData _loadDataScript;

    private Grid _grid;
    private Camera _mainCamera;
    private GameObject _blueprintObject;

    private InputAction _rotateAction;
    private InputAction _leftSwitchAction;
    private InputAction _rightSwitchAction;
    private InputAction _deleteAction;

    private List<GameObject> _prefabsList = new List<GameObject>();
    private int _currentIndex = 0;

    private int _rotationAngle = 0;

    private bool _deletionState = false;

    private void Awake()
    {
        _grid = FindFirstObjectByType<Grid>();
        _mainCamera = Camera.main;

        _loadDataScript = loadScriptObject.GetComponent<LoadData>();

        _rotateAction = InputSystem.actions.FindAction("RotateTile");
        _leftSwitchAction = InputSystem.actions.FindAction("ChangeTileLeft");
        _rightSwitchAction = InputSystem.actions.FindAction("ChangeTileRight");
        _deleteAction = InputSystem.actions.FindAction("DeleteTile");
    }

    private void Start()
    {
        for (var idx = 0; idx < prefabsObject.transform.childCount; idx++)
        {
            var child = prefabsObject.transform.GetChild(idx);
            _prefabsList.Add(child.gameObject);
        }

        NewBlueprint();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hitInfo;
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitInfo))
        {
            PlaceCubeNear(hitInfo.point);
        }

        //rotate tile
        if (_rotateAction.triggered)
        {
            _rotationAngle += 90;
            if (_rotationAngle >= 360) _rotationAngle -= 360;

            const int rotateBlueprint = 90;
            _blueprintObject.transform.Rotate(new Vector3(0, 1, 0), rotateBlueprint);
        }

        if (_leftSwitchAction.triggered)
        {
            if (_deletionState)
            {
                _deletionState = !_deletionState;
            }
            else
            {
                _currentIndex--;
            }

            if (_currentIndex < 0) _currentIndex = _prefabsList.Count - 1;
            NewBlueprint();
        }

        if (_rightSwitchAction.triggered)
        {
            if (_deletionState)
            {
                _deletionState = !_deletionState;
            }
            else
            {
                _currentIndex++;
            }

            if (_currentIndex > _prefabsList.Count - 1) _currentIndex = 0;
            NewBlueprint();
        }

        if (_deleteAction.triggered)
        {
            _deletionState = !_deletionState;
            NewBlueprint();
        }
    }

    private void NewBlueprint()
    {
        if (_blueprintObject != null)
        {
            GameObject.Destroy(_blueprintObject);
            _blueprintObject = null;
        }

        if (_prefabsList[_currentIndex] != null && deletionObject != null)
        {
            if (!_deletionState)
            {
                _blueprintObject = Instantiate(_prefabsList[_currentIndex]);
            }
            else if (_deletionState)
            {
                _blueprintObject = Instantiate(deletionObject);
            }

            if (_blueprintObject != null)
            {
                for (var idx = 0; idx < _blueprintObject.transform.childCount; idx++)
                {
                    MeshRenderer meshRender = _blueprintObject.transform.GetChild(idx).GetComponent<MeshRenderer>();
                    meshRender.material = blueprintMaterial;
                }
            }

        }
    }

    private void PlaceCubeNear(Vector3 nearPoint)
    {
        //if there is an instance on the position from nearPoint return, otherwise continue
        var finalPosition = _grid.GetNearestPointOnGrid(nearPoint);
        _blueprintObject.transform.position = finalPosition;

        if (Input.GetMouseButtonDown(0))
        {
            //check for filled tile
            for (var idx = 0; idx < transform.childCount; idx++)
            {
                var child = transform.GetChild(idx);

                //Skip the blueprint if it's in hierarchy
                if (_blueprintObject.transform.CompareTag("DeletionCube") && (child.position == _blueprintObject.transform.position))
                {
                    GameObject.Destroy(child.gameObject);
                    return;
                }

                if (child.gameObject == _blueprintObject) continue;

                if (child.position == finalPosition)
                    return;
            }
            //make new object and place on position
            if (_blueprintObject.transform.CompareTag("DeletionCube")) return;


            var prefabObject = Instantiate(_blueprintObject);

            for (var idx = 0; idx < prefabObject.transform.childCount; idx++)
            {
                MeshRenderer meshRender = prefabObject.transform.GetChild(idx).GetComponent<MeshRenderer>();
                meshRender.material = prefabMaterial;
            }

            prefabObject.transform.position = finalPosition;
            prefabObject.transform.parent = transform;

        }
    }

    private void GetListOutFile()
    {
        //TODO: get text value from json loadData, compare for matching tag, then instantiate the prefab with the given position and rotation

        TileDataListWrapper tileListWrapper = _loadDataScript.GetLoadedList();

        for (var idx = 0; idx < _prefabsList.Count; idx++)
        {
            if (_prefabsList[idx].CompareTag(tileListWrapper.tiles[idx].tagName))
            {

            }
        }
    }
}
