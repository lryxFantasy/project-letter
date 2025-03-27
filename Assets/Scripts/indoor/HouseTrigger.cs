using UnityEngine;
using System.Collections;

public class HouseTrigger : MonoBehaviour
{
    public CameraController cameraController; // 引用相机控制脚本
    public int houseIndex; // 这个门的房屋索引（0-5，对应六个房屋）
    public bool isExitDoor = false; // 是否是房屋内的出口门
    private bool playerInTrigger = false; // 跟踪玩家是否在触发区域内
    private PlayerController playerController; // 引用玩家控制器
    private TaskManager taskManager; // 引用 TaskManager
    private bool isShowingMessage = false; // 跟踪是否正在显示提示信息

    void Start()
    {
        // 获取 PlayerController 引用
        playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("HouseTrigger: 未找到 PlayerController！");
        }

        // 获取 TaskManager 引用
        taskManager = FindObjectOfType<TaskManager>();
        if (taskManager == null)
        {
            Debug.LogError("HouseTrigger: 未找到 TaskManager！");
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
                Debug.LogWarning("HouseTrigger: CameraController 未绑定！");
                return;
            }

            if (!cameraController.IsIndoors() && !isExitDoor)
            {
                // 如果是社区中心房子（houseIndex == 0），检查钥匙
                if (houseIndex == 0)
                {
                    if (HasKey())
                    {
                        cameraController.EnterHouse(houseIndex);
                        Debug.Log("玩家持有钥匙，进入社区中心");
                    }
                    else
                    {
                        ShowNoKeyMessage();
                        Debug.Log("玩家没有钥匙，无法进入社区中心");
                    }
                }
                else
                {
                    // 其他房屋直接进入
                    cameraController.EnterHouse(houseIndex);
                }
            }
            else if (cameraController.IsIndoors() && isExitDoor)
            {
                cameraController.ExitHouse();
            }
        }
    }

    // 检查玩家是否持有钥匙
    private bool HasKey()
    {
        if (playerController == null || taskManager == null || taskManager.inventoryManager == null)
        {
            Debug.LogWarning("HouseTrigger: PlayerController 或 InventoryManager 未正确初始化");
            return false;
        }

        // 检查背包中是否有钥匙
        foreach (var letter in taskManager.inventoryManager.letters)
        {
            if (letter.title == "废弃房屋的钥匙") // 替换为实际的钥匙标题
            {
                return true;
            }
        }
        return false;
    }

    // 显示“需要钥匙”的提示信息
    private void ShowNoKeyMessage()
    {
        if (playerController == null) return;

        // 使用 PlayerController 的交互提示文本显示信息
        if (!isShowingMessage)
        {
            isShowingMessage = true;
            playerController.GetInteractionText().gameObject.SetActive(true);
            playerController.GetInteractionText().text = "门上锁了";
            playerController.GetBottomSprite().gameObject.SetActive(true);
            playerController.GetInteractionText().CrossFadeAlpha(1f, 0.1f, false);
            playerController.GetBottomSprite().CrossFadeAlpha(1f, 0.1f, false);

            // 3秒后隐藏提示
            StartCoroutine(HideMessageAfterDelay(3f));
        }
    }

    // 隐藏提示信息
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