using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    private PlayerControls playerControls;
    public static event System.Action<Vector2> OnMovementEvent;
    public static event System.Action<bool> OnRunEvent;
    public static event System.Action<bool> OnShieldEvent;
    public static event System.Action<bool> OnJumpEvent;
    public static event System.Action<bool> OnQuickAttackEvent;
    public static event System.Action<bool> OnPowerAttackEvent;
    public static event System.Action<bool> OnSpecialAttackEvent;
    public static event System.Action<bool> OnPauseEvent;

    void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.PlayerMovement.Movement.performed += ctx => OnMovementEvent(ctx.ReadValue<Vector2>());
            playerControls.PlayerMovement.Movement.canceled += _ => OnMovementEvent(Vector2.zero);

            playerControls.PlayerActions.Run.performed += ctx => OnRunEvent(true);
            playerControls.PlayerActions.Run.canceled += ctx => OnRunEvent(false);

            playerControls.PlayerActions.Jump.performed += ctx => OnJumpEvent(true);
            playerControls.PlayerActions.Jump.canceled += ctx => OnJumpEvent(false);

            playerControls.PlayerActions.QuickAttack.performed += ctx => OnQuickAttackEvent(true);
            playerControls.PlayerActions.QuickAttack.canceled += ctx => OnQuickAttackEvent(false);

            playerControls.PlayerActions.PowerAttack.performed += ctx => OnPowerAttackEvent(true);
            playerControls.PlayerActions.PowerAttack.canceled += ctx => OnPowerAttackEvent(false);

            playerControls.PlayerActions.PowerUp.performed += ctx => OnSpecialAttackEvent(true);
            playerControls.PlayerActions.PowerUp.canceled += ctx => OnSpecialAttackEvent(false);

            playerControls.PlayerActions.Shield.performed += ctx => OnShieldEvent(true);
            playerControls.PlayerActions.Shield.canceled += ctx => OnShieldEvent(false);

            playerControls.OtherActions.Pause.performed += ctx => OnPauseEvent(true);
        }
        playerControls.Enable();
    }

    void OnDisable()
    {
        playerControls.Disable();
    }
}
