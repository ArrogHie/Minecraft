using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BlockIconGenerator : MonoBehaviour
{
    public static BlockIconGenerator Instance { get; private set; }

    public Material blockMaterial;
    public int iconSize = 64;

    private Dictionary<string, Sprite> iconCache = new Dictionary<string, Sprite>();
    private GameObject tempBlock;
    private Camera iconCamera;
    private RenderTexture renderTexture;

    private void Awake()
    {
        Instance = this;
    }

    private void InitializeIfNeeded()
    {
        if (iconCamera != null) return;

        SetupCamera();
    }

    private void SetupCamera()
    {
        GameObject cameraObj = new GameObject("IconCamera");
        cameraObj.transform.SetParent(transform);
        cameraObj.transform.position = new Vector3(2, 2, -2);
        cameraObj.transform.LookAt(Vector3.zero);

        iconCamera = cameraObj.AddComponent<Camera>();
        iconCamera.orthographic = true;
        iconCamera.orthographicSize = 0.9f;
        iconCamera.cullingMask = 1 << LayerMask.NameToLayer("IconLayer");
        iconCamera.enabled = false;
        iconCamera.clearFlags = CameraClearFlags.SolidColor;
        iconCamera.backgroundColor = new Color(0, 0, 0, 0);

        renderTexture = new RenderTexture(iconSize, iconSize, 16);
        iconCamera.targetTexture = renderTexture;

        tempBlock = new GameObject("TempBlock");
        tempBlock.transform.SetParent(transform);
        tempBlock.transform.position =new Vector3(-1,-1,0);
        tempBlock.layer = LayerMask.NameToLayer("IconLayer");
    }

    private void GenerateIcon(BlockType type)
    {
        foreach (Transform child in tempBlock.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (CubeSide side in System.Enum.GetValues(typeof(CubeSide)))
        {
            Block.CreateMesh(type, side, tempBlock.transform, Vector3.zero);
        }
        Block.CombineMeshes(tempBlock, blockMaterial);

        iconCamera.Render();

        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(iconSize, iconSize, TextureFormat.RGBA32, false);
        texture.ReadPixels(new Rect(0, 0, iconSize, iconSize), 0, 0);
        texture.Apply();
        RenderTexture.active = null;

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, iconSize, iconSize), new Vector2(0.5f, 0.5f));
        iconCache[type.ToString()] = sprite;
    }

    public Sprite GetIcon(string itemName)
    {
        if (iconCache.TryGetValue(itemName, out Sprite cached))
        {
            return cached;
        }

        if (System.Enum.TryParse<BlockType>(itemName, out BlockType blockType))
        {
            InitializeIfNeeded();

            if (blockMaterial == null && World.instance != null)
            {
                blockMaterial = World.instance.cubeMate;
            }

            if (blockMaterial != null)
            {
                GenerateIcon(blockType);
                return iconCache[itemName];
            }
        }

        return null;
    }
}
