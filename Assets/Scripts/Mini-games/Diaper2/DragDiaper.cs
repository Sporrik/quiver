using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class DragDiaper : SimpleDrag
{
    [Header("Minigame specifics:")]
    [SerializeField] private PoopManager _poopManager; // Reference to the PoopManager

    [Header("Offset reference:")]
    [SerializeField] private Transform _meshTransform; // Reference to the mesh's transform
    private Vector3 meshOffset; // Offset between the rig and the mesh

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Calculate the offset between the rig's pivot and the mesh's pivot
        meshOffset = transform.position - _meshTransform.position;
    }

    private void Update()
    {
        ControllerInput();
        DragObject(meshOffset);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name == "DiaperGoal")
        {
            _isControllerDragging = false;
            _isMouseDragging = false;

            gameObject.GetComponent<MoveToObject>().MoveTo(1);
            gameObject.GetComponent<DiaperChangingBehavior>().enabled = true;

            _poopManager.CleanDiaperEquipped = true;

            Destroy(this);
        }
    }
}
