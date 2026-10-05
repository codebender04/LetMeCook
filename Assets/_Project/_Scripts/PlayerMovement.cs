using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private Animator animator;
    [Tooltip("Parent of the sprite and the HoldPoint. Flipped when moving left/right.")]
    [SerializeField] private Transform visual;
    [SerializeField] private BoxCollider2D box;
    [Tooltip("Layers that block the player (walls, counters). Do NOT include the Player layer.")]
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float moveSpeed = 5f;

    // Owner writes, everyone reads. Only changes when the player turns around.
    private readonly NetworkVariable<bool> facingLeft = new(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    /// <summary>Last direction the owner moved in. Used to aim interaction.</summary>
    public Vector2 FacingDir { get; private set; } = Vector2.down;

    private void Update()
    {
        // Every client applies the flip for every player.
        visual.localScale = new Vector3(facingLeft.Value ? -1f : 1f, 1f, 1f);

        if (!IsOwner)
            return;

        Vector2 input = GameInput.Instance.MoveInput.normalized;
        bool walking = input.sqrMagnitude > 0f;
        animator.SetBool("IsWalking", walking);

        if (!walking)
            return;

        FacingDir = input;

        if (Mathf.Abs(input.x) > 0.1f)
        {
            bool wantLeft = input.x < 0f;
            if (facingLeft.Value != wantLeft)
                facingLeft.Value = wantLeft;
        }

        Move(input * moveSpeed * Time.deltaTime);
    }

    // Moves one axis at a time so the player slides along walls instead of sticking.
    private void Move(Vector2 step)
    {
        Vector2 stepX = new Vector2(step.x, 0f);
        Vector2 stepY = new Vector2(0f, step.y);

        if (CanMove(stepX))
            transform.position += (Vector3)stepX;

        if (CanMove(stepY))
            transform.position += (Vector3)stepY;
    }

    // Only checks obstacleMask, so players walk straight through each other.
    private bool CanMove(Vector2 step)
    {
        if (step == Vector2.zero)
            return true;

        Vector2 center = transform.TransformPoint(box.offset);
        Vector2 size = Vector2.Scale(box.size, transform.lossyScale);

        RaycastHit2D hit = Physics2D.BoxCast(center, size, 0f, step.normalized, step.magnitude, obstacleMask);
        return hit.collider == null;
    }
}