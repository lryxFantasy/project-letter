using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Task7 : TaskBase
{
    private bool taskCondition = false; // 替换为具体完成条件
    private string[] currentDialogue;
    private int dialogueIndex = 0;
    private TMP_Text dialogueText;
    private GameObject dialoguePanel;
    private Button nextButton;
    private TaskManager taskManager;

    public void SetupTask(TaskManager manager, GameObject panel, TMP_Text text, Button button)
    {
        taskManager = manager;
        dialoguePanel = panel;
        dialogueText = text;
        nextButton = button;
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(NextDialogue);
        dialoguePanel.SetActive(false);
    }

    public override string GetTaskName() => "任务7名称"; // 替换为具体名称

    public override string GetTaskObjective() => "任务7目标描述"; // 替换为具体目标

    public override bool IsTaskComplete() => taskCondition;

    public override void DeliverLetter(string targetResident)
    {
        dialogueIndex = 0;
        currentDialogue = targetResident == "目标NPC" && !taskCondition // 替换为具体NPC和条件
            ? GetDialogueForTarget()
            : new string[] { "【……】你还没有信" };

        StartDialogue();
    }

    private void StartDialogue()
    {
        dialoguePanel.SetActive(true);
        dialogueText.text = currentDialogue[dialogueIndex];
    }

    private void NextDialogue()
    {
        dialogueIndex++;
        if (dialogueIndex < currentDialogue.Length)
        {
            dialogueText.text = currentDialogue[dialogueIndex];
        }
        else
        {
            dialoguePanel.SetActive(false);
            if (!taskCondition && currentDialogue.Length > 1)
            {
                taskCondition = true;
                if (taskManager != null && taskManager.inventoryManager != null)
                {
                    Debug.Log("Task7: 更新背包 - 移除前置信件，添加新信件");
                    taskManager.inventoryManager.RemoveLetter("前置信件标题"); // 替换为具体信件
                    taskManager.inventoryManager.AddLetter(new Letter
                    {
                        title = "新信件标题", // 替换为具体标题
                        content = "新信件内容" // 替换为具体内容
                    });
                }
                else
                {
                    Debug.LogError("Task7: TaskManager 或 InventoryManager 未正确绑定");
                }
            }

            PlayerController playerController = Object.FindObjectOfType<PlayerController>();
            if (playerController != null && playerController.IsInDialogue())
                playerController.EndDialogue();

            if (IsTaskComplete())
            {
                if (taskManager != null)
                {
                    Debug.Log("Task7: 任务完成");
                    // 如果有 Task8，可以添加跳转逻辑
                    // Task8 newTask = gameObject.AddComponent<Task8>();
                    // taskManager.SetTask(newTask);
                    // newTask.SetupTask(taskManager, dialoguePanel, dialogueText, nextButton);
                    taskManager.UpdateTaskDisplay();
                }
                else
                {
                    Debug.LogError("Task7: TaskManager 未找到");
                }
            }
        }
    }

    private string[] GetDialogueForTarget()
    {
        return new string[]
        {
            "【目标NPC】对话1", // 替换为具体对话
            "【目标NPC】对话2"
        };
    }
}