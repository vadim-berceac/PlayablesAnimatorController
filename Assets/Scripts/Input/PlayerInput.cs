using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour, ICharacterInput
{
    [field: SerializeField] public InputActionAsset PlayerAction { get; set; }
    
    public Vector2 Move { get; set; }
    public bool Run { get; set; }
    public bool Jump { get; set; }
    public bool Crouch { get; set; }
    public bool Draw { get; set; }
    public bool Attack { get; set; }
    
    private InputAction _moveAction;
    private InputAction _runAction;
    private InputAction _jumpAction;
    private InputAction _crouchAction;
    private InputAction _drawAction;
    private InputAction _attackAction;

    private void Awake()
    {
        _moveAction = PlayerAction.FindAction("Move");
        _runAction = PlayerAction.FindAction("Run");
        _jumpAction = PlayerAction.FindAction("Jump");
        _crouchAction = PlayerAction.FindAction("Crouch");
        _drawAction = PlayerAction.FindAction("Draw");
        _attackAction = PlayerAction.FindAction("Attack");

        Subscribe();
    }

    public void ResetBufferedInput()
    {
        Jump = false;
        Draw = false;
        Attack = false;
    }

    private void Subscribe()
    {
        _moveAction.performed += OnMove;
        _moveAction.canceled += OnMoveCancel;
        
        _runAction.performed += OnRun;
        _runAction.canceled += OnRunCancel;
        
        _jumpAction.performed += OnJump;
        _drawAction.performed += OnDraw;
        _attackAction.performed += OnAttack;
        
        _crouchAction.performed += OnCrouch;
        _crouchAction.canceled += OnCrouchCancel;
    }

    private void Unsubscribe()
    {
        _moveAction.performed -= OnMove;
        _moveAction.canceled -= OnMoveCancel;
        
        _runAction.performed -= OnRun;
        _runAction.canceled -= OnRunCancel;
        
        _jumpAction.performed -= OnJump;
        _drawAction.performed -= OnDraw;
        _attackAction.performed -= OnAttack;
        
        _crouchAction.performed -= OnCrouch;
        _crouchAction.canceled -= OnCrouchCancel;
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>();
    }
    
    private void OnMoveCancel(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>();
    }

    private void OnRun(InputAction.CallbackContext context)
    {
        Run = context.ReadValueAsButton();
    }
    
    private void OnRunCancel(InputAction.CallbackContext context)
    {
        Run = context.ReadValueAsButton();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        Jump = true;
    }

    private void OnDraw(InputAction.CallbackContext context)
    {
        Draw = true;
    }
    
    private void OnAttack(InputAction.CallbackContext context)
    {
        Attack = true;
    }

    private void OnCrouch(InputAction.CallbackContext context)
    {
        Crouch = context.ReadValueAsButton();
    }

    private void OnCrouchCancel(InputAction.CallbackContext context)
    {
        Crouch = context.ReadValueAsButton();
    }
}
