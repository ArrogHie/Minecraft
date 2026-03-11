using System.Collections;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public static int chunkSize = 16;
    public static int chunkHeight = 48;
    public static int horizon = 8;
    public static float upAndDownRate = 16f;
    public static int treeDensity = 2;

    public Vector2Int chunkPos;
    private Material mate;
    private int seed;

    private Block[,,] blocks;

    public void InitChunk(Material material, int seed, Vector2Int pos)
    {
        this.mate = material;
        this.seed = seed;
        this.chunkPos = pos;
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

    public bool SetBlockType(Vector3 pos, BlockType blockType)
    {
        Block block = GetBlock(pos);
        if (block != null)
        {
            if (blockType != BlockType.Air)
            {
                Vector3 targetPos = pos + transform.position;
                targetPos += new Vector3(0.5f, 0.5f, 0.5f);
                Vector3 box = new Vector3(0.49f, 0.49f, 0.49f);
                if (Physics.CheckBox(targetPos, box, Quaternion.identity, LayerMask.GetMask("Player"))) return false;
            }
            block.blockType = blockType;
            RedrawChunk();
            return true;
        }
        return false;
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
                int height = (int)(Mathf.PerlinNoise(worldX / 30f + seed, worldZ / 30f + seed) * upAndDownRate) + horizon;
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

        int treeCount = Random.Range(0, treeDensity * 2);
        for (int i = 0; i < treeCount; i++)
        {
            int x = Random.Range(2, chunkSize - 2);
            int z = Random.Range(2, chunkSize - 2);
            for (int y = chunkHeight - 1; y >= 0; y--)
            {
                if (blocks[x, y, z].blockType == BlockType.Grass)
                {
                    CreateTree(new Vector3(x, y + 1, z));
                    break;
                }
            }
        }
    }

    private void CreateTree(Vector3 pos)
    {
        int height = Random.Range(3, 6);
        for (int y = 0; y < height; y++)
        {
            if (pos.y + y < chunkHeight)
            {
                blocks[(int)pos.x, (int)pos.y + y, (int)pos.z].blockType = BlockType.Wood;
            }
        }
        for (int x = -2; x <= 2; x++)
        {
            for (int y = height - 2; y <= height - 1; y++)
            {
                for (int z = -2; z <= 2; z++)
                {
                    if (Mathf.Abs(x) + Mathf.Abs(z) == 4 && Random.Range(0, 5) == 0) // �������ϡ���Ҷ��
                        continue;
                    if (pos.y + y < chunkHeight)
                    {
                        if (blocks[(int)pos.x + x, (int)pos.y + y, (int)pos.z + z].blockType == BlockType.Air)
                        {
                            blocks[(int)pos.x + x, (int)pos.y + y, (int)pos.z + z].blockType = BlockType.Leaves;
                        }
                    }
                }
            }
        }

        for (int x = -1; x <= 1; x++)
        {
            for (int y = 0; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x != 0 && z != 0) continue; // ֻ��������Χ����Ҷ��
                    if (y == 1 && Random.Range(0, 8) == 0) // �������ϡ���Ҷ��
                        continue;
                    if (pos.y + height + y < chunkHeight)
                    {
                        if (blocks[(int)pos.x + x, (int)pos.y + y + height, (int)pos.z + z].blockType == BlockType.Air)
                        {
                            blocks[(int)pos.x + x, (int)pos.y + y + height, (int)pos.z + z].blockType = BlockType.Leaves;
                        }
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
        Block.CombineMeshes(gameObject, mate);
    }

    private void CreatCollider()
    {
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        meshCollider.sharedMesh = mesh;
    }

    public Block GetBlock(Vector3 pos)
    {
        if (pos.x < 0 || pos.x >= chunkSize || pos.y < 0 || pos.y >= chunkHeight || pos.z < 0 || pos.z >= chunkSize) return null;
        return blocks[(int)pos.x, (int)pos.y, (int)pos.z];
    }

    public bool CheckFoxVoxel(Vector3 pos)
    {
        if (pos.x < 0 || pos.x >= chunkSize || pos.y < 0 || pos.y >= chunkHeight || pos.z < 0 || pos.z >= chunkSize) return false;
        BlockType type = blocks[(int)pos.x, (int)pos.y, (int)pos.z].blockType;
        return type != BlockType.Air && type != BlockType.Leaves;
    }

    public bool HasLeafNeighbor(Vector3 pos)
    {
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
        foreach (Vector3 dir in directions)
        {
            Vector3 checkPos = pos + dir;
            if (checkPos.x >= 0 && checkPos.x < chunkSize && checkPos.y >= 0 && checkPos.y < chunkHeight && checkPos.z >= 0 && checkPos.z < chunkSize)
            {
                if (blocks[(int)checkPos.x, (int)checkPos.y, (int)checkPos.z].blockType == BlockType.Leaves)
                    return true;
            }
        }
        return false;
    }

    public void BreakBlock(Vector3 position, BlockType type)
    {
        SetBlockType(position, BlockType.Air);
        if (type == BlockType.Stone) type = BlockType.Cobblestone;
        if (type != BlockType.Leaves)
            World.instance.CreatDrop(transform.position + position + new Vector3(0.5f, 0.5f, 0.5f), type);
    }
}
