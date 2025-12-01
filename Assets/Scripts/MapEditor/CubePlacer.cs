#if UNITY_EDITOR

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CubePlacer : MonoBehaviour
{
    [Header("prefabs")]
    [SerializeField] private GameObject prefabsObject;
    [SerializeField] private GameObject deletionObject;
    [Header("UI")]
    [SerializeField] private GameObject loadScriptObject;
    [SerializeField] private GameObject tileTextGameObject;

    private LoadData _loadDataScript;

    private GridScript _grid;
    private Camera _mainCamera;
    private GameObject _blueprintObject;

    private InputAction _rotateAction;
    private InputAction _leftSwitchAction;
    private InputAction _rightSwitchAction;
    private InputAction _deleteAction;
    private InputAction _switchInBound;

    private TextMeshProUGUI _tileTextMeshProUGUI;

    private List<List<GameObject>> _prefabsList = new List<List<GameObject>>();
    private int _currentIndex = 0;
    private int _isInBound = 0;

    private bool _deletionState = false;

    private void Awake()
    {

        _grid = FindFirstObjectByType<GridScript>();
        _mainCamera = Camera.main;

        _loadDataScript = loadScriptObject.GetComponent<LoadData>();

        _rotateAction = InputSystem.actions.FindAction("RotateTile");
        _leftSwitchAction = InputSystem.actions.FindAction("ChangeTileLeft");
        _rightSwitchAction = InputSystem.actions.FindAction("ChangeTileRight");
        _deleteAction = InputSystem.actions.FindAction("DeleteTile");
        _switchInBound = InputSystem.actions.FindAction("SwitchInBound");
    }

    private void Start()
    {
        if (tileTextGameObject != null) _tileTextMeshProUGUI = tileTextGameObject.GetComponent<TextMeshProUGUI>();

        for (var childIdx = 0; childIdx < prefabsObject.transform.childCount; childIdx++)
        {
            List<GameObject> tempList = new List<GameObject>();
            for (var idx = 0; idx < prefabsObject.transform.GetChild(childIdx).childCount; idx++)
            {
                var child = prefabsObject.transform.GetChild(childIdx).GetChild(idx).gameObject;
                tempList.Add(child);
            }
            _prefabsList.Add(tempList);
        }

        NewBlueprint();
    }

    // Update is called once per frame
    void Update()
    {
        GetListOutFile();

        RaycastHit hitInfo;
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitInfo))
        {
            PlaceCubeNear(hitInfo.point);
        }

        //rotate tile
        if (_rotateAction.triggered)
        {
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

            if (_currentIndex < 0) _currentIndex = _prefabsList[_isInBound].Count - 1;
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

            if (_currentIndex > _prefabsList[_isInBound].Count - 1) _currentIndex = 0;
            NewBlueprint();
        }

        if (_deleteAction.triggered)
        {
            _deletionState = !_deletionState;
            NewBlueprint();
        }

        if (_switchInBound.triggered)
        {
            if (_isInBound == 0)
            {
                _isInBound = 1;
            }
            else
            {
                _isInBound = 0;
            }

            _currentIndex = 0;
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

        if (_prefabsList[_isInBound][_currentIndex] != null && deletionObject != null)
        {
            if (!_deletionState)
            {
                _blueprintObject = Instantiate(_prefabsList[_isInBound][_currentIndex]);
            }
            else if (_deletionState)
            {
                _blueprintObject = Instantiate(deletionObject);
            }

            _tileTextMeshProUGUI.text = _prefabsList[_isInBound][_currentIndex].name;

        }
    }

    private void PlaceCubeNear(Vector3 nearPoint)
    {
        var finalPosition = _grid.GetNearestPointOnGrid(nearPoint);
        _blueprintObject.transform.position = finalPosition;

        if (Input.GetMouseButton(0))
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

                if (child.position == finalPosition) return;
            }
            //make new object and place on position
            if (_blueprintObject.transform.CompareTag("DeletionCube")) return;

            var prefabObject = Instantiate(_blueprintObject);

            prefabObject.transform.position = finalPosition;
            prefabObject.transform.SetParent(transform);
        }
    }

    private bool _mapDeleted = false;
    public void GetListOutFile()
    {
        if (_loadDataScript.GetFileSelected())
        {
            if (!_mapDeleted)
            {
                //discard previous map
                for (var idx = 0; idx < transform.childCount; idx++)
                {
                    var child = transform.GetChild(idx);
                    //remove all children
                    GameObject.Destroy(child.gameObject);
                }
                _mapDeleted = true;
            }
            Debug.Log("Setting map");

            //get new items
            TileDataListWrapper tileListWrapper = _loadDataScript.GetLoadedList();
            foreach (var tile in tileListWrapper.tiles)
            {
                for (var idx1 = 0; idx1 < _prefabsList.Count; idx1++)
                {
                    foreach (var prefab in _prefabsList[idx1])
                    {
                        if (prefab.CompareTag(tile.tagName))
                        {
                            //check if no blocks are on same position
                            for (var idx = 0; idx < transform.childCount; idx++)
                            {
                                var child = transform.GetChild(idx);

                                if (child.position == tile.position)
                                    return;
                            }
                            //if not continue
                            var prefabObject = Instantiate(prefab);
                            prefabObject.transform.position = tile.position;
                            prefabObject.transform.rotation = tile.rotation;
                            prefabObject.transform.SetParent(transform);
                        }
                    }
                }
            }
            _loadDataScript.ResetFileSelected();
            _mapDeleted = false;
        }
    }
}

#endif