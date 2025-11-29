using UnityEngine;

public class InputHandler
{
    private readonly ICharacterInput _characterInput;

    public InputHandler(ICharacterInput characterInput)
    {
        _characterInput = characterInput;
    }

    public Vector2 GetMoveInput()
    {
        return _characterInput.Move;
    }

    public bool GetRunInput()
    {
        return _characterInput.Run;
    }

    public bool GetJumpInput()
    {
        return _characterInput.Jump;
    }

    public bool GetCrouchInput()
    {
        return _characterInput.Crouch;
    }
    
    public void ResetBufferedInput()
    {
        _characterInput.ResetBufferedInput();
    }
}
