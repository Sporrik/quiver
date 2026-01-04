using UnityEngine;
using UnityEngine.InputSystem;

public enum ControlScheme
{
    KeyboardMouse,
    Gamepad
}

public class PlayerInputBinder : MonoBehaviour
{
    public PlayerInput PlayerInput { get; private set; }
    public static ControlScheme CurrentScheme { get; private set; } = ControlScheme.KeyboardMouse;

    [SerializeField] private PlayerInputRelay _relay;
    public EdiblesManager Edibles { get; set; }
    public DragBaby DragBaby { get; set; }

    void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();
        UpdateSchemeFromPlayer();
    }

    void OnEnable()
    {
        if (PlayerInput != null)
        {
            PlayerInput.onActionTriggered += HandleAction;
            PlayerInput.onControlsChanged += HandleControlsChanged;
        }
    }

    void OnDisable()
    {
        if (PlayerInput != null)
        {
            PlayerInput.onActionTriggered -= HandleAction;
            PlayerInput.onControlsChanged -= HandleControlsChanged;
        }
    }

    private void HandleControlsChanged(PlayerInput pi)
    {
        UpdateSchemeFromPlayer();
    }

    private void UpdateSchemeFromPlayer()
    {
        string scheme = PlayerInput.currentControlScheme;
        if (scheme == "Gamepad")
            CurrentScheme = ControlScheme.Gamepad;
        else
            CurrentScheme = ControlScheme.KeyboardMouse;
    }

    private void HandleAction(InputAction.CallbackContext ctx)
    {
        string name = ctx.action.name;

        // Player
        if (_relay != null)
        {
            switch (name)
            {
                case "Move":     _relay.OnMove(ctx);     break;
                case "Sprint":   _relay.OnSprint(ctx);   break;
                case "Jump":     _relay.OnJump(ctx);     break;
                case "Interact": _relay.OnInteract(ctx); break;
                case "Takedown": _relay.OnTakedown(ctx); break;
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