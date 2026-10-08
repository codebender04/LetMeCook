using Unity.Netcode;
using UnityEngine;

/// <summary>
/// A carryable object (ingredient, plate, tool). Not parented to its holder;
/// every client just follows the holder's HoldPoint each frame, so no
/// NetworkTransform is needed on items.
/// </summary>
[RequireComponent(typeof(NetworkObject))]
public class Item : NetworkBehaviour
{
    [SerializeField] private ItemSO itemSO;
    public ItemSO ItemSO => itemSO;

    // Server writes, everyone reads.
    private readonly NetworkVariable<bool> isHeld = new();
    private readonly NetworkVariable<NetworkObjectReference> holderRef = new();

    [Tooltip("Added to the sorting order while held, so the item draws over its holder.")]
    [SerializeField] private int heldSortingBoost = 1;

    private IItemHolder holder;   // server only
    private Transform follow;     // every client

    private SpriteRenderer[] renderers;
    private int[] baseSortingOrders;

    private void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>(true);
        baseSortingOrders = new int[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            baseSortingOrders[i] = renderers[i].sortingOrder;
    }

    public override void OnNetworkSpawn()
    {
        holderRef.OnValueChanged += (_, _) => follow = null;
        isHeld.OnValueChanged += (_, held) => ApplySorting(held);
        ApplySorting(isHeld.Value);
    }

    private void ApplySorting(bool held)
    {
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].sortingOrder = baseSortingOrders[i] + (held ? heldSortingBoost : 0);
    }

    /// <summary>Server only. Spawns a new item straight into a holder.</summary>
    public static Item Spawn(ItemSO so, IItemHolder target)
    {
        Item item = Instantiate(so.prefab, target.HoldPoint.position, Quaternion.identity);
        item.GetComponent<NetworkObject>().Spawn();
        item.SetHolder(target);
        return item;
    }

    /// <summary>Server only. Moves this item to a new holder (or null to drop it).</summary>
    public void SetHolder(IItemHolder newHolder)
    {
        if (!IsServer)
            return;

        if (holder != null)
            holder.CurrentItem = null;

        holder = newHolder;

        if (holder != null)
        {
            holder.CurrentItem = this;
            holderRef.Value = holder.NetworkObject;
            isHeld.Value = true;
        }
        else
        {
            isHeld.Value = false;
        }
    }

    /// <summary>Server only.</summary>
    public void DestroyItem()
    {
        if (!IsServer)
            return;

        SetHolder(null);
        NetworkObject.Despawn(true);
    }

    private void LateUpdate()
    {
        if (!isHeld.Value)
        {
            follow = null;
            return;
        }

        // The holder may not exist yet on a client that just joined, so keep retrying.
        if (follow == null
            && holderRef.Value.TryGet(out NetworkObject holderObject)
            && holderObject.TryGetComponent(out IItemHolder foundHolder))
        {
            follow = foundHolder.HoldPoint;
        }

        if (follow != null)
            transform.position = follow.position;
    }
}