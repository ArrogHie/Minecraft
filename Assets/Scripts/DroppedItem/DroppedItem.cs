using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public float rotationSpeed = 90f; 
    public float floatAmplitude = 0.1f; 
    public float floatFrequency = 1f; 

    public BlockType blockType;
    public Material mate;
    public Rigidbody rb;

    public void Init(BlockType type)
    {
        blockType = type;
        float size = 0.2f;
        Vector3 offset = new Vector3(-0.5f, -0.5f, -0.5f);
        foreach (CubeSide side in System.Enum.GetValues(typeof(CubeSide)))
        {
            Block.CreateMesh(blockType, side, transform, offset, size);
        }
        Block.CombineMeshes(gameObject, mate);
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
