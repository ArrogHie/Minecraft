using UnityEngine;

public class MenuTitleFloat : MonoBehaviour
{
    public float floatSpeed = 1.8f;
    public float minScaleMultiplier = 0.96f;
    public float maxScaleMultiplier = 1.04f;

    private Vector3 baseScale;

    void Awake()
    {
        baseScale = transform.localScale;
    }

    void Update()
    {
        float wave = (Mathf.Sin(Time.unscaledTime * floatSpeed) + 1f) * 0.5f;
        float scaleMultiplier = Mathf.Lerp(minScaleMultiplier, maxScaleMultiplier, wave);
        transform.localScale = baseScale * scaleMultiplier;
    }
}
