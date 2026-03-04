using UnityEngine;

public class PlayerControl : Entity
{
    public CameraSettings cameraSettings;
    public BlockType activeBlock;
    public Inventory inventory;

    private int onGround = 0;
    private float xRotation = 0f;
    private float yRotation = 0f;

    private float breakSeconds = 0f;
    private Block targetBlock;
    private Block breakingBlock;
    private RaycastHit targetRaycastHit;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckMove();
        if (!inventory.isOpen)
        {
            CheckRotation();
            CheckJump();
            //Debug.Log(onGround);
            CheckTargetBlock();
            //Debug.Log(targetBlock);
            if (Input.GetButtonDown("Fire2")) TryPressBlock();
            if (Input.GetButton("Fire1")) TryBreakBlock();
            else
            {
                breakSeconds = 0f;
                if (breakingBlock != null) breakingBlock.TryBreak(breakSeconds);
            }
        }
        //if (targetBlock != null)
        //{
        //    Debug.Log("Target Block: " + targetBlock.blockType + " at " + (targetBlock.position + targetBlock.owner.transform.position));
        //}
    }

    private void CheckRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * cameraSettings.sensitivityX;
        float mouseY = Input.GetAxis("Mouse Y") * cameraSettings.sensitivityY;
        xRotation += mouseX;
        yRotation -= mouseY;
        yRotation = Mathf.Clamp(yRotation, -89f, 89f);
        //transform.localRotation = Quaternion.Euler(0f, xRotation, 0f);
        cameraSettings.camera.transform.localRotation = Quaternion.Euler(yRotation, xRotation, 0f);
    }

    private void CheckMove()
    {
        if (inventory.isOpen) { rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0); }
        else
        {
            Vector3 camForward = cameraSettings.camera.transform.forward;
            Vector3 camRight = cameraSettings.camera.transform.right;
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();
            Vector3 moveDirection = camForward * Input.GetAxis("Vertical") + camRight * Input.GetAxis("Horizontal");
            rigidbody.velocity = new Vector3(
                moveDirection.x * speed,
                rigidbody.velocity.y,
                moveDirection.z * speed
                );
        }

        //Debug.Log(rigidbody.velocity);
        rigidbody.velocity = transform.TransformDirection(rigidbody.velocity);
    }

    private void CheckJump()
    {
        if (onGround == 0) return;
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
            //Debug.Log("press jump");
        }
    }

    private void CheckTargetBlock()
    {
        RaycastHit hit;
        Ray ray = cameraSettings.camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 5f, LayerMask.GetMask("Chunk")))
        {
            Transform objectHit = hit.transform;
            targetRaycastHit = hit;

            //float size = 0.5f;
            //Debug.DrawLine(hit.point - Vector3.up * size, hit.point + Vector3.up * size, Color.red);
            //Debug.DrawLine(hit.point - Vector3.left * size, hit.point + Vector3.left * size, Color.red);
            //Debug.DrawLine(hit.point - Vector3.forward * size, hit.point + Vector3.forward * size, Color.red);
            //Debug.Log(hit.point);

            Chunk targetChunk = objectHit.GetComponent<Chunk>();
            if (targetChunk == null)
            {
                targetBlock = null;
                return;
            }

            //Debug.Log("Hit Chunk at " + targetChunk.gameObject.name);
            Vector3 pos = hit.point - hit.normal * 0.01f;
            targetBlock = targetChunk.GetBlock(pos - targetChunk.transform.position);
        }
        else targetBlock = null;
    }

    private void TryBreakBlock()
    {
        if (targetBlock == null)
        {
            breakSeconds = 0f;
            if (breakingBlock != null) breakingBlock.TryBreak(breakSeconds);
            return;
        }
        if (breakingBlock != targetBlock)
        {
            breakSeconds = 0f;
            if (breakingBlock != null) breakingBlock.TryBreak(breakSeconds);
        }
        breakingBlock = targetBlock;
        breakSeconds += Time.deltaTime;
        if (breakingBlock.TryBreak(breakSeconds)) { breakSeconds = 0f; }
    }

    private void TryPressBlock()
    {
        if (targetBlock == null) { return; }
        Chunk targetChunk = targetBlock.owner;
        if (targetChunk == null) { return; }

        Vector3 normal = targetRaycastHit.normal;
        if (!targetChunk.SetBlockType(targetBlock.position + normal, activeBlock)) // 跨越区块放置处理
        {
            Vector2Int pos = targetChunk.chunkPos;
            Vector2Int normal2 = new Vector2Int((int)normal.x, (int)normal.z);
            pos += normal2;
            Chunk trueChunk = World.instance.getChunk(pos);
            Vector3 pos3 = targetBlock.position + normal + targetChunk.transform.position;
            pos3 -= trueChunk.transform.position;
            if (trueChunk != null) trueChunk.SetBlockType(pos3, activeBlock);
        }
    }

    public void PickUp(BlockType type)
    {
        //Debug.Log("Pickup" + type);
        inventory.Pickup(type);
    }

    private void OnTriggerEnter(Collider other)
    {
        onGround++;
        //Debug.Log("On Ground");
    }

    private void OnTriggerExit(Collider other)
    {
        onGround--;
        //Debug.Log("Jumped");
    }

    [System.Serializable]
    public class CameraSettings
    {
        public Camera camera;
        public float sensitivityX;
        public float sensitivityY;
    }
}
