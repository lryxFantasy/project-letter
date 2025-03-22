using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject pauseMask; // ��������
    public Button pauseButton;
    public Button saveButton;
    public Button quitButton;
    public Button continueButton;
    public PlayerInput playerInput;

    private float previousTimeScale;
    private bool isPaused = false;
    public static bool IsPaused { get; private set; } // ȫ����ͣ״̬

    void Start()
    {
        pausePanel.SetActive(false);
        pauseMask.SetActive(false);

        pauseButton.onClick.AddListener(TogglePause);
        saveButton.onClick.AddListener(SaveGame);
        quitButton.onClick.AddListener(QuitGame);
        continueButton.onClick.AddListener(ContinueGame);

        previousTimeScale = Time.timeScale;
        IsPaused = false; // ��ʼ��
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        if (playerInput != null)
        {
            if (playerInput.inventoryPanel.activeSelf ||
                playerInput.inventoryManager.letterDetailPanel.activeSelf)
            {
                return;
            }
        }

        isPaused = !isPaused;
        IsPaused = isPaused; // ����ȫ��״̬

        if (isPaused)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
            pauseMask.SetActive(true);
        }
        else
        {
            Time.timeScale = previousTimeScale;
            pausePanel.SetActive(false);
            pauseMask.SetActive(false);
        }
    }

    void SaveGame()
    {
        Debug.Log("��Ϸ������...");
    }

    void QuitGame()
    {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void ContinueGame()
    {
        Time.timeScale = previousTimeScale;
        pausePanel.SetActive(false);
        pauseMask.SetActive(false);
        isPaused = false;
        IsPaused = false; // ����ȫ��״̬
    }
}