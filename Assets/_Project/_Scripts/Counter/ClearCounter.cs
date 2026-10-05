using UnityEngine;

/// <summary>Plain counter: put one item down, pick it back up.</summary>
public class ClearCounter : BaseCounter
{
    public override void Interact(PlayerInteraction player)
    {
        if (CurrentItem == null)
        {
            if (player.CurrentItem != null)
                player.CurrentItem.SetHolder(this);
        }
        else
        {
            if (player.CurrentItem == null)
                CurrentItem.SetHolder(player);
        }
    }
}