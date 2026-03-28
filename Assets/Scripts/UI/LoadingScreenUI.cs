using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenUI : MonoBehaviour
{
    public World world;
    public WorldLoadingState loadingState;

    [Header("UI")]
    public GameObject root;
    public Slider progressSlider;

    [Header("Smooth")]
    public float smoothSpeed = 8f;
    public float hideDelay = 0.25f;

    private float visualProgress = 0f;
    private float targetProgress = 0f;
    private float finishTimestamp = -1f;

    void Awake()
    {
        if (world == null)
        {
            world = World.instance;
            if (world == null)
            {
                world = FindObjectOfType<World>();
            }
        }

        if (loadingState == null && world != null)
        {
            loadingState = world.loadingState;
            if (loadingState == null)
            {
                loadingState = world.GetComponent<WorldLoadingState>();
            }
        }

        if (root == null)
        {
            root = gameObject;
        }
    }

    void OnEnable()
    {
        Bind();
    }

    void Update()
    {
        if (loadingState == null)
        {
            return;
        }

        targetProgress = loadingState.Progress01;
        visualProgress = Mathf.MoveTowards(visualProgress, targetProgress, Time.unscaledDeltaTime * smoothSpeed);
        RefreshProgressVisuals(visualProgress);

        if (loadingState.IsLoaded)
        {
            if (finishTimestamp < 0f)
            {
                finishTimestamp = Time.unscaledTime;
            }

            if (Time.unscaledTime - finishTimestamp >= hideDelay)
            {
                root.SetActive(false);
            }
        }
        else
        {
            finishTimestamp = -1f;
            if (!root.activeSelf)
            {
                root.SetActive(true);
            }
        }
    }

    private void Bind()
    {
        if (loadingState == null && world != null)
        {
            loadingState = world.loadingState;
            if (loadingState == null)
            {
                loadingState = world.GetComponent<WorldLoadingState>();
            }
        }

        if (loadingState == null)
        {
            return;
        }

        if (progressSlider != null)
        {
            if (progressSlider.maxValue <= progressSlider.minValue)
            {
                progressSlider.minValue = 0f;
                progressSlider.maxValue = 1f;
            }
            progressSlider.wholeNumbers = false;
        }

        targetProgress = loadingState.Progress01;
        visualProgress = targetProgress;
        RefreshProgressVisuals(visualProgress);

        root.SetActive(!loadingState.IsLoaded);
    }

    private void RefreshProgressVisuals(float progress)
    {
        if (progressSlider != null)
        {
            progressSlider.SetValueWithoutNotify(Mathf.Clamp01(progress));
        }
    }
}
