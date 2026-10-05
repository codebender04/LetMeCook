using UnityEngine;

/// <summary>Destroys whatever the player is holding.</summary>
public class TrashCounter : BaseCounter
{
    public override void Interact(PlayerInteraction player)
    {
        if (player.CurrentItem != null)
            player.CurrentItem.DestroyItem();
    }
}