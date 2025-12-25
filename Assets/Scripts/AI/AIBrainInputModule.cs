using UnityEngine;
using Zenject;

public class AIBrainInputModule : MonoBehaviour, ICharacterInput
{
    [field: SerializeField] public Character Character{ get; set; }
    private CharacterSelector _characterSelector;
    public bool Enabled { get; set; }
    public Vector2 Move { get; set; }
    public Vector2 Look { get; set; }
    public bool Run { get; set; }
    public bool Jump { get; set; }
    public bool Crouch { get; set; }
    public bool Draw { get; set; }
    public bool Attack { get; set; }

    public void ResetBufferedInput()
    {
        
    }

    public void Enable(bool value)
    {
        Enabled = value;
    }

    [Inject]
    private void Construct(CharacterSelector characterSelector)
    {
        _characterSelector = characterSelector;
        _characterSelector.Connect(this);
    }

    private void OnDestroy()
    {
        _characterSelector.Disconnect(this);
    }
}
