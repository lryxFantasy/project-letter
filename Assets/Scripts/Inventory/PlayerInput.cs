using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public GameObject inventoryPanel;      // 背包面板
    public InventoryManager inventoryManager; // 引用 InventoryManager 以访问信件详情面板

    private float previousTimeScale; // 保存暂停前的时间缩放值

    void Start()
    {
        inventoryPanel.SetActive(false); // 默认隐藏背包
        previousTimeScale = Time.timeScale; // 初始化为当前时间缩放值（通常为 1）
    }

    void Update()
    {
        // 按 "I" 键切换背包显示
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }

        // 按 "Esc" 键关闭打开的界面
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inventoryManager.letterDetailPanel.activeSelf) // 如果信件详情打开
            {
                inventoryManager.letterDetailPanel.SetActive(false); // 关闭信件详情
            }
            else if (inventoryPanel.activeSelf) // 如果背包打开
            {
                CloseInventory(); // 关闭背包
            }
        }
    }

    void ToggleInventory()
    {
        bool isActive = inventoryPanel.activeSelf;
        if (!isActive) // 打开背包
        {
            inventoryPanel.SetActive(true);
            previousTimeScale = Time.timeScale; // 保存当前时间缩放值
            Time.timeScale = 0f; // 暂停游戏
        }
        else // 关闭背包
        {
            CloseInventory();
        }
    }

    void CloseInventory()
    {
        inventoryPanel.SetActive(false);
        // 如果关闭背包时信件详情还开着，也关闭它
        if (inventoryManager.letterDetailPanel.activeSelf)
        {
            inventoryManager.letterDetailPanel.SetActive(false);
        }
        Time.timeScale = previousTimeScale; // 恢复时间缩放
    }
}