using UnityEngine;
using System.Collections;

public class HouseTrigger : MonoBehaviour
{
    public CameraController cameraController;
    public int houseIndex; // ��������
    public bool isExitDoor = false; // �Ƿ�Ϊ����
    private bool playerInTrigger = false;
    private PlayerController playerController;
    private TaskManager taskManager;
    private bool isShowingMessage = false;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("HouseTrigger: δ�ҵ� PlayerController��");
        }

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
        // ������ҽ���
        if (playerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            if (cameraController == null)
            {
                Debug.LogWarning("HouseTrigger: CameraController δ�󶨣�");
                return;
            }

            if (!cameraController.IsIndoors() && !isExitDoor)
            {
                if (houseIndex == 0) // ����������ҪԿ��
                {
                    if (HasKey())
                    {
                        cameraController.EnterHouse(houseIndex);
                        // ���������λ��
                        FindObjectOfType<RubyController>().UpdateLastHousePosition(cameraController.housePlayerPositions[houseIndex]);
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
                    cameraController.EnterHouse(houseIndex);
                    FindObjectOfType<RubyController>().UpdateLastHousePosition(cameraController.housePlayerPositions[houseIndex]);
                }
            }
            else if (cameraController.IsIndoors() && isExitDoor)
            {
                cameraController.ExitHouse();
            }
        }
    }

    // ����Ƿ���Կ��
    private bool HasKey()
    {
        if (playerController == null || taskManager == null || taskManager.inventoryManager == null)
        {
            Debug.LogWarning("HouseTrigger: PlayerController �� InventoryManager δ��ȷ��ʼ��");
            return false;
        }

        foreach (var letter in taskManager.inventoryManager.letters)
        {
            if (letter.title == "�������ݵ�Կ��")
            {
                return true;
            }
        }
        return false;
    }

    private void ShowNoKeyMessage()
    {
        if (playerController == null) return;

        if (!isShowingMessage)
        {
            isShowingMessage = true;
            playerController.GetInteractionText().gameObject.SetActive(true);
            playerController.GetInteractionText().text = "��������";
            playerController.GetBottomSprite().gameObject.SetActive(true);
            playerController.GetInteractionText().CrossFadeAlpha(1f, 0.1f, false);
            playerController.GetBottomSprite().CrossFadeAlpha(1f, 0.1f, false);

            StartCoroutine(HideMessageAfterDelay(3f));
        }
    }

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