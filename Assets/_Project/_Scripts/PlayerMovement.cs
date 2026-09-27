using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float moveSpeed = 5f;

    private void Update()
    {
        if (!IsOwner)
            return;

        Vector2 moveInput = GameInput.Instance.MoveInput.normalized;
        Vector3 movement = new Vector3(moveInput.x, moveInput.y, 0f);
        animator.SetBool("IsWalking", movement.sqrMagnitude > 0f);

        transform.position += movement * moveSpeed * Time.deltaTime;
    }
}