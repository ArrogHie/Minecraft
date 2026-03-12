using UnityEngine;

public enum BlockType
{
    Air,
    Dirt,
    Grass,
    Stone,
    Wood,
    Leaves,
    Cobblestone,
    Planks,
    Stick,
    CraftingTable,
    Coal,
    Torch
}

public enum CubeSide
{
    Front,
    Back,
    Left,
    Right,
    Top,
    Bottom
}

public enum BlockFaceType
{
    Dirt,
    GrassSide,
    GrassTop,
    WoodSide,
    WoodTop,
    Sand,
    Stone,
    Leaves,
    Cobblestone,
    Planks,
    CraftingTableTop,
    CraftingTableSide1,
    CraftingTableSide2
}

public class Block
{
    public float durabilitySecond = 2.0f;
    //public ParticleSystem breakParticlePrefeb;
    //ParticleSystem breakParticleInstance;
    private float breakTime = 0f;
    private int lastBreakUV = 0;

    public Vector3 position;
    public BlockType blockType;
    public Chunk owner;

    public static Vector2[,] blockUVs =
    {
        /*Dirt*/        {new Vector2(0.1251f,0.9376f),new Vector2(0.1874f,0.9376f),new Vector2(0.1251f,0.9999f),new Vector2(0.1874f,0.9999f)},
        /*GrassSide*/   {new Vector2(0.1876f,0.9376f),new Vector2(0.2499f,0.9376f),new Vector2(0.1876f,0.9999f),new Vector2(0.2499f,0.9999f)},
        /*GrassTop*/    {new Vector2(0.0001f,0.9376f),new Vector2(0.0624f,0.9376f),new Vector2(0.0001f,0.9999f),new Vector2(0.0624f,0.9999f)},
        /*WoodSide*/    {new Vector2(0.2501f,0.8751f),new Vector2(0.3124f,0.8751f),new Vector2(0.2501f,0.9374f),new Vector2(0.3124f,0.9374f)},
        /*WoodTop*/     {new Vector2(0.3126f,0.8751f),new Vector2(0.3749f,0.8751f),new Vector2(0.3126f,0.9374f),new Vector2(0.3749f,0.9374f)},
        /*Sand*/        {new Vector2(0.1251f,0.8751f),new Vector2(0.1874f,0.8751f),new Vector2(0.1251f,0.9374f),new Vector2(0.1874f,0.9374f)},
        /*Stone*/       {new Vector2(0.0626f,0.9376f),new Vector2(0.1249f,0.9376f),new Vector2(0.0626f,0.9999f),new Vector2(0.1249f,0.9999f)},
        /*Leaves*/      {new Vector2(0.2501f,0.7501f),new Vector2(0.3124f,0.7501f),new Vector2(0.2501f,0.8124f),new Vector2(0.3124f,0.8124f)},
        /*Cobblestone*/ {new Vector2(0.0001f,0.8751f),new Vector2(0.0624f,0.8751f),new Vector2(0.0001f,0.9374f),new Vector2(0.0624f,0.9374f)},
        /*Planks*/      {new Vector2(0.2501f,0.9376f),new Vector2(0.3124f,0.9376f),new Vector2(0.2501f,0.9999f),new Vector2(0.3124f,0.9999f)},
        /*CraftingTableTop*/   {new Vector2(0.6876f,0.8126f),new Vector2(0.7499f,0.8126f),new Vector2(0.6876f,0.8749f),new Vector2(0.7499f,0.8749f)},
        /*CraftingTableSide1*/ {new Vector2(0.6876f,0.7501f),new Vector2(0.7499f,0.7501f),new Vector2(0.6876f,0.8124f),new Vector2(0.7499f,0.8124f)},
        /*CraftingTableSide2*/ {new Vector2(0.7501f,0.7501f),new Vector2(0.8124f,0.7501f),new Vector2(0.7501f,0.8124f),new Vector2(0.8124f,0.8124f)}
    };

    public static Vector2[,] healthUVs =
    {
        {new Vector2(0.2500f,0.2500f),new Vector2(0.2510f,0.25f),new Vector2(0.2500f,0.2510f),new Vector2(0.2510f,0.2510f)},
        {new Vector2(0.0000f,0.0000f),new Vector2(0.0625f,0.0000f),new Vector2(0.0000f,0.0625f),new Vector2(0.0625f,0.0625f)},
        {new Vector2(0.0625f,0.0000f),new Vector2(0.1250f,0.0000f),new Vector2(0.0625f,0.0625f),new Vector2(0.1250f,0.0625f)},
        {new Vector2(0.1250f,0.0000f),new Vector2(0.1875f,0.0000f),new Vector2(0.1250f,0.0625f),new Vector2(0.1875f,0.0625f)},
        {new Vector2(0.1875f,0.0000f),new Vector2(0.2500f,0.0000f),new Vector2(0.1875f,0.0625f),new Vector2(0.2500f,0.0625f)},
        {new Vector2(0.2500f,0.0000f),new Vector2(0.3125f,0.0000f),new Vector2(0.2500f,0.0625f),new Vector2(0.3125f,0.0625f)},
        {new Vector2(0.3125f,0.0000f),new Vector2(0.3750f,0.0000f),new Vector2(0.3125f,0.0625f),new Vector2(0.3750f,0.0625f)},
        {new Vector2(0.3750f,0.0000f),new Vector2(0.4375f,0.0000f),new Vector2(0.3750f,0.0625f),new Vector2(0.4375f,0.0625f)},
        {new Vector2(0.4375f,0.0000f),new Vector2(0.5000f,0.0000f),new Vector2(0.4375f,0.0625f),new Vector2(0.5000f,0.0625f)},
        {new Vector2(0.5000f,0.0000f),new Vector2(0.5625f,0.0000f),new Vector2(0.5000f,0.0625f),new Vector2(0.5625f,0.0625f)},
        {new Vector2(0.5625f,0.0000f),new Vector2(0.6250f,0.0000f),new Vector2(0.5625f,0.0625f),new Vector2(0.6250f,0.0625f)}
    };

    public Block(BlockType type, Chunk owner, Vector3 pos)
    {
        this.blockType = type;
        this.position = pos;
        this.owner = owner;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if (breakParticleInstance)
        //{
        //    if (lastBreakProgress < Time.time - .1f)
        //    {
        //        var emission = breakParticleInstance.emission;
        //        emission.enabled = false;
        //    }
        //}
    }

    public void CreateCube()
    {
        if (blockType == BlockType.Air) return;
        if (!owner.CheckFoxVoxel(position + Vector3.up))
            CreateFace(CubeSide.Top);
        if (!owner.CheckFoxVoxel(position + Vector3.down))
            CreateFace(CubeSide.Bottom);
        if (!owner.CheckFoxVoxel(position + Vector3.left))
            CreateFace(CubeSide.Left);
        if (!owner.CheckFoxVoxel(position + Vector3.right))
            CreateFace(CubeSide.Right);
        if (!owner.CheckFoxVoxel(position + Vector3.forward))
            CreateFace(CubeSide.Front);
        if (!owner.CheckFoxVoxel(position + Vector3.back))
            CreateFace(CubeSide.Back);
    }

    void CreateFace(CubeSide side)
    {
        CreateMesh(blockType, side, owner.transform, position, 1f, lastBreakUV);
    }

    public bool TryBreak(float breakSecond)
    {
        breakTime = breakSecond;

        if (Mathf.Clamp((int)(10.9f * breakTime / durabilitySecond), 0, 10) != lastBreakUV)
        {
            lastBreakUV = Mathf.Clamp((int)(10.9f * breakTime / durabilitySecond), 0, 10);
            owner.RedrawChunk();
        }

        if (breakSecond > durabilitySecond)
        {
            breakTime = 0f;
            lastBreakUV = 0;
            Break();
            return true;
        }
        return false;
    }

    private void Break()
    {
        owner.BreakBlock(position, blockType);
    }

    public static void CreateMesh(BlockType blockType, CubeSide side, Transform parent, Vector3 localPos, float size = 1f, int breakUV = 0)
    {
        Mesh mesh = new Mesh();
        mesh.name = "S_Mesh" + side.ToString();

        Vector3[] vertices = null;
        int[] trangles = null;
        Vector2[] uvs = null;
        Vector3[] normals = null;

        float s = size;
        Vector3 p0 = new Vector3(0, 0, s);
        Vector3 p1 = new Vector3(s, 0, s);
        Vector3 p2 = new Vector3(s, 0, 0);
        Vector3 p3 = new Vector3(0, 0, 0);
        Vector3 p4 = new Vector3(0, s, s);
        Vector3 p5 = new Vector3(s, s, s);
        Vector3 p6 = new Vector3(s, s, 0);
        Vector3 p7 = new Vector3(0, s, 0);

        Vector2 uv0 = blockUVs[0, 0];
        Vector2 uv1 = blockUVs[0, 1];
        Vector2 uv2 = blockUVs[0, 2];
        Vector2 uv3 = blockUVs[0, 3];

        Vector2 suvs0 = healthUVs[breakUV, 0];
        Vector2 suvs1 = healthUVs[breakUV, 1];
        Vector2 suvs2 = healthUVs[breakUV, 2];
        Vector2 suvs3 = healthUVs[breakUV, 3];

        if (blockType == BlockType.Grass)
        {
            if (side == CubeSide.Top)
            {
                uv0 = blockUVs[(int)BlockFaceType.GrassTop, 0];
                uv1 = blockUVs[(int)BlockFaceType.GrassTop, 1];
                uv2 = blockUVs[(int)BlockFaceType.GrassTop, 2];
                uv3 = blockUVs[(int)BlockFaceType.GrassTop, 3];
            }
            else if (side == CubeSide.Bottom)
            {
                uv0 = blockUVs[(int)BlockFaceType.Dirt, 0];
                uv1 = blockUVs[(int)BlockFaceType.Dirt, 1];
                uv2 = blockUVs[(int)BlockFaceType.Dirt, 2];
                uv3 = blockUVs[(int)BlockFaceType.Dirt, 3];
            }
            else
            {
                uv0 = blockUVs[(int)BlockFaceType.GrassSide, 0];
                uv1 = blockUVs[(int)BlockFaceType.GrassSide, 1];
                uv2 = blockUVs[(int)BlockFaceType.GrassSide, 2];
                uv3 = blockUVs[(int)BlockFaceType.GrassSide, 3];
            }
        }
        else if (blockType == BlockType.Wood)
        {
            if (side == CubeSide.Top || side == CubeSide.Bottom)
            {
                uv0 = blockUVs[(int)BlockFaceType.WoodTop, 0];
                uv1 = blockUVs[(int)BlockFaceType.WoodTop, 1];
                uv2 = blockUVs[(int)BlockFaceType.WoodTop, 2];
                uv3 = blockUVs[(int)BlockFaceType.WoodTop, 3];
            }
            else
            {
                uv0 = blockUVs[(int)BlockFaceType.WoodSide, 0];
                uv1 = blockUVs[(int)BlockFaceType.WoodSide, 1];
                uv2 = blockUVs[(int)BlockFaceType.WoodSide, 2];
                uv3 = blockUVs[(int)BlockFaceType.WoodSide, 3];
            }
        }
        else if (blockType == BlockType.CraftingTable)
        {
            if (side == CubeSide.Top || side == CubeSide.Bottom)
            {
                uv0 = blockUVs[(int)BlockFaceType.CraftingTableTop, 0];
                uv1 = blockUVs[(int)BlockFaceType.CraftingTableTop, 1];
                uv2 = blockUVs[(int)BlockFaceType.CraftingTableTop, 2];
                uv3 = blockUVs[(int)BlockFaceType.CraftingTableTop, 3];
            }
            else if (side == CubeSide.Front || side == CubeSide.Back)
            {
                uv0 = blockUVs[(int)BlockFaceType.CraftingTableSide1, 0];
                uv1 = blockUVs[(int)BlockFaceType.CraftingTableSide1, 1];
                uv2 = blockUVs[(int)BlockFaceType.CraftingTableSide1, 2];
                uv3 = blockUVs[(int)BlockFaceType.CraftingTableSide1, 3];
            }
            else
            {
                uv0 = blockUVs[(int)BlockFaceType.CraftingTableSide2, 0];
                uv1 = blockUVs[(int)BlockFaceType.CraftingTableSide2, 1];
                uv2 = blockUVs[(int)BlockFaceType.CraftingTableSide2, 2];
                uv3 = blockUVs[(int)BlockFaceType.CraftingTableSide2, 3];
            }
        }
        else if (blockType != BlockType.Air)
        {
            BlockFaceType faceType = (BlockFaceType)System.Enum.Parse(typeof(BlockFaceType), blockType.ToString());
            uv0 = blockUVs[(int)faceType, 0];
            uv1 = blockUVs[(int)faceType, 1];
            uv2 = blockUVs[(int)faceType, 2];
            uv3 = blockUVs[(int)faceType, 3];
        }

        switch (side)
        {
            case CubeSide.Bottom:
                vertices = new Vector3[] { p0, p1, p2, p3 };
                normals = new Vector3[] { Vector3.down, Vector3.down, Vector3.down, Vector3.down };
                uvs = new Vector2[] { uv0, uv1, uv3, uv2 };
                trangles = new int[] { 0, 2, 1, 0, 3, 2 };
                break;

            case CubeSide.Top:
                vertices = new Vector3[] { p7, p6, p5, p4 };
                normals = new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up };
                uvs = new Vector2[] { uv0, uv1, uv3, uv2 };
                trangles = new int[] { 0, 2, 1, 0, 3, 2 };
                break;

            case CubeSide.Left:
                vertices = new Vector3[] { p0, p3, p7, p4 };
                normals = new Vector3[] { Vector3.left, Vector3.left, Vector3.left, Vector3.left };
                uvs = new Vector2[] { uv0, uv1, uv3, uv2 };
                trangles = new int[] { 0, 2, 1, 0, 3, 2 };
                break;

            case CubeSide.Right:
                vertices = new Vector3[] { p2, p1, p5, p6 };
                normals = new Vector3[] { Vector3.right, Vector3.right, Vector3.right, Vector3.right };
                uvs = new Vector2[] { uv0, uv1, uv3, uv2 };
                trangles = new int[] { 0, 2, 1, 0, 3, 2 };
                break;

            case CubeSide.Front:
                vertices = new Vector3[] { p1, p0, p4, p5 };
                normals = new Vector3[] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward };
                uvs = new Vector2[] { uv0, uv1, uv3, uv2 };
                trangles = new int[] { 0, 2, 1, 0, 3, 2 };
                break;

            case CubeSide.Back:
                vertices = new Vector3[] { p3, p2, p6, p7 };
                normals = new Vector3[] { Vector3.back, Vector3.back, Vector3.back, Vector3.back };
                uvs = new Vector2[] { uv0, uv1, uv3, uv2 };
                trangles = new int[] { 0, 2, 1, 0, 3, 2 };
                break;
        }

        mesh.vertices = vertices;
        mesh.triangles = trangles;
        mesh.uv = uvs;
        mesh.SetUVs(1, new Vector2[] { suvs0, suvs1, suvs3, suvs2 });
        mesh.normals = normals;

        mesh.RecalculateBounds();

        GameObject quad = new GameObject("quad");
        quad.transform.parent = parent;
        quad.transform.localPosition = localPos;
        MeshFilter meshFilter = quad.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }

    public static void CombineMeshes(GameObject target, Material material)
    {
        MeshFilter[] meshFilters = target.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.parent.localToWorldMatrix.inverse * meshFilters[i].transform.localToWorldMatrix;
        }

        MeshFilter meshFilter = target.GetComponent<MeshFilter>();
        if(meshFilter == null) meshFilter = target.AddComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.CombineMeshes(combine);

        MeshRenderer meshRenderer=target.GetComponent<MeshRenderer>();
        if (meshRenderer == null) meshRenderer = target.AddComponent<MeshRenderer>();
        meshRenderer.material = material;

        foreach (Transform quad in target.transform)
        {
            UnityEngine.Object.Destroy(quad.gameObject);
        }
    }
}
