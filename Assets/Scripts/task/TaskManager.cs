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

        // 从 PlayerController 获取当前对话的 NPC 名称
        if (playerController.IsInDialogue())
        {
            currentNPCName = playerController.GetCurrentNPCRole();
            if (!string.IsNullOrEmpty(currentNPCName))
            {
                Debug.Log($"TaskManager: 当前对话的NPC：{currentNPCName}");
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
        Debug.Log("TaskManager: 送信按钮被点击"); // 调试：确认按钮点击是否触发

        if (currentTask == null)
        {
            Debug.Log("TaskManager: 当前任务未初始化");
            return;
        }

        if (string.IsNullOrEmpty(currentNPCName))
        {
            Debug.Log("TaskManager: 当前没有与NPC对话");
            return;
        }

        if (currentTask is Task1 task1)
        {
            Debug.Log($"TaskManager: 准备送信给 {currentNPCName}");
            task1.DeliverLetter(currentNPCName);
        }
        else
        {
            Debug.Log("TaskManager: 当前任务不是 Task1 类型");
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