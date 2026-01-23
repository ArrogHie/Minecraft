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

public class Block : MonoBehaviour
{
    public float durabilitySecond = 2.0f;
    public ParticleSystem breakParticlePrefeb;
    ParticleSystem breakParticleInstance;
    float lastBreakProgress = 0f;

    public Vector3 position;
    public BlockType blockType;
    public Chunk owner;

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
        if (breakParticleInstance)
        {
            if (lastBreakProgress < Time.time - .1f)
            {
                var emission = breakParticleInstance.emission;
                emission.enabled = false;
            }
        }
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

    }

    public bool TryBreak(float breakSecond)
    {
        lastBreakProgress = Time.time;
        if (breakParticlePrefeb && !breakParticleInstance)
        {
            Vector3 position = transform.position;
            position += new Vector3(0.5f, 0.5f, 0.5f);
            breakParticleInstance = Instantiate(breakParticlePrefeb, position, Quaternion.identity);
            breakParticleInstance.transform.parent = transform;
        }
        var emission = breakParticleInstance.emission;
        emission.enabled = true;
        if (breakSecond > durabilitySecond)
        {
            Break();
            return true;
        }
        return false;
    }

    private void Break()
    {
        if (breakParticleInstance) Destroy(breakParticleInstance);
        Destroy(gameObject);
    }
}
