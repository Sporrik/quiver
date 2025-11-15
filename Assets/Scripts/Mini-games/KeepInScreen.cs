using UnityEngine;

public class KeepInScreen : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    private MoveToObject moveToObject;
    [SerializeField] private int targetIndex = 0;

    private void Start()
    {
        moveToObject = GetComponent<MoveToObject>();
    }
    // Update is called once per frame
    void Update()
    {
        if (!IsObjectOnScreen())
        {
            moveToObject.MoveTo(targetIndex);
        }
    }

    private bool IsObjectOnScreen()
    {
        Vector3 viewportPosition = _camera.WorldToViewportPoint(transform.position);

        return viewportPosition.x >= 0 && viewportPosition.x <= 1 &&
               viewportPosition.y >= 0 && viewportPosition.y <= 1 &&
               viewportPosition.z > 0;
    }
}