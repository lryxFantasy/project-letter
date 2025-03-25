using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskManager : MonoBehaviour
{
    public GameObject taskPanel;
    public GameObject taskMask;
    public Button taskButton;
    public Button closeButton;
    public TMP_Text taskTitle;
    public TMP_Text taskObjective;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public Button nextButton;
    public GameObject normalDialoguePanel;
    public Button deliverButton;
    public PlayerController playerController;
    public InventoryManager inventoryManager; // ��������������������

    private bool isPanelOpen = false;
    private float previousTimeScale;
    public TaskBase currentTask;
    private string currentNPCName;

    void Start()
    {
        taskPanel.SetActive(false);
        taskMask.SetActive(false);
        dialoguePanel.SetActive(false);

        taskButton.onClick.AddListener(ToggleTaskPanel);
        closeButton.onClick.AddListener(ToggleTaskPanel);
        deliverButton.onClick.AddListener(TriggerDeliverLetter);

        currentTask = gameObject.AddComponent<Task0>();
        (currentTask as Task0)?.SetupDialogueUI(dialoguePanel, dialogueText, nextButton);
        (currentTask as Task0)?.StartTaskDialogue();
        UpdateTaskDisplay();

        previousTimeScale = Time.timeScale;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleTaskPanel();
        }

        if (playerController.IsInDialogue())
        {
            currentNPCName = playerController.GetCurrentNPCRole();
            if (!string.IsNullOrEmpty(currentNPCName))
            {
                Debug.Log($"TaskManager: ��ǰ�Ի���NPC��{currentNPCName}");
            }
        }
        else
        {
            currentNPCName = null;
        }
    }

    void ToggleTaskPanel()
    {
        isPanelOpen = !isPanelOpen;
        if (isPanelOpen)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            taskPanel.SetActive(true);
            taskMask.SetActive(true);
            dialoguePanel.SetActive(false);
        }
        else
        {
            Time.timeScale = previousTimeScale;
            taskPanel.SetActive(false);
            taskMask.SetActive(false);
        }
    }

    public void TriggerDeliverLetter()
    {
        Debug.Log("TaskManager: ���Ű�ť�����");

        if (currentTask == null)
        {
            Debug.Log("TaskManager: ��ǰ����δ��ʼ��");
            return;
        }

        if (string.IsNullOrEmpty(currentNPCName))
        {
            Debug.Log("TaskManager: ��ǰû����NPC�Ի�");
            return;
        }

        if (normalDialoguePanel != null)
        {
            normalDialoguePanel.SetActive(false);
        }

        Debug.Log($"TaskManager: ׼�����Ÿ� {currentNPCName}");
        currentTask.DeliverLetter(currentNPCName);
    }

    public void UpdateTaskDisplay()
    {
        if (currentTask != null)
        {
            taskTitle.text = currentTask.GetTaskName();
            taskObjective.text = currentTask.GetTaskObjective();
        }
    }

    public void SetTask(TaskBase newTask)
    {
        if (currentTask != null) Destroy(currentTask);
        currentTask = newTask;
        UpdateTaskDisplay();

        // Ϊ����������ż������� Task0��
        if (inventoryManager != null && !(newTask is Task0))
        {
            if (newTask is Task1)
            {
                inventoryManager.AddLetter(new Letter { title = "���ά���е���", content = "ά���У����޺���һ̨FANLU-317�������ͺţ��ӷ�������������ƴ�����ģ������е㲻�ȶ����������������á��Ҵ����������Ż�����ţ�����̫ǿ����û�������ţ��������ֲ�����������ĿǰΨһ�İ취����֮ǰ������������˹д�ţ��еĻ��ͽ������ɣ���ʱ�������������͹�ȥ����֪����������Щ�Ƽ���������������ǻ���һ�У����ⶫ�����ٲ��ᱻ�����ջ����ܰ��ϵ�æ���һ����о���Я�͹���֧�ܣ��������������˻�����͹�ȥ���б����Ҫ��д�����������ͻ���������ҿ�����������" });
            }
 
        }
    }
}