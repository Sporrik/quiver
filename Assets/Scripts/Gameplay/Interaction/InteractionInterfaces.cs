using UnityEngine;

namespace Gameplay.Interaction
{
    // Implement on things the player can use/open/pick up/ etc.
    public interface IInteractable
    {
        // Should this be able to be interacted with right now?
        bool CanInteract(Interactor interactor);

        // Perform the interaction
        void Interact(Interactor interactor);

        // Optional UI hint (return null to hide)
        string GetPrompt(Interactor interactor);
    }

    //Implement on targets that support silent takedowns
    public interface ITakedownTarget
    {
        bool CanTakedown(Interactor interactor);
        void Takedown(Interactor interactor);
    }

    // Lightweight context passed to targets instead of raw GameObjects
    public readonly struct Interactor
    {
        public readonly Transform Transform;
        public readonly GameObject GameObject;

        public Interactor(Transform t)
        {
            Transform = t;
            GameObject = t.gameObject;
        }
    }
}