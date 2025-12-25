
using UnityEngine;

public interface ICharacterInput
{
    public Vector2 Move { get; set; }
    public Vector2 Look { get; set; }
    public bool Run { get; set; }
    public bool Jump { get; set; }
    public bool Crouch { get; set; }
    public bool Draw { get; set; }
    public bool Attack { get; set; }
    
    public void ResetBufferedInput();
}
