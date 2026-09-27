using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : Singleton<GameInput>
{
    private InputSystem_Actions actions;
    public Vector2 MoveInput => actions.Player.Move.ReadValue<Vector2>();

    protected override void Awake()
    {
        actions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        actions.Enable();
    }

    private void OnDisable()
    {
        actions.Disable();
    }
}