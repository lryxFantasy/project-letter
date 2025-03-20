using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public GameObject inventoryPanel;      // �������
    public InventoryManager inventoryManager; // ���� InventoryManager �Է����ż��������

    private float previousTimeScale; // ������ͣǰ��ʱ������ֵ

    void Start()
    {
        inventoryPanel.SetActive(false); // Ĭ�����ر���
        previousTimeScale = Time.timeScale; // ��ʼ��Ϊ��ǰʱ������ֵ��ͨ��Ϊ 1��
    }

    void Update()
    {
        // �� "I" ���л�������ʾ
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }

        // �� "Esc" ���رմ򿪵Ľ���
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inventoryManager.letterDetailPanel.activeSelf) // ����ż������
            {
                inventoryManager.letterDetailPanel.SetActive(false); // �ر��ż�����
            }
            else if (inventoryPanel.activeSelf) // ���������
            {
                CloseInventory(); // �رձ���
            }
        }
    }

    void ToggleInventory()
    {
        bool isActive = inventoryPanel.activeSelf;
        if (!isActive) // �򿪱���
        {
            inventoryPanel.SetActive(true);
            previousTimeScale = Time.timeScale; // ���浱ǰʱ������ֵ
            Time.timeScale = 0f; // ��ͣ��Ϸ
        }
        else // �رձ���
        {
            CloseInventory();
        }
    }

    void CloseInventory()
    {
        inventoryPanel.SetActive(false);
        // ����رձ���ʱ�ż����黹���ţ�Ҳ�ر���
        if (inventoryManager.letterDetailPanel.activeSelf)
        {
            inventoryManager.letterDetailPanel.SetActive(false);
        }
        Time.timeScale = previousTimeScale; // �ָ�ʱ������
    }
}