using Unity.Netcode;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerInteraction : NetworkBehaviour, IItemHolder
{
    [SerializeField] private PlayerMovement movement;
    [Tooltip("Child of the Visual object, so it flips with the player.")]
    [SerializeField] private Transform holdPoint;
    [Tooltip("Layer(s) your counters are on.")]
    [SerializeField] private LayerMask counterMask;
    [SerializeField] private float reachDistance = 0.8f;
    [SerializeField] private float reachRadius = 0.5f;
    [Tooltip("Server rejects interactions from further away than this.")]
    [SerializeField] private float maxServerDistance = 2.5f;

    // Server writes, everyone reads. Lets every client pose the hands.
    private readonly NetworkVariable<bool> isHolding = new();

    public Transform HoldPoint => holdPoint;
    public bool IsHolding => isHolding.Value;

    private Item currentItem;
    public Item CurrentItem // server only
    {
        get => currentItem;
        set
        {
            currentItem = value;
            if (IsServer)
                isHolding.Value = value != null;
        }
    }

    private BaseCounter selected;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
            GameInput.Instance.OnInteract += HandleInteract;
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner && GameInput.Instance != null)
            GameInput.Instance.OnInteract -= HandleInteract;

        SetSelected(null);
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        SetSelected(FindCounter());
    }

    private BaseCounter FindCounter()
    {
        Vector2 center = (Vector2)transform.position + movement.FacingDir * reachDistance;
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, reachRadius, counterMask);

        BaseCounter best = null;
        float bestDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            BaseCounter counter = hit.GetComponentInParent<BaseCounter>();
            if (counter == null)
                continue;

            float distance = ((Vector2)counter.transform.position - center).sqrMagnitude;
            if (distance < bestDistance)
            {
                bestDistance = distance;
                best = counter;
            }
        }

        return best;
    }

    private void SetSelected(BaseCounter counter)
    {
        if (counter == selected)
            return;

        if (selected != null)
            selected.SetSelected(false);

        selected = counter;

        if (selected != null)
            selected.SetSelected(true);
    }

    private void HandleInteract()
    {
        if (selected == null)
            return;

        InteractServerRpc(selected.NetworkObject);
    }

    [Rpc(SendTo.Server)]
    private void InteractServerRpc(NetworkObjectReference counterRef)
    {
        if (!counterRef.TryGet(out NetworkObject counterObject))
            return;

        if (!counterObject.TryGetComponent(out BaseCounter counter))
            return;

        // Basic anti-cheat / lag sanity check.
        if (Vector2.Distance(transform.position, counter.transform.position) > maxServerDistance)
            return;

        counter.Interact(this);
    }
}