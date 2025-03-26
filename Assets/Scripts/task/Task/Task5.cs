using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[System.Serializable]
public class Task5 : TaskBase
{
    private bool letterDeliveredToEve = false;
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

    public override string GetTaskName() => "画中的光";

    public override string GetTaskObjective() => $"送达【小卢克·伍德】给【伊芙·伍德】的信：{(letterDeliveredToEve ? "已完成" : "未完成")}";

    public override bool IsTaskComplete() => letterDeliveredToEve;

    public override void DeliverLetter(string targetResident)
    {
        dialogueIndex = 0;
        currentDialogue = targetResident == "伊芙·伍德" && !letterDeliveredToEve
            ? GetDialogueForEve()
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
            if (!letterDeliveredToEve && currentDialogue.Length > 1)
            {
                letterDeliveredToEve = true;
                if (taskManager != null && taskManager.inventoryManager != null)
                {
                    Debug.Log("Task5: 移除小卢克的信，添加伊芙的信");
                    taskManager.inventoryManager.RemoveLetter("小卢克的信");
                    taskManager.inventoryManager.AddLetter(new Letter
                    {
                        title = "伊芙的信",
                        content = "维克托先生，好久没跟你说话了，也不知道你最近怎么样。简说在弄个支架，你的腿还疼吗？我记得你说过那伤老折磨你，辐射天冷了，你得多注意，别硬撑着。我听伊莱亚斯提过你，他说你还是老样子，嘴硬得像石头，可他写诗的事你管得少了，是不是你们爷俩关系好点了？我挺替他高兴的，他老说你不懂他的诗，可我看得出他还是在乎你的。\r\n\r\n最近小卢克老问起他爸爸。他长大了，好奇心重，老缠着我问他在哪，我想知道他的故事。我知道他死在战场上，是你的部下，流弹带走了他，可我从没怪过你，战争不给人留余地，你能活着回来已经是奇迹。这些年你帮了不少，防护服、物资，总有你的份，我一直记着这份情。伊莱亚斯说你心里不好受，觉得没保住他，我不希望你背着这个负担。他走的时候，我没见着最后一面，可你一定记得他吧？他的笑，他的脾气，或者那天他说了什么……我想听听，哪怕只是几句话，我好告诉卢克，让他知道爸爸是个什么样的人。\r\n\r\n卢克让我画太阳，可我画不出，我只能画他的脸，模糊的，越画越模糊。你要是记得什么，写回来吧，不急，我等着。谢谢你，维克托，这么多年了，你还是他的兄弟。——伊芙\r\n"
                    });
                }
                else
                {
                    Debug.LogError("Task5: TaskManager 或 InventoryManager 未正确绑定");
                }
            }

            PlayerController playerController = Object.FindObjectOfType<PlayerController>();
            if (playerController != null && playerController.IsInDialogue())
                playerController.EndDialogue();

            if (IsTaskComplete())
            {
                if (taskManager != null)
                {
                    Debug.Log("Task5: 任务完成，切换到 Task6");
                    Task6 newTask = gameObject.AddComponent<Task6>();
                    taskManager.SetTask(newTask);
                    newTask.SetupTask(taskManager, dialoguePanel, dialogueText, nextButton);
                    taskManager.UpdateTaskDisplay();
                }
                else
                {
                    Debug.LogError("Task5: TaskManager 未找到，无法切换到 Task6");
                }
            }
        }
    }

    private string[] GetDialogueForEve()
    {
        return new string[]
        {
            "【伊芙·伍德】你又来了，小机器人。啊？卢克给我的信，这孩子真是的，明明住一块还要麻烦你……",
            "【……】",
            "【伊芙·伍德】唉……他说想知道爸爸的故事，还让我画太阳……这孩子，天真得让我心疼。",
            "【伊芙·伍德】他爸爸死在战场上，他是老兵维克托的部下，你应该认识维克托吧。",
            "【伊芙·伍德】我……没有见上他的最后一面，我只知道流弹带走了他。",
            "【伊芙·伍德】卢克老问爸爸在哪，我只好告诉他爸爸在远方看着他，等他长大了就会回来……",
            "【伊芙·伍德】……我也不知道这么做是不是对的，毕竟我也只会画画了……",
            "【伊芙·伍德】我一直尽力想画一些东西，我想画过去的事物，那些田野、河流，还有他爸爸活着时的笑脸，",
            "【伊芙·伍德】可每次下笔，手都在抖，画出来的只有模糊的影子。",
            "【伊芙·伍德】我想给他画一副没有战争的明天，蓝天白云，太阳高挂，孩子们在草地上跑……",
            "【伊芙·伍德】可目前我还画不出，也许未来我可以画出来吧……",
            "【伊芙·伍德】这是给维克托的信，麻烦你送过去吧。",
            "【伊芙·伍德】伊莱亚斯跟我说过老维克托一直很难受，他一直为了在战场上没保下他而内疚……",
            "【伊芙·伍德】我不怪他，我很感谢他这么多年对丈夫的关照。",
            "【伊芙·伍德】谢谢你跑这一趟，小家伙，你是这村子里最忙的了吧。"
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
            taskCompleteText.text = "任务5——画中的光";
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

            Debug.Log("任务5 开始面板已显示并隐藏");
        }
        else
        {
            Debug.LogWarning("任务完成面板或文字组件未正确初始化，无法显示任务5开始提示！");
        }
    }
}