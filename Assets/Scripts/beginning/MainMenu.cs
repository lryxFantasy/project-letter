using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button newGameButton; // ��ʼ����Ϸ��ť
    public Button exitGameButton; // �˳���Ϸ��ť

    void Start()
    {
        newGameButton.onClick.AddListener(StartNewGame);
        exitGameButton.onClick.AddListener(ExitGame);
    }

    void StartNewGame()
    {
        Debug.Log("��ʼ����Ϸ");
        SceneManager.LoadScene("main");
    }


    void ExitGame()
    {
        Debug.Log("�˳���Ϸ");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}