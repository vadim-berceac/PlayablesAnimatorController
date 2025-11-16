using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour, ICharacterInput
{
    [field: SerializeField] public InputActionAsset PlayerAction { get; set; }
    
    public Vector2 Move { get; set; }
    public bool Run { get; set; }
    public bool Jump { get; set; }
    
    private InputAction _moveAction;
    private InputAction _runAction;
    private InputAction _jumpAction;

    private void Awake()
    {
        _moveAction = PlayerAction.FindAction("Move");
        _runAction = PlayerAction.FindAction("Run");
        _jumpAction = PlayerAction.FindAction("Jump");

        Subscribe();
    }

    public void ResetBufferedInput()
    {
        Jump = false;
    }

    private void Subscribe()
    {
        _moveAction.performed += OnMove;
        _moveAction.canceled += OnMoveCancel;
        
        _runAction.performed += OnRun;
        _runAction.canceled += OnRunCancel;
        
        _jumpAction.performed += OnJump;
    }

    private void Unsubscribe()
    {
        _moveAction.performed -= OnMove;
        _moveAction.canceled -= OnMoveCancel;
        
        _runAction.performed -= OnRun;
        _runAction.canceled -= OnRunCancel;
        
        _jumpAction.performed -= OnJump;
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
}
