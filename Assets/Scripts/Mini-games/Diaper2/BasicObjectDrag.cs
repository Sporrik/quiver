using UnityEngine;

public class BasicObjectDrag : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    private bool _isDragging = false;
    private float _zOffset;

    private void Update()
    {
        // Begin drag
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider == GetComponent<Collider>())
                {
                    _isDragging = true;
                    _zOffset = _camera.WorldToScreenPoint(transform.position).z; // Store depth
                }
            }
        }

        // End drag
        if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
        }

        // Dragging
        if (_isDragging)
        {
            Vector3 screenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _zOffset);
            Vector3 worldPos = _camera.ScreenToWorldPoint(screenPos);
            transform.position = worldPos;
        }
    }
}
