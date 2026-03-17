using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;

    [Header("Time Settings")]
    public float timeScale = 1f;
    public float dayDuration = 600f;

    [Header("Current Time")]
    [Range(0, 1)] public float currentTime = 0f;

    [Header("Lighting")]
    public Light directionalLight;
    public float dayIntensity = 1f;
    public float dayAmbientIntensity = 1f;
    public float nightIntensity = 0.15f;
    public float nightAmbientIntensity = 0.2f;

    [Header("Skybox")]
    public Material skyboxDay;
    public Material skyboxNight;

    private bool isPaused = false;
    private float acceleratedTime = 1f;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        HandleInput();

        if (!isPaused)
        {
            float timeIncrement = (Time.deltaTime * timeScale * acceleratedTime) / dayDuration;
            currentTime = (currentTime + timeIncrement) % 1f;
        }

        UpdateLighting();
        UpdateSkybox();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            isPaused = !isPaused;
        }

        if (Input.GetKey(KeyCode.G))
        {
            acceleratedTime = 100f;
        }
        else
        {
            acceleratedTime = 1f;
        }
    }

    void UpdateLighting()
    {
        if (directionalLight == null) return;

        float sunAngle = (currentTime-0.25f) * 360f;
        directionalLight.transform.rotation = Quaternion.Euler(sunAngle, 0, 0);

        float intensityFactor = GetIntensityFactor(currentTime);
        directionalLight.intensity = Mathf.Lerp(nightIntensity, dayIntensity, intensityFactor);

        RenderSettings.ambientIntensity = Mathf.Lerp(nightAmbientIntensity, dayAmbientIntensity, intensityFactor);
    }

    float GetIntensityFactor(float time)
    {
        if (time < 0.2f) return 0f;
        if (time < 0.3f)
        {
            float t = (time - 0.2f) / 0.1f;
            return Mathf.Pow(Mathf.Sin(t * Mathf.PI / 2), 2);
        }
        if (time < 0.7f) return 1f;
        if (time < 0.8f)
        {
            float t = (time - 0.7f) / 0.1f;
            return 1f - Mathf.Pow(Mathf.Sin(t * Mathf.PI / 2), 2);
        }
        return 0f;
    }

    void UpdateSkybox()
    {
        if (skyboxDay == null || skyboxNight == null) return;

        if (currentTime > 0.25f && currentTime < 0.75f)
        {
            RenderSettings.skybox = skyboxDay;
        }
        else
        {
            RenderSettings.skybox = skyboxNight;
        }
    }

    public float GetCurrentTimeNormalized() => currentTime;

    public float GetGameTimeHours() => currentTime * 24f;

    public float GetGameTimeMinutes() => (currentTime * 24f % 1f) * 60f;

    public string GetGameTimeString()
    {
        float hours = GetGameTimeHours();
        float minutes = GetGameTimeMinutes();
        return $"{(int)hours}:{(int)minutes:D2}";
    }
}
