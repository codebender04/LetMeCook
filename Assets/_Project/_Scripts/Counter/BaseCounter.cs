using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Base for every station. Interact/InteractAlternate run on the SERVER only.
/// </summary>
public abstract class BaseCounter : NetworkBehaviour, IItemHolder
{
    [SerializeField] private Transform holdPoint;
    [Tooltip("Outline or glow child object, shown when a player is aiming at this counter.")]
    [SerializeField] private GameObject selectedVisual;

    public Transform HoldPoint => holdPoint;
    public Item CurrentItem { get; set; }

    public abstract void Interact(PlayerInteraction player);

    public virtual void InteractAlternate(PlayerInteraction player) { }

    // Local visual only, not networked.
    public void SetSelected(bool selected)
    {
        if (selectedVisual != null)
            selectedVisual.SetActive(selected);
    }
}