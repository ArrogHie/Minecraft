using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("Menu UI")]
    public CanvasGroup menuCanvasGroup;

    [Header("Scene")]
    public string mainMenuSceneName = "Menu";

    [Header("Optional References")]
    public PlayerControl playerControl;
    public Inventory inventory;

    private bool isPaused;

    void Awake()
    {
        if (menuCanvasGroup == null)
        {
            menuCanvasGroup = GetComponent<CanvasGroup>();
        }

        if (menuCanvasGroup == null)
        {
            menuCanvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        if (playerControl == null)
        {
            playerControl = FindObjectOfType<PlayerControl>();
        }

        if (inventory == null)
        {
            if (playerControl != null)
            {
                inventory = playerControl.inventory;
            }

            if (inventory == null)
            {
                inventory = FindObjectOfType<Inventory>();
            }
        }

        isPaused = false;
        SetMenuVisible(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        if (isPaused) return;

        if (inventory != null && inventory.window != null && inventory.window.gameObject.activeSelf)
        {
            inventory.window.gameObject.SetActive(false);
            inventory.isOpen = false;
        }

        isPaused = true;
        Time.timeScale = 0f;
        SetMenuVisible(true);

        if (playerControl != null)
        {
            playerControl.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = 1f;
        SetMenuVisible(false);

        if (playerControl != null)
        {
            playerControl.enabled = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnExitToMainMenuClick()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }

    private void SetMenuVisible(bool visible)
    {
        if (menuCanvasGroup == null) return;

        menuCanvasGroup.alpha = visible ? 1f : 0f;
        menuCanvasGroup.interactable = visible;
        menuCanvasGroup.blocksRaycasts = visible;
    }
}
