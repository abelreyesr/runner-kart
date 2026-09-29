using UnityEngine;
using UnityEngine.Pool;

public class ItemSpawner : MonoBehaviour
{
    [Header("Items")]
    [SerializeField] private SpawnableItemData[] _items;
    private float[] _spawnTimes;

    [Header("Banana")]
    [SerializeField] private float _minBananaSpawnTime;
    [SerializeField] private float _maxBananaSpawnTime;
    [Range(0f, 1f)] [SerializeField] private float _doubleBananaChance;
    private float _bananaSpawnTime;

    [Header("Shell")]
    [SerializeField] private float _minShellSpawnTime;
    [SerializeField] private float _maxShellSpawnTime;
    private float _shellSpawnTime;

    [Header("Refs")]
    [SerializeField] private GameObject _banana;
    [SerializeField] private GameObject _shell;
    [SerializeField] private Transform _minSpawnPos;
    [SerializeField] private Transform _maxSpawnPos;
    [SerializeField] private PlayerController _player;

    private float _speedMult;

    void Awake()
    {
        _spawnTimes = new float[_items.Length];

        for (int i = 0; i < _items.Length; i++)
        {
            _spawnTimes[i] = Time.time + Random.Range(_items[i].minSpawnTime, _items[i].maxSpawnTime);
        }
    }

    private void Update()
    {
        // si vas muy lento el tiempo de spawn sigue siendo el mismo y se acumulan items arriba
        _speedMult = Mathf.Max(Mathf.InverseLerp(0f, _player.MaxSpeed, _player.Speed), 0.4f);

        for (int i = 0; i < _items.Length; i++)
        {
            HandleItemSpawn(i);
        }
    }

    private void HandleItemSpawn(int index)
    {
        SpawnableItemData data = _items[index];

        if (_spawnTimes[index] < Time.time)
        {
            int amount = Random.value <= data.doubleChance ? 2 : 1;

            for (int i = 0; i < amount; i++)
            {
                Vector3 pos = Vector3.Lerp(_minSpawnPos.position, _maxSpawnPos.position, Random.value);

                SpawnItem(data.item, pos);
            }

            _spawnTimes[index] = Time.time + (Random.Range(data.minSpawnTime, data.maxSpawnTime) / _speedMult);
        }
    }

    private void SpawnItem(GameObject prefab, Vector3 pos)
    {
        Item item = Instantiate(prefab, pos, Quaternion.identity).GetComponent<Item>();
        item.Init(_player);

        Destroy(item.gameObject, 10f);
    }
}
