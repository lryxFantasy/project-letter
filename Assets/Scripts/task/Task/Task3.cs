using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[System.Serializable]
public class Task3 : TaskBase
{
    private bool letterDeliveredToJane = false;
    private string[] currentDialogue;
    private int dialogueIndex = 0;
    private TMP_Text dialogueText;
    private GameObject dialoguePanel;
    private Button nextButton;
    private TaskManager taskManager;

    // 任务开始面板相关
    private GameObject taskCompletePanel;
    private TextMeshProUGUI taskCompleteText;

    public void SetupTask(TaskManager manager, GameObject panel, TMP_Text text, Button button)
    {
        taskManager = manager;
        dialoguePanel = panel;
        dialogueText = text;
        nextButton = button;
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(NextDialogue);
        dialoguePanel.SetActive(false);

        // 初始化任务开始面板并显示
        SetupTaskCompletePanel();
        StartCoroutine(ShowTaskStartPanel());
    }

    public override string GetTaskName() => "理性的恋歌";

    public override string GetTaskObjective() => $"送达【伊莱亚斯·凯恩】给【简·怀特】的信：{(letterDeliveredToJane ? "已完成" : "未完成")}";

    public override bool IsTaskComplete() => letterDeliveredToJane;

    public override void DeliverLetter(string targetResident)
    {
        dialogueIndex = 0;
        currentDialogue = targetResident == "简·怀特" && !letterDeliveredToJane
            ? GetDialogueForJane()
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
            if (!letterDeliveredToJane && currentDialogue.Length > 1)
            {
                letterDeliveredToJane = true;
                if (taskManager != null && taskManager.inventoryManager != null)
                {
                    Debug.Log("Task3: 移除伊莱亚斯的信");
                    taskManager.inventoryManager.RemoveLetter("伊莱亚斯的信");
                }
                else
                {
                    Debug.LogError("Task3: TaskManager 或 InventoryManager 未正确绑定");
                }
            }

            PlayerController playerController = Object.FindObjectOfType<PlayerController>();
            if (playerController != null && playerController.IsInDialogue())
                playerController.EndDialogue();

            if (IsTaskComplete())
            {
                if (taskManager != null)
                {
                    Debug.Log("Task3: 任务完成，切换到 Task4");
                    Task4 newTask = gameObject.AddComponent<Task4>();
                    taskManager.SetTask(newTask);
                    newTask.SetupTask(taskManager, dialoguePanel, dialogueText, nextButton);
                    taskManager.UpdateTaskDisplay();
                }
                else
                {
                    Debug.LogError("Task3: TaskManager 未找到，无法切换到 Task4");
                }
            }
        }
    }

    private string[] GetDialogueForJane()
    {
        return new string[]
        {
            "【简·怀特】回来了，给我的信？让我看看。",
            "【……】",
            "【简·怀特】伊莱亚斯的？他又写这些……乱七八糟的词，我得理清楚才能看懂。",
            "【简·怀特】他说起了初见那晚的星空，我是记得，可我当时只想测辐射值，哪有心思看他。",
            "【简·怀特】他……写得这么直接，我真……不知道怎么回应，脑子都乱了。",
            "【简·怀特】他谈恋爱时也是这样，热情得像短路的电池，我有点跟不上。",
            "【简·怀特】在遇见他之前，我的世界是由0和1构成的，一切事物都可以被公式表达，",
            "【简·怀特】可他的热情似火让我的世界出了bug，那些情感的涌入让我一时难以应对。",
            "【简·怀特】我喜欢数据，喜欢能修好的东西，他却老给我讲诗，讲心跳，讲那些抓不住的……",
            "【简·怀特】其实有时候我能懂，但我有些话我难以启齿，可能会觉得……害羞吧。",
            "【简·怀特】这傻子，想让我怎么办啊？",
            "【简·怀特】假如你再回去见他，帮我跟他说，就说……嗯，我不太会说那些华丽的词藻，但可能，我确实喜欢着他。",
            "【简·怀特】我会再写信给他的。",
            "【简·怀特】哦对了，奶奶萝丝说想让你帮她送信，她在村子东北面边缘的小房子里，谢谢你。"
        };
    }

    // 初始化任务完成面板
    private void SetupTaskCompletePanel()
    {
        taskCompletePanel = GameObject.Find("TaskCompletePanel");
        if (taskCompletePanel != null)
        {
            taskCompleteText = taskCompletePanel.GetComponentInChildren<TextMeshProUGUI>();
            if (taskCompleteText == null)
            {
                Debug.LogWarning("TaskCompletePanel 中没有找到 TextMeshProUGUI 组件！");
            }
            else
            {
                CanvasGroup canvasGroup = taskCompletePanel.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = taskCompletePanel.AddComponent<CanvasGroup>();
                }
                canvasGroup.alpha = 0f;
            }
        }
        else
        {
            Debug.LogError("未找到 TaskCompletePanel，请确保场景中已存在该面板！");
        }
    }

    // 显示任务开始提示
    private IEnumerator ShowTaskStartPanel()
    {
        if (taskCompletePanel != null && taskCompleteText != null)
        {
            taskCompleteText.text = "任务3——理性的恋歌";
            CanvasGroup canvasGroup = taskCompletePanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = taskCompletePanel.AddComponent<CanvasGroup>();
                canvasGroup.alpha = 0f;
            }

            // 淡入
            float fadeDuration = 1f;
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
                yield return null;
            }
            canvasGroup.alpha = 1f;

            // 显示 2 秒
            yield return new WaitForSecondsRealtime(2f);

            // 淡出
            elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                yield return null;
            }
            canvasGroup.alpha = 0f;

            Debug.Log("任务3 开始面板已显示并隐藏");
        }
        else
        {
            Debug.LogWarning("任务完成面板或文字组件未正确初始化，无法显示任务3开始提示！");
        }
    }
}