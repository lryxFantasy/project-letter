using UnityEngine;

public class HouseTrigger : MonoBehaviour
{
    public CameraController cameraController; // 引用相机控制脚本
    public int houseIndex; // 这个门的房屋索引（0-5，对应六个房屋）
    public bool isExitDoor = false; // 是否是房屋内的出口门
    private bool playerInTrigger = false; // 跟踪玩家是否在触发区域内

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