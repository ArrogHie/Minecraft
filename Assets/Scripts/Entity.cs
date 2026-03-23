using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField]
    public float speed;
    public float jumpForce;

    public new Rigidbody rigidbody;

    protected virtual void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Jump()
    {
        rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}
