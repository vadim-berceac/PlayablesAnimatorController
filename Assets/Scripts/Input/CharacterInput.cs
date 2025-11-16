
using UnityEngine;

public interface ICharacterInput
{
    public Vector2 Move { get; set; }
    public bool Run { get; set; }
    public bool Jump { get; set; }
    
    public void ResetBufferedInput();
}
