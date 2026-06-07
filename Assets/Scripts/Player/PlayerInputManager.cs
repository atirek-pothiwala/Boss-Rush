using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    private PlayerControls playerControls;
    public static event System.Action<Vector2> OnMovementEvent;
    public static event System.Action<bool> OnRunEvent;
    public static event System.Action<bool> OnShieldEvent;
    public static event System.Action<bool> OnJumpEvent;
    public static event System.Action<bool> OnPunchEvent;
    public static event System.Action<bool> OnKickEvent;
    public static event System.Action<bool> OnChargeEvent;

    void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.PlayerMovement.Movement.performed += ctx => OnMovementEvent(ctx.ReadValue<Vector2>());

            playerControls.PlayerActions.Run.performed += ctx => OnRunEvent(true);
            playerControls.PlayerActions.Run.canceled += ctx => OnRunEvent(false);

            playerControls.PlayerActions.Jump.performed += ctx => OnJumpEvent(true);
            playerControls.PlayerActions.Jump.canceled += ctx => OnJumpEvent(false);

            playerControls.PlayerActions.Punch.performed += ctx => OnPunchEvent(true);
            playerControls.PlayerActions.Punch.canceled += ctx => OnPunchEvent(false);

            playerControls.PlayerActions.Kick.performed += ctx => OnKickEvent(true);
            playerControls.PlayerActions.Kick.canceled += ctx => OnKickEvent(false);

            playerControls.PlayerActions.Charge.performed += ctx => OnChargeEvent(true);
            playerControls.PlayerActions.Charge.canceled += ctx => OnChargeEvent(false);

            playerControls.PlayerActions.Shield.performed += ctx => OnShieldEvent(true);
            playerControls.PlayerActions.Shield.canceled += ctx => OnShieldEvent(false);
        }
        playerControls.Enable();
    }

    void OnDisable()
    {
        playerControls.Disable();
    }

}

