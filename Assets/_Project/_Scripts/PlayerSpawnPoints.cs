using UnityEngine;

/// <summary>
/// Scene list of player spawn positions. Index = client id (host is 0).
/// </summary>
public class PlayerSpawnPoints : Singleton<PlayerSpawnPoints>
{
    [SerializeField] private Transform[] points;

    public Vector3 Get(ulong clientId)
    {
        if (points == null || points.Length == 0)
            return Vector3.zero;

        return points[(int)(clientId % (ulong)points.Length)].position;
    }
}
