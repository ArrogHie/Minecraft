using UnityEngine;

/// <summary>
/// 敌人生成器：根据配置在玩家周围自动生成敌人
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;

    public GameObject zombiePrefab;
    public GameObject skeletonPrefab;

    public int maxEnemies = 5;
    public float spawnRadiusMin = 8f;
    public float spawnRadiusMax = 15f;
    public float spawnCooldown = 5f;

    private float lastSpawnTime;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        lastSpawnTime = Time.time;
    }

    private void Update()
    {
        if (World.instance == null || World.instance.player == null) return;
        if (EnemyManager.instance == null) return;

        int currentEnemies = EnemyManager.instance.GetEnemyCount();

        if (currentEnemies < maxEnemies && Time.time - lastSpawnTime >= spawnCooldown)
        {
            SpawnEnemy();
            lastSpawnTime = Time.time;
        }
    }

    private void SpawnEnemy()
    {
        if (World.instance.player == null) return;

        Vector3 playerPos = World.instance.player.transform.position;
        float randomAngle = Random.Range(0f, 360f);
        float randomDistance = Random.Range(spawnRadiusMin, spawnRadiusMax);

        Vector3 spawnOffset = new Vector3(
            Mathf.Cos(randomAngle * Mathf.Deg2Rad) * randomDistance,
            0,
            Mathf.Sin(randomAngle * Mathf.Deg2Rad) * randomDistance
        );

        Vector3 spawnPos = playerPos + spawnOffset;
        spawnPos.y = GetGroundHeight(spawnPos);

        if (spawnPos.y < 0) spawnPos.y = 10;

        int randomType = Random.Range(0, 2);
        GameObject prefab = randomType == 0 ? zombiePrefab : skeletonPrefab;

        if (prefab != null)
        {
            Quaternion rotation = Quaternion.Euler(0, randomAngle, 0);
            Instantiate(prefab, spawnPos, rotation);
            Debug.Log($"Spawned {(randomType == 0 ? "Zombie" : "Skeleton")} at {spawnPos}");
        }
        else
        {
            Debug.LogWarning($"EnemySpawner: {(randomType == 0 ? "zombiePrefab" : "skeletonPrefab")} not set!");
        }
    }

    private float GetGroundHeight(Vector3 position)
    {
        if (World.instance == null) return 10f;

        int x = Mathf.FloorToInt(position.x);
        int z = Mathf.FloorToInt(position.z);

        Chunk chunk = World.instance.getChunk(new Vector2Int(
            Mathf.FloorToInt(x / 16f),
            Mathf.FloorToInt(z / 16f)
        ));

        if (chunk != null)
        {
            int localX = ((x % 16) + 16) % 16;
            int localZ = ((z % 16) + 16) % 16;

            for (int y = 47; y >= 0; y--)
            {
                Block block = chunk.GetBlock(new Vector3(localX, y, localZ));
                if (block != null && block.blockType != BlockType.Air)
                {
                    return y + 1;
                }
            }
        }

        return 10f;
    }
}
