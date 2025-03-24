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

    private bool isPanelOpen = false;
    private float previousTimeScale;
    private TaskBase currentTask;
    private string currentNPCName;

    void Start()
    {
        taskPanel.SetActive(false);
        taskMask.SetActive(false);
        dialoguePanel.SetActive(false);

        taskButton.onClick.AddListener(ToggleTaskPanel);
        closeButton.onClick.AddListener(ToggleTaskPanel);
        deliverButton.onClick.AddListener(TriggerDeliverLetter);

        currentTask = gameObject.AddComponent<Task1>();
        (currentTask as Task1)?.SetupDialogueUI(dialoguePanel, dialogueText, nextButton);
        (currentTask as Task1)?.SetupDeliverButton(normalDialoguePanel, deliverButton);
        UpdateTaskDisplay();

        previousTimeScale = Time.timeScale;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleTaskPanel();
        }

        // �� PlayerController ��ȡ��ǰ�Ի��� NPC ����
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
        Debug.Log("TaskManager: ���Ű�ť�����"); // ���ԣ�ȷ�ϰ�ť����Ƿ񴥷�

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

        if (currentTask is Task1 task1)
        {
            Debug.Log($"TaskManager: ׼�����Ÿ� {currentNPCName}");
            task1.DeliverLetter(currentNPCName);
        }
        else
        {
            Debug.Log("TaskManager: ��ǰ������ Task1 ����");
        }
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
    }
}