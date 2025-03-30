using UnityEngine;
using UnityEngine.UI;

public class PanelOpener : MonoBehaviour
{
    public Button openButton;  // �����İ�ť
    public Button closeButton; // �ر����İ�ť
    public GameObject panel;   // Ҫ�򿪻�رյ����

    void Start()
    {

        // ��ʼ��ʱ�ر����
        panel.SetActive(false);

        // �󶨰�ť����¼�
        openButton.onClick.AddListener(OpenPanel);
        closeButton.onClick.AddListener(ClosePanel);
    }

    // �����
    public void OpenPanel()
    {
        panel.SetActive(!panel.activeSelf); 
    }

    // �ر����
    public void ClosePanel()
    {
        panel.SetActive(false);
    }
}