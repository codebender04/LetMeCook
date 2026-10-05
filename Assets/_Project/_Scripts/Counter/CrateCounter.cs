using UnityEngine;

/// <summary>Fridge / crate with an infinite supply of one item.</summary>
public class CrateCounter : BaseCounter
{
    [SerializeField] private ItemSO itemSO;

    public override void Interact(PlayerInteraction player)
    {
        if (player.CurrentItem == null)
            Item.Spawn(itemSO, player);
    }
}