using UnityEngine;

public class PlayerControl : Entity
{
    public CameraSettings cameraSettings;
    public Block activeBlock;

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
        CheckRotation();
        CheckMove();
        CheckJump();
        CheckTargetBlock();
        if (Input.GetButtonDown("Fire2")) TryPressBlock();
        if (Input.GetButton("Fire1")) TryBreakBlock();
        else { breakSeconds = 0f; }
    }

    private void CheckRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * cameraSettings.sensitivityX;
        float mouseY = Input.GetAxis("Mouse Y") * cameraSettings.sensitivityY;
        xRotation += mouseX;
        yRotation -= mouseY;
        yRotation = Mathf.Clamp(yRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(0f, xRotation, 0f);
        cameraSettings.camera.transform.localRotation = Quaternion.Euler(yRotation, 0f, 0f);

    }

    private void CheckMove()
    {
        rigidbody.velocity = new Vector3(
            Input.GetAxis("Horizontal") * speed * Time.deltaTime,
            rigidbody.velocity.y,
            Input.GetAxis("Vertical") * speed * Time.deltaTime
            );
        //Debug.Log(rigidbody.velocity);
        rigidbody.velocity = transform.TransformDirection(rigidbody.velocity);
    }

    private void CheckJump()
    {
        if (onGround == 0) return;
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
            Debug.Log("press jump");
        }
    }

    private void CheckTargetBlock()
    {
        RaycastHit hit;
        Ray ray = cameraSettings.camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;
            targetRaycastHit = hit;
            if (!objectHit.GetComponent<Block>()) { return; }
            if (Vector3.Distance(transform.position, hit.point) > 5f) { return; }
            targetBlock = objectHit.GetComponent<Block>();
        }
    }

    private void TryBreakBlock()
    {
        if (!targetBlock) { breakSeconds = 0f; return; }
        if (breakingBlock != targetBlock) { breakSeconds = 0f; }
        breakingBlock = targetBlock;
        breakSeconds += Time.deltaTime;
        if (breakingBlock.TryBreak(breakSeconds)) { breakSeconds = 0f; }
    }

    private void TryPressBlock()
    {
        if (!targetBlock) { return; }
        GameObject block = Instantiate(activeBlock.gameObject);
        block.transform.position = targetBlock.transform.position;
        Vector3 normal = targetRaycastHit.normal;
        block.transform.position += normal;
        block.transform.parent = targetBlock.transform.parent;
    }

    private void OnTriggerEnter(Collider other)
    {
        onGround++;
        Debug.Log("On Ground");
    }

    private void OnTriggerExit(Collider other)
    {
        onGround--;
        Debug.Log("Jumped");
    }

    [System.Serializable]
    public class CameraSettings
    {
        public Camera camera;
        public float sensitivityX;
        public float sensitivityY;
    }
}
