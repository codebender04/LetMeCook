using UnityEngine;
using static UnityEditor.Progress;

[CreateAssetMenu(menuName = "Kitchen/Item", fileName = "NewItem")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public Sprite sprite;
    [Tooltip("Prefab with NetworkObject + Item. Must be in the NetworkManager prefab list.")]
    public Item prefab;
}