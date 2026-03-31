using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    [SerializeField]
    private float rotateSpeed = 15f;

    private void Update()
    {
        transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f, Space.World);
    }
}
