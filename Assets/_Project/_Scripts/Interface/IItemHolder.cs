using Unity.Netcode;
using UnityEngine;
using static UnityEditor.Progress;

/// <summary>
/// Anything that can hold one Item: players and counters.
/// CurrentItem is only maintained on the server.
/// </summary>
public interface IItemHolder
{
    NetworkObject NetworkObject { get; }
    Transform HoldPoint { get; }
    Item CurrentItem { get; set; }
}