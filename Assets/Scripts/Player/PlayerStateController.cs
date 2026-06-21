using UnityEngine;

public class PlayerStateController : MonoBehaviour
{
    [Header("Inputs")]
    public Vector2 MovementInput { get; private set; }
    public bool OnRun { get; private set; }
    public bool OnPunch { get; private set; }
    public bool OnKick { get; private set; }
    public bool OnJump { get; private set; }
    public bool OnShield { get; private set; }
    public bool OnCharge { get; private set; }
    public bool IsGrounded { get; private set; }
    

    public PlayerState CurrentState { get; private set; }

    private void OnMovementEvent(Vector2 value)
    {
        value.y = 0f; // Ensure vertical input doesn't affect movement state
        MovementInput = value;
    }
    private void OnRunEvent(bool value) => OnRun = value;
    private void OnPunchEvent(bool value) => OnPunch = value;
    private void OnKickEvent(bool value) => OnKick = value;
    private void OnJumpEvent(bool value) => OnJump = value;
    private void OnShieldEvent(bool value) => OnShield = value;
    private void OnChargeEvent(bool value) => OnCharge = value;

    void OnEnable()
    {
        PlayerInputManager.OnMovementEvent += OnMovementEvent;
        PlayerInputManager.OnRunEvent += OnRunEvent;
        PlayerInputManager.OnPunchEvent += OnPunchEvent;
        PlayerInputManager.OnKickEvent += OnKickEvent;
        PlayerInputManager.OnJumpEvent += OnJumpEvent;
        PlayerInputManager.OnShieldEvent += OnShieldEvent;
        PlayerInputManager.OnChargeEvent += OnChargeEvent;
    }

    void OnDisable()
    {
        PlayerInputManager.OnMovementEvent -= OnMovementEvent;
        PlayerInputManager.OnRunEvent -= OnRunEvent;
        PlayerInputManager.OnPunchEvent -= OnPunchEvent;
        PlayerInputManager.OnKickEvent -= OnKickEvent;
        PlayerInputManager.OnJumpEvent -= OnJumpEvent;
        PlayerInputManager.OnShieldEvent -= OnShieldEvent;
        PlayerInputManager.OnChargeEvent -= OnChargeEvent;
    }

    void Awake()
    {
        CurrentState = PlayerState.Idle;
    }

    public void RefreshState()
    {
        if(!IsGrounded)
        {
            CurrentState = PlayerState.Fall;
            return;
        }

        if (OnRun && MovementInput.magnitude > 0f)
        {
            CurrentState = PlayerState.Run;
        } 
        else if (MovementInput.magnitude > 0f)
        {
            CurrentState = PlayerState.Walk;
        }
        else
        {
            if (OnJump)
            {
                CurrentState = PlayerState.Jump;
            } 
            else if (OnPunch && OnKick)
            {   
                CurrentState = PlayerState.Power;
            } 
            else if (OnPunch)
            {
                CurrentState = PlayerState.Punch;
            } 
            else if (OnKick)
            {
                CurrentState = PlayerState.Kick;
            }
            else if (OnCharge)
            {
                CurrentState = PlayerState.Charge;
            } 
            else if (OnShield)
            {
                CurrentState = PlayerState.Shield;
            } 
            else
            {
                CurrentState = PlayerState.Idle;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground")) IsGrounded = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) IsGrounded = false;
    }
}