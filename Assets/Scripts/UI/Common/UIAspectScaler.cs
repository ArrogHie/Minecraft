using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(CanvasScaler))]
public class UIAspectScaler : MonoBehaviour
{
    public Vector2 referenceResolution = new Vector2(1920f, 1080f);
    [Range(0f, 1f)] public float matchWidthOrHeight = 0.5f;

    private CanvasScaler scaler;

    void Awake()
    {
        Apply();
    }

    void OnEnable()
    {
        Apply();
    }

    void OnValidate()
    {
        if (referenceResolution.x <= 0f) referenceResolution.x = 1920f;
        if (referenceResolution.y <= 0f) referenceResolution.y = 1080f;
        Apply();
    }

    private void Apply()
    {
        if (scaler == null)
        {
            scaler = GetComponent<CanvasScaler>();
        }

        if (scaler == null) return;

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = referenceResolution;
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = matchWidthOrHeight;
    }
}
