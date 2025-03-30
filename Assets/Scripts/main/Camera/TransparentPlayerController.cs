using UnityEngine;

public class TransparentPlayerController : MonoBehaviour
{
    public GameObject mainPlayer; // ���Ƕ���ͬ��λ����
    private PlayerController mainPlayerController; // ���ǵ� PlayerController�����ڼ��Ի�״̬

    void Start()
    {
        // ��ȡ���ǵ� PlayerController ���
        if (mainPlayer != null)
        {
            mainPlayerController = mainPlayer.GetComponent<PlayerController>();
            if (mainPlayerController == null)
            {
                Debug.LogError("����ɫ��δ�ҵ� PlayerController �����");
            }
        }
        else
        {
            Debug.LogError("���� Inspector ��ָ�� Main Player��");
        }
    }

    void Update()
    {
        // ͬ��λ�õ�����
        if (mainPlayer != null)
        {
            transform.position = mainPlayer.transform.position;
        }

        // ��������ڶԻ��У���ͣ�����߼������������չ��������ͣ������
        if (mainPlayerController != null && mainPlayerController.IsInDialogue())
        {
            return; // ��ͣ�����߼�
        }
    }

    // �ṩ���ⲿ���Ի�״̬�Ľӿڣ�������һ�£�
    public bool IsInDialogue()
    {
        return mainPlayerController != null && mainPlayerController.IsInDialogue();
    }
}