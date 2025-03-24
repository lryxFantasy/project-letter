using UnityEngine;

public class HouseTrigger : MonoBehaviour
{
    public CameraController cameraController; // ����������ƽű�
    public int houseIndex; // ����ŵķ���������0-5����Ӧ�������ݣ�
    public bool isExitDoor = false; // �Ƿ��Ƿ����ڵĳ�����
    private bool playerInTrigger = false; // ��������Ƿ��ڴ���������

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
        }
    }

    void Update()
    {
        if (playerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            if (cameraController == null)
            {
                return;
            }

            if (!cameraController.IsIndoors() && !isExitDoor)
            {
                cameraController.EnterHouse(houseIndex);
            }
            else if (cameraController.IsIndoors() && isExitDoor)
            {
                cameraController.ExitHouse();
            }
        }
    }
}