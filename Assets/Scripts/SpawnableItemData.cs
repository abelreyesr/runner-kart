using UnityEngine;

[CreateAssetMenu()]
public class SpawnableItemData : ScriptableObject
{
    public GameObject item;
    public float minSpawnTime;
    public float maxSpawnTime;
    [Range(0f, 1f)] public float doubleChance;
}
