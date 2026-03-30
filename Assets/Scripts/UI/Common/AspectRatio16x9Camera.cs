using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class AspectRatio16x9Camera : MonoBehaviour
{
    public int targetWidth = 16;
    public int targetHeight = 9;

    private Camera targetCamera;
    private int lastScreenWidth = -1;
    private int lastScreenHeight = -1;

    void Awake()
    {
        targetCamera = GetComponent<Camera>();
        ApplyViewport();
    }

    void OnEnable()
    {
        if (targetCamera == null)
        {
            targetCamera = GetComponent<Camera>();
        }

        ApplyViewport();
    }

    void LateUpdate()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            ApplyViewport();
        }
    }

    void OnValidate()
    {
        if (targetWidth <= 0) targetWidth = 16;
        if (targetHeight <= 0) targetHeight = 9;

        if (targetCamera == null)
        {
            targetCamera = GetComponent<Camera>();
        }

        ApplyViewport();
    }

    private void ApplyViewport()
    {
        if (targetCamera == null) return;
        if (targetWidth <= 0 || targetHeight <= 0) return;
        if (Screen.width <= 0 || Screen.height <= 0) return;

        float targetAspect = (float)targetWidth / targetHeight;
        float screenAspect = (float)Screen.width / Screen.height;

        Rect rect = new Rect(0f, 0f, 1f, 1f);

        if (screenAspect > targetAspect)
        {
            float width = targetAspect / screenAspect;
            float x = (1f - width) * 0.5f;
            rect = new Rect(x, 0f, width, 1f);
        }
        else if (screenAspect < targetAspect)
        {
            float height = screenAspect / targetAspect;
            float y = (1f - height) * 0.5f;
            rect = new Rect(0f, y, 1f, height);
        }

        targetCamera.rect = rect;
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
    }
}
