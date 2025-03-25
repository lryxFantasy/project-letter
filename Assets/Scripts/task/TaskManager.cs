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
    public InventoryManager inventoryManager; // 新增：背包管理器引用

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
        Debug.Log("TaskManager: 送信按钮被点击");

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

        if (normalDialoguePanel != null)
        {
            normalDialoguePanel.SetActive(false);
        }

        Debug.Log($"TaskManager: 准备送信给 {currentNPCName}");
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

        // 为新任务添加信件（除了 Task0）
        if (inventoryManager != null && !(newTask is Task0))
        {
            if (newTask is Task1)
            {
                inventoryManager.AddLetter(new Letter { title = "简给维克托的信", content = "维克托，我修好了一台FANLU-317，家用型号，从废墟里捡来的零件拼出来的，可能有点不稳定，但测试下来能用。我打算让它在信火村送信，辐射太强，人没法随便出门，防护服又不够，机器是目前唯一的办法。你之前提过想给伊莱亚斯写信，有的话就交给它吧，到时候我让它帮你送过去。我知道你讨厌这些科技玩意儿，觉得它们毁了一切，可这东西至少不会被辐射烧坏，能帮上点忙。我还在研究便携型骨骼支架，进度慢，但成了会给你送过去。有别的需要就写下来，它会送回来，别跟我客气。——简" });
            }
 
        }
    }
}