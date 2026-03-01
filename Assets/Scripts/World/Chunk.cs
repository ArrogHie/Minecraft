using System.Collections;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public static int chunkSize = 16;
    public static int chunkHeight = 24;
    public static int horizon = 8;
    public static float upAndDownRate = 16f;

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
                Vector3 box = new Vector3(0.5f, 0.5f, 0.5f);
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
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.parent.localToWorldMatrix.inverse * meshFilters[i].transform.localToWorldMatrix;
        }

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.CombineMeshes(combine);

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = mate;

        foreach (Transform quad in transform)
        {
            Destroy(quad.gameObject);
        }
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
        return blocks[(int)pos.x, (int)pos.y, (int)pos.z].blockType != BlockType.Air;
    }

    public void BreakBlock(Vector3 position, BlockType type)
    {
        SetBlockType(position, BlockType.Air);
        World.instance.CreatDrop(transform.position + position + new Vector3(0.5f, 0.5f, 0.5f), type);
    }

    //void OnDrawGizmos()
    //{
    //    // 设置侧面颜色（例如半透明红色，方便识别边界）
    //    Gizmos.color = new Color(1f, 0f, 0f, 0.2f);

    //    Vector3 pos = transform.position;

    //    // --- 1. 绘制 X 轴最小侧面 (Left) ---
    //    // 位置在区块左侧中心，宽度近乎0，高度为chunkHeight，深度为chunkSize
    //    Vector3 leftFaceCenter = pos + new Vector3(0, chunkHeight / 2.0f, chunkSize / 2.0f);
    //    Gizmos.DrawCube(leftFaceCenter, new Vector3(0.01f, chunkHeight, chunkSize));

    //    // --- 2. 绘制 X 轴最大侧面 (Right) ---
    //    Vector3 rightFaceCenter = pos + new Vector3(chunkSize, chunkHeight / 2.0f, chunkSize / 2.0f);
    //    Gizmos.DrawCube(rightFaceCenter, new Vector3(0.01f, chunkHeight, chunkSize));

    //    // --- 3. 绘制 Z 轴最小侧面 (Back) ---
    //    Vector3 backFaceCenter = pos + new Vector3(chunkSize / 2.0f, chunkHeight / 2.0f, 0);
    //    Gizmos.DrawCube(backFaceCenter, new Vector3(chunkSize, chunkHeight, 0.01f));

    //    // --- 4. 绘制 Z 轴最大侧面 (Front) ---
    //    Vector3 frontFaceCenter = pos + new Vector3(chunkSize / 2.0f, chunkHeight / 2.0f, chunkSize);
    //    Gizmos.DrawCube(frontFaceCenter, new Vector3(chunkSize, chunkHeight, 0.01f));

    //    // 可选：画出边框线让边缘更清晰
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireCube(pos + new Vector3(chunkSize / 2f, chunkHeight / 2f, chunkSize / 2f),
    //                        new Vector3(chunkSize, chunkHeight, chunkSize));
    //}
}
