using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public int maxDistance;
    public int blockSize;
    public int minHeight;
    public Blocks blocks;
    public PlayerControl player;

    private int seed;
    private Vector2Int playerBlock = new Vector2Int(int.MinValue, int.MinValue);
    private Dictionary<Vector2Int, GameObject> generatedBlocks = new Dictionary<Vector2Int, GameObject>();

    private void Awake()
    {
        seed = Random.Range(-10000, 10000);
    }

    private void Update()
    {
        Vector3 position = player.transform.position;
        Vector2Int blockPosition = new Vector2Int();
        blockPosition.x = (int)(position.x / blockSize);
        blockPosition.y = (int)(position.z / blockSize);

        if (blockPosition != playerBlock)
        {
            Debug.Log("Start Generate");
            playerBlock = blockPosition;
            Generate();
        }
    }

    private int GetChebyshevDis(Vector2Int a, Vector2Int b)
    {
        return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y));
    }

    private void Generate()
    {
        foreach (var block in generatedBlocks)
        {
            if (GetChebyshevDis(block.Key, playerBlock) > maxDistance)
            {
                Destroy(block.Value);
                generatedBlocks.Remove(block.Key);
            }
        }

        for (int x = playerBlock.x - maxDistance; x <= playerBlock.x + maxDistance; x++)
        {
            for (int y = playerBlock.y - maxDistance; y <= playerBlock.y + maxDistance; y++)
            {
                var blockPosition = new Vector2Int(x, y);
                if (generatedBlocks.ContainsKey(blockPosition)) continue;
                generatedBlocks[blockPosition] = GenerateBlock(blockPosition);
            }
        }
    }

    private GameObject GenerateBlock(Vector2Int blockPosition)
    {
        GameObject newBlock = new GameObject($"({blockPosition.x},{blockPosition.y})");
        newBlock.transform.parent = transform;

        for (int x = 0; x < blockSize; x++)
        {
            for (int y = 0; y < blockSize; y++)
            {
                int worldX = blockPosition.x * blockSize + x;
                int worldY = blockPosition.y * blockSize + y;
                int height = (int)(Mathf.PerlinNoise(worldX * 0.1f + seed, worldY * 0.1f + seed) * 10f);
                Block block = Instantiate(blocks.grass, new Vector3(worldX, height, worldY), Quaternion.identity);
                block.transform.parent = newBlock.transform;

                for (int h = height - 1; h >= height - 5; h--)
                {
                    block = Instantiate(blocks.dirt, new Vector3(worldX, h, worldY), Quaternion.identity);
                    block.transform.parent = newBlock.transform;
                }
                for (int h = height - 6; h >= minHeight; h--)
                {
                    block = Instantiate(blocks.stone, new Vector3(worldX, h, worldY), Quaternion.identity);
                    block.transform.parent = newBlock.transform;
                }
            }
        }
        return newBlock;
    }

    //private void CleanBlock(Vector2Int blockPosition)
    //{
    //    Destroy(transform.Find($"({blockPosition.x},{blockPosition.y})"));
    //}

    [System.Serializable]
    public struct Blocks
    {
        public Block dirt;
        public Block grass;
        public Block stone;
        public Block wood;
        public Block leaf;
    }
}
