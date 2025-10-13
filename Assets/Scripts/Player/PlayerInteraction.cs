using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public void Interact(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        //interaction suff here
    }
}
