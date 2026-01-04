using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputBinder : MonoBehaviour
{
    public PlayerInput PlayerInput { get; private set; }
    
    [SerializeField] private PlayerInputRelay _relay;
    public EdiblesManager Edibles { get; set; }      
    public DragBaby DragBaby { get; set; }

    void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();
    }

    void OnEnable()
    {
        if (PlayerInput != null)
            PlayerInput.onActionTriggered += HandleAction;
    }

    void OnDisable()
    {
        if (PlayerInput != null)
            PlayerInput.onActionTriggered -= HandleAction;
    }

    private void HandleAction(InputAction.CallbackContext ctx)
    {
        string name = ctx.action.name;

        // Player
        if (_relay != null)
        {
            switch (name)
            {
                case "Move":      _relay.OnMove(ctx);      break;
                case "Sprint":    _relay.OnSprint(ctx);    break;
                case "Jump":      _relay.OnJump(ctx);      break;
                case "Interact":  _relay.OnInteract(ctx);  break;
                case "Takedown":  _relay.OnTakedown(ctx);  break;
            }
        }
        
        // Hunger
        if (Edibles != null)
        {
            switch (name)
            {
                case "Discard1": Edibles.Discard1(ctx); break;
                case "Discard2": Edibles.Discard2(ctx); break;
                case "Discard3": Edibles.Discard3(ctx); break;
                case "Discard4": Edibles.Discard4(ctx); break;
            }
        }
        
        // Pee
        if (DragBaby != null)
        {
            switch (name)
            {
                case "DragBaby": DragBaby.OnMove(ctx); break;
            }
        }
    }
}
