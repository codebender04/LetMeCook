using System;
using UnityEngine;

public class GameInput : Singleton<GameInput>
{
    private InputSystem_Actions actions;

    public event Action OnInteract;

    public Vector2 MoveInput => actions.Player.Move.ReadValue<Vector2>();

    protected override void Awake()
    {
        base.Awake();

        // A duplicate destroys itself in base.Awake, so don't set it up.
        if (Instance != this)
            return;

        actions = new InputSystem_Actions();
        actions.Player.Interact.performed += _ => OnInteract?.Invoke();
    }

    private void OnEnable()
    {
        actions?.Enable();
    }

    private void OnDisable()
    {
        actions?.Disable();
    }

    private void OnDestroy()
    {
        actions?.Dispose();
    }
}