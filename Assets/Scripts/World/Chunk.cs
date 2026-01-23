using System.Collections;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public static int chunkSize = 16;
    public static int chunkHeight = 16;
    public static int horizon = 8;
    public static float upAndDownRate = 8f;

    private Material mate;
    private int seed;

    private Block[,,] blocks;

    public void InitChunk(Material material, int seed)
    {
        this.mate = material;
    }

    public IEnumerator GenerateChunk()
    {
        CreateChunk();
        CreateBlock();
        CombineBlockMesh();
        CreatCollider();
        yield return null;
    }

    public void DrawChunk()
    {
        CreateBlock();
        CombineBlockMesh();
        CreatCollider();
    }

    public void RedrawChunk()
    {
        DestroyImmediate(gameObject.GetComponent<MeshFilter>());
        DestroyImmediate(gameObject.GetComponent<MeshRenderer>());
        DestroyImmediate(gameObject.GetComponent<MeshCollider>());
        DrawChunk();
    }

    private void CreateChunk()
    {
        blocks = new Block[chunkSize, chunkHeight, chunkSize];
        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                float worldX = transform.position.x + x;
                float worldZ = transform.position.z + z;
                int height = (int)(Mathf.PerlinNoise(worldX * 0.1f + seed, worldZ * 0.1f + seed) * upAndDownRate) + horizon;
                for (int y = 0; y < chunkHeight; y++)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    if (y > height)
                    {
                        blocks[x, y, z] = new Block(BlockType.Air, this, pos);
                    }
                    else if (y == height)
                    {
                        blocks[x, y, z] = new Block(BlockType.Grass, this, pos);
                    }
                    else if (y >= height - 3)
                    {
                        blocks[x, y, z] = new Block(BlockType.Dirt, this, pos);
                    }
                    else
                    {
                        blocks[x, y, z] = new Block(BlockType.Stone, this, pos);
                    }
                }
            }
        }
    }

    private void CreateBlock()
    {
        foreach (Block block in blocks)
        {
            block.CreateCube();
        }
    }

    private void CombineBlockMesh()
    {

    }

    private void CreatCollider()
    {

    }

    public bool CheckFoxVoxel(Vector3 pos)
    {
        if (pos.x < 0 || pos.x > chunkSize || pos.y < 0 || pos.y > chunkHeight || pos.z < 0 || pos.z > chunkSize) return false;
        return blocks[(int)pos.x, (int)pos.y, (int)pos.z].blockType != BlockType.Air;
    }
}
