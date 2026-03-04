using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    [Header("旋转设置")]
    public float rotationSpeed = 90f; // 旋转速度
    [Header("浮动设置")]
    public float floatAmplitude = 0.1f; // 浮动幅度
    public float floatFrequency = 1f; // 浮动频率


    public BlockType blockType;
    public Material mate;

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

    void CreateFace(CubeSide side)
    {
        Mesh mesh = new Mesh();
        mesh.name = "S_Mesh" + side.ToString();

        Vector3[] vertices = new Vector3[4];
        int[] trangles = new int[6];
        Vector2[] uvs = new Vector2[4];
        Vector3[] normals = new Vector3[4];

        Vector3 p0 = new Vector3(-0.1f, -0.1f, 0.1f); // 左下前
        Vector3 p1 = new Vector3(0.1f, -0.1f, 0.1f); // 右下前
        Vector3 p2 = new Vector3(0.1f, -0.1f, -0.1f); // 右下后
        Vector3 p3 = new Vector3(-0.1f, -0.1f, -0.1f); // 左下后
        Vector3 p4 = new Vector3(-0.1f, 0.1f, 0.1f); // 左上前
        Vector3 p5 = new Vector3(0.1f, 0.1f, 0.1f); // 右上前
        Vector3 p6 = new Vector3(0.1f, 0.1f, -0.1f); // 右上后
        Vector3 p7 = new Vector3(-0.1f, 0.1f, -0.1f); // 左上后

        Vector2 uv0 = blockUVs[0, 0];
        Vector2 uv1 = blockUVs[0, 1];
        Vector2 uv2 = blockUVs[0, 2];
        Vector2 uv3 = blockUVs[0, 3];

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
        mesh.normals = normals;

        mesh.RecalculateBounds();

        GameObject quad = new GameObject("quad");
        quad.transform.parent = transform;
        quad.transform.localPosition = Vector3.zero;
        MeshFilter meshFilter = quad.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        //Debug.Log(quad.transform.position);
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

    public void Init(BlockType type)
    {
        blockType = type;
        foreach (CubeSide side in System.Enum.GetValues(typeof(CubeSide)))
        {
            CreateFace(side);
        }
        CombineBlockMesh();

    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        float newY = (Mathf.Sin(Time.time * floatFrequency) + 1) * floatAmplitude + 0.1f;
        transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
    }
}
