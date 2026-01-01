using UnityEngine;

public class WipeCursor : MonoBehaviour
{
    PoopManager _poopManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _poopManager = FindFirstObjectByType<PoopManager>();
    }
    private void OnMouseOver()
    {
        if (Input.GetMouseButton(0))
        {
            _poopManager.ChangeMouseCursor(2); // Change cursor to clicking state   
        }
        else
        {
            _poopManager.ChangeMouseCursor(1); // Change cursor to dragging state   
        }
    }
    private void OnMouseExit()
    {
        _poopManager.ChangeMouseCursor(0); // Change cursor to default state
    }
}
