using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[System.Serializable]
public class Task2 : TaskBase
{
    private bool letterDeliveredToElias = false;
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

    public override string GetTaskName() => "枪杆子和笔";

    public override string GetTaskObjective() => $"送达【维克托·凯恩】给【伊莱亚斯·凯恩】的信：{(letterDeliveredToElias ? "已完成" : "未完成")}";

    public override bool IsTaskComplete() => letterDeliveredToElias;

    public override void DeliverLetter(string targetResident)
    {
        dialogueIndex = 0;
        currentDialogue = targetResident == "伊莱亚斯·凯恩" && !letterDeliveredToElias
            ? GetDialogueForElias()
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
            if (!letterDeliveredToElias && currentDialogue.Length > 1)
            {
                letterDeliveredToElias = true;
                if (taskManager != null && taskManager.inventoryManager != null)
                {
                    Debug.Log("Task2: 更新信件 - 移除维克托的信，添加伊莱亚斯的信");
                    taskManager.inventoryManager.RemoveLetter("维克托的信");
                    taskManager.inventoryManager.AddLetter(new Letter
                    {
                        title = "伊莱亚斯的信",
                        content = "简，我的理性之光，我的灵魂焰火，我的生命中最炽烈的星辰。你不懂我笔下的风暴，不解我为何在残阳下低吟，可我不在乎。简，你还记得吗？那是我们相识的夜晚，战争刚结束，辐射还未完全封锁村子，天空虽无太阳，却有一片残存的星光，像破碎的镜子洒在山坡上。你出现了，穿着美丽的长裙，手里提着工具箱测量夜晚辐射值，灯光摇曳下，你的身影像一束光照亮了我的心。你问我在干什么，我说在写诗，你皱眉笑了一声，说“诗不能点亮黑夜”。那一刻，我看见你的眼睛，清澈得像从未被战争玷污的水面，我的心跳乱了节奏。\r\n" +
                                  "后来，我常去看你。你在小屋里埋头苦干，那专注的表情，比我写过的任何诗行都动人。那一刻，我彻底沦陷了。我爱你，简。我想你，我的梦里全是你的影子。辐射隔开你我，可我愿化作飞蛾，扑向你窗前的灯 fusing，我，哪怕烧成灰，也要触碰你的温度。你不懂我的狂热，简，可这是我爱你的证明。哪怕你永远站在理性的山巅冷眼看我，我也要用这热情为你燃尽废墟，为你涂满这灰暗世界的色彩。简，这份爱永不熄灭。——伊莱亚斯"
                    });
                }
                else
                {
                    Debug.LogError("Task2: TaskManager 或 InventoryManager 未正确绑定，无法更新信件");
                }
            }

            PlayerController playerController = Object.FindObjectOfType<PlayerController>();
            if (playerController != null && playerController.IsInDialogue())
            {
                playerController.EndDialogue();
            }

            if (IsTaskComplete())
            {
                if (taskManager != null)
                {
                    Debug.Log("Task2: 任务完成，切换到 Task3");
                    Task3 newTask = gameObject.AddComponent<Task3>();
                    taskManager.SetTask(newTask);
                    newTask.SetupTask(taskManager, dialoguePanel, dialogueText, nextButton);
                    taskManager.UpdateTaskDisplay();
                }
                else
                {
                    Debug.LogError("Task2: TaskManager 未找到，无法切换到 Task3");
                }
            }
        }
    }

    private string[] GetDialogueForElias()
    {
        return new string[]
        {
            "【伊莱亚斯·凯恩】哦，又是你啊，小小的机器人。谁的信呢？让我看看。",
            "【……】",
            "【伊莱亚斯·凯恩】唉，父亲，他还是那样，父亲的字迹，像枪身一样硬，可这次……多了点什么，他以前的文字不会这么温柔的，也许在书信中，人能更加真实的表达自己吧。",
            "【伊莱亚斯·凯恩】他提到了母亲，我还记得她读我第一首诗时的笑容，她说我会成为诗人……那时候太阳还在天上。",
            "【伊莱亚斯·凯恩】后来，当太阳爆发的时候，当战场传来噩耗的时候，父亲带着我离开了城市，来到了信火村。",
            "【伊莱亚斯·凯恩】母亲没有跟我们一块离开，很多年后我才知道，她暴露在爆发的那瞬间的阳光下，父亲不肯告诉我……他否定我的诗歌——这是我纪念母亲的方式，因此我和他分居了。",
            "【伊莱亚斯·凯恩】他说战争毁了一切，我也知道，他腿上的伤疤是他从尸体堆中爬出的见证，可他不理解，诗是我的一切，是母亲存在过的证据。",
            "【伊莱亚斯·凯恩】他问我过得好不好……他竟然会问这个，那个沉默的老兵，战场上的铁人，我以为他早就忘了怎么问。",
            "【伊莱亚斯·凯恩】你知道吗，他从没像这次一样说过害怕失去我。",
            "【伊莱亚斯·凯恩】这是我给简的信，带给她吧。告诉她我想她，想她皱眉调试机器的样子，想她眼里那点冷静的光，哪怕她总笑我诗里没逻辑。",
            "【伊莱亚斯·凯恩】你会再去找他吗？替我带句话吧，就说……我没忘他扛过的枪，但我也不会丢下我的笔。",
            "【伊莱亚斯·凯恩】我会再给他回信的。"
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
                taskCompletePanel.SetActive(false);
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
            taskCompleteText.text = "任务2——枪杆子和笔";
            taskCompletePanel.SetActive(true);

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

            Debug.Log("任务2 开始面板已显示并隐藏");
        }
        else
        {
            Debug.LogWarning("任务完成面板或文字组件未正确初始化，无法显示任务2开始提示！");
        }
    }
}