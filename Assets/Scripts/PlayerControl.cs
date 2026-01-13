using UnityEngine;

public class PlayerControl : Entity
{
    public CameraSettings cameraSettings;

    private bool onGround = false;
    private float xRotation = 0f;
    private float yRotation = 0f;

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
        Debug.Log(rigidbody.velocity);
        rigidbody.velocity = transform.TransformDirection(rigidbody.velocity);
    }

    private void CheckJump()
    {
        if (!onGround) return;
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        onGround = true;
    }

    private void OnTriggerExit(Collider other)
    {
        onGround = false;
    }

    [System.Serializable]
    public class CameraSettings
    {
        public Camera camera;
        public float sensitivityX;
        public float sensitivityY;
    }
}
