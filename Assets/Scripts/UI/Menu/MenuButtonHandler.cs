using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public enum ButtonAction
    {
        StartGame,
        QuitGame
    }

    public ButtonAction actionType = ButtonAction.StartGame;
    public string targetSceneName;

    [Header("Scale Feedback")]
    public float pressedScale = 0.92f;
    public float transitionSpeed = 12f;

    private Vector3 baseScale;
    private Vector3 targetScale;

    void Awake()
    {
        baseScale = transform.localScale;
        targetScale = baseScale;
    }

    void OnEnable()
    {
        baseScale = transform.localScale;
        targetScale = baseScale;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * transitionSpeed);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        targetScale = baseScale * pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        targetScale = baseScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = baseScale;
    }

    public void ExecuteAction()
    {
        if (actionType == ButtonAction.StartGame)
        {
            if (string.IsNullOrEmpty(targetSceneName))
            {
                Debug.Log("Target scene name is empty.");
                return;
            }

            SceneManager.LoadScene(targetSceneName);
            return;
        }

        Application.Quit();

#if UNITY_EDITOR
        Debug.Log("Application.Quit called.");
#endif
    }
}
