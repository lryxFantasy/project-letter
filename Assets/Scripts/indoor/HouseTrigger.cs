using UnityEngine;
using System.Collections;

public class HouseTrigger : MonoBehaviour
{
    public CameraController cameraController; // ����������ƽű�
    public int houseIndex; // ����ŵķ���������0-5����Ӧ�������ݣ�
    public bool isExitDoor = false; // �Ƿ��Ƿ����ڵĳ�����
    private bool playerInTrigger = false; // ��������Ƿ��ڴ���������
    private PlayerController playerController; // ������ҿ�����
    private TaskManager taskManager; // ���� TaskManager
    private bool isShowingMessage = false; // �����Ƿ�������ʾ��ʾ��Ϣ

    void Start()
    {
        // ��ȡ PlayerController ����
        playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("HouseTrigger: δ�ҵ� PlayerController��");
        }

        // ��ȡ TaskManager ����
        taskManager = FindObjectOfType<TaskManager>();
        if (taskManager == null)
        {
            Debug.LogError("HouseTrigger: δ�ҵ� TaskManager��");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            isShowingMessage = false;
        }
    }

    void Update()
    {
        if (playerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            if (cameraController == null)
            {
                Debug.LogWarning("HouseTrigger: CameraController δ�󶨣�");
                return;
            }

            if (!cameraController.IsIndoors() && !isExitDoor)
            {
                // ������������ķ��ӣ�houseIndex == 0�������Կ��
                if (houseIndex == 0)
                {
                    if (HasKey())
                    {
                        cameraController.EnterHouse(houseIndex);
                        Debug.Log("��ҳ���Կ�ף�������������");
                    }
                    else
                    {
                        ShowNoKeyMessage();
                        Debug.Log("���û��Կ�ף��޷�������������");
                    }
                }
                else
                {
                    // ��������ֱ�ӽ���
                    cameraController.EnterHouse(houseIndex);
                }
            }
            else if (cameraController.IsIndoors() && isExitDoor)
            {
                cameraController.ExitHouse();
            }
        }
    }

    // �������Ƿ����Կ��
    private bool HasKey()
    {
        if (playerController == null || taskManager == null || taskManager.inventoryManager == null)
        {
            Debug.LogWarning("HouseTrigger: PlayerController �� InventoryManager δ��ȷ��ʼ��");
            return false;
        }

        // ��鱳�����Ƿ���Կ��
        foreach (var letter in taskManager.inventoryManager.letters)
        {
            if (letter.title == "�������ݵ�Կ��") // �滻Ϊʵ�ʵ�Կ�ױ���
            {
                return true;
            }
        }
        return false;
    }

    // ��ʾ����ҪԿ�ס�����ʾ��Ϣ
    private void ShowNoKeyMessage()
    {
        if (playerController == null) return;

        // ʹ�� PlayerController �Ľ�����ʾ�ı���ʾ��Ϣ
        if (!isShowingMessage)
        {
            isShowingMessage = true;
            playerController.GetInteractionText().gameObject.SetActive(true);
            playerController.GetInteractionText().text = "��������";
            playerController.GetBottomSprite().gameObject.SetActive(true);
            playerController.GetInteractionText().CrossFadeAlpha(1f, 0.1f, false);
            playerController.GetBottomSprite().CrossFadeAlpha(1f, 0.1f, false);

            // 3���������ʾ
            StartCoroutine(HideMessageAfterDelay(3f));
        }
    }

    // ������ʾ��Ϣ
    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (playerController != null)
        {
            playerController.GetInteractionText().CrossFadeAlpha(0f, 0.1f, false);
            playerController.GetBottomSprite().CrossFadeAlpha(0f, 0.1f, false);
            yield return new WaitForSeconds(0.1f);
            playerController.GetInteractionText().gameObject.SetActive(false);
            playerController.GetBottomSprite().gameObject.SetActive(false);
        }
        isShowingMessage = false;
    }
}