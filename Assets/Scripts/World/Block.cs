using UnityEngine;

public enum BlockType
{
    Air,
    Dirt,
    Grass,
    Stone,
    Wood,
    Leaf,
    Cobblestone
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
    Cobblestone
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
        /*Dirt*/        {new Vector2(0.00f,0.75f),new Vector2(0.25f,0.75f),new Vector2(0.00f,1.00f),new Vector2(0.25f,1.00f)},
        /*GrassSide*/   {new Vector2(0.25f,0.75f),new Vector2(0.50f,0.75f),new Vector2(0.25f,1.00f),new Vector2(0.50f,1.00f)},
        /*GrassTop*/    {new Vector2(0.50f,0.75f),new Vector2(0.75f,0.75f),new Vector2(0.50f,1.00f),new Vector2(0.75f,1.00f)},
        /*WoodSide*/    {new Vector2(0.75f,0.75f),new Vector2(1.00f,0.75f),new Vector2(0.75f,1.00f),new Vector2(1.00f,1.00f)},
        /*WoodTop*/     {new Vector2(0.00f,0.50f),new Vector2(0.25f,0.50f),new Vector2(0.00f,0.75f),new Vector2(0.25f,0.75f)},
        /*Sand*/        {new Vector2(0.25f,0.50f),new Vector2(0.50f,0.50f),new Vector2(0.25f,0.75f),new Vector2(0.50f,0.75f)},
        /*Stone*/       {new Vector2(0.50f,0.50f),new Vector2(0.75f,0.50f),new Vector2(0.50f,0.75f),new Vector2(0.75f,0.75f)},
        /*Leaves*/      {new Vector2(0.75f,0.50f),new Vector2(1.00f,0.50f),new Vector2(0.75f,0.75f),new Vector2(1.00f,0.75f)},
        /*Cobblestone*/ {new Vector2(0.00f,0.25f),new Vector2(0.25f,0.25f),new Vector2(0.00f,0.50f),new Vector2(0.25f,0.50f)}
    };

    public static Vector2[,] healthUVs =
    {
        {new Vector2(0.50f,0.25f),new Vector2(0.75f,0.25f),new Vector2(0.50f,0.50f),new Vector2(0.75f,0.50f)},
        {new Vector2(0.75f,0.25f),new Vector2(1.00f,0.25f),new Vector2(0.75f,0.50f),new Vector2(1.00f,0.50f)},
        {new Vector2(0.00f,0.00f),new Vector2(0.25f,0.00f),new Vector2(0.00f,0.25f),new Vector2(0.25f,0.25f)},
        {new Vector2(0.25f,0.00f),new Vector2(0.50f,0.00f),new Vector2(0.25f,0.25f),new Vector2(0.50f,0.25f)},
        {new Vector2(0.50f,0.00f),new Vector2(0.75f,0.00f),new Vector2(0.50f,0.25f),new Vector2(0.75f,0.25f)},
        {new Vector2(0.75f,0.00f),new Vector2(1.00f,0.00f),new Vector2(0.75f,0.25f),new Vector2(1.00f,0.25f)}
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
        Mesh mesh = new Mesh();
        mesh.name = "S_Mesh" + side.ToString();

        Vector3[] vertices = new Vector3[4];
        int[] trangles = new int[6];
        Vector2[] uvs = new Vector2[4];
        Vector3[] normals = new Vector3[4];

        Vector3 p0 = new Vector3(0, 0, 1); // 左下前
        Vector3 p1 = new Vector3(1, 0, 1); // 右下前
        Vector3 p2 = new Vector3(1, 0, 0); // 右下后
        Vector3 p3 = new Vector3(0, 0, 0); // 左下后
        Vector3 p4 = new Vector3(0, 1, 1); // 左上前
        Vector3 p5 = new Vector3(1, 1, 1); // 右上前
        Vector3 p6 = new Vector3(1, 1, 0); // 右上后
        Vector3 p7 = new Vector3(0, 1, 0); // 左上后

        Vector2 uv0 = blockUVs[0, 0];
        Vector2 uv1 = blockUVs[0, 1];
        Vector2 uv2 = blockUVs[0, 2];
        Vector2 uv3 = blockUVs[0, 3];

        Vector2 suvs0 = healthUVs[lastBreakUV, 0];
        Vector2 suvs1 = healthUVs[lastBreakUV, 1];
        Vector2 suvs2 = healthUVs[lastBreakUV, 2];
        Vector2 suvs3 = healthUVs[lastBreakUV, 3];

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
                uvs = new Vector2[] { uv0, uv1, uv3, uv2 }; // uv0:左下, uv1:右下, uv3:右上, uv2:左上
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
        quad.transform.parent = owner.transform;
        quad.transform.localPosition = position;
        MeshFilter meshFilter = quad.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }

    public bool TryBreak(float breakSecond)
    {
        breakTime = breakSecond;
        //if (breakParticlePrefeb && !breakParticleInstance)
        //{
        //    Vector3 pos = this.position;
        //    pos += new Vector3(0.5f, 0.5f, 0.5f);
        //    breakParticleInstance = UnityEngine.Object.Instantiate(breakParticlePrefeb, pos, Quaternion.identity);
        //    breakParticleInstance.transform.parent = owner.transform;
        //}
        //var emission = breakParticleInstance.emission;
        //emission.enabled = true;
        if (Mathf.Clamp((int)(5.9f * breakTime / durabilitySecond), 0, 5) != lastBreakUV)
        {
            lastBreakUV = Mathf.Clamp((int)(5.9f * breakTime / durabilitySecond), 0, 5);
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
        //if (breakParticleInstance) Destroy(breakParticleInstance);
        //Destroy(gameObject);
        owner.SetBlockType(position, BlockType.Air);
    }
}
