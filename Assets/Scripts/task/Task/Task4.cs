using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[System.Serializable]
public class Task4 : TaskBase
{
    private bool visitedRose = false;
    private bool letterDeliveredToLuke = false;
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

    public override string GetTaskName() => "神秘朋友";

    public override string GetTaskObjective() => $"拜访【萝丝】，\n" +
                                                 $"送达【萝丝】给【小卢克·伍德】的信：{(letterDeliveredToLuke ? "已完成" : "未完成")}";

    public override bool IsTaskComplete() => visitedRose && letterDeliveredToLuke;

    public override void DeliverLetter(string targetResident)
    {
        dialogueIndex = 0;
        if (targetResident == "萝丝" && !visitedRose)
        {
            currentDialogue = GetDialogueForRose();
        }
        else if (targetResident == "小卢克·伍德" && visitedRose && !letterDeliveredToLuke)
        {
            currentDialogue = GetDialogueForLuke();
        }
        else
        {
            currentDialogue = new string[] { "【……】你还没有信" };
        }

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
            if (taskManager != null && taskManager.inventoryManager != null)
            {
                if (!visitedRose && currentDialogue.Length > 1)
                {
                    visitedRose = true;
                    Debug.Log("Task4: 添加萝丝的信");
                    taskManager.inventoryManager.AddLetter(new Letter
                    {
                        title = "萝丝的信",
                        content = "亲爱的小朋友，今天夜里我坐在窗边，听着外面的声音，想起了从前的日子。那时候，世界还没被辐射笼罩，天空蓝得像一块洗净的布，太阳挂在上头，金灿灿的，像个大灯笼，暖得能融化人的心。早上，露珠会挂在草叶上，像一颗颗小珍珠，闪着光，风一吹，草就跳起舞，发出沙沙的歌声。河边有柳树，长长的枝条垂下来，像姑娘的头发，轻轻拂过水面，水清得能看见鱼儿游来游去。我还记得夏天的夜晚，蛐蛐在草丛里唱歌，萤火虫提着小灯笼到处飞，像天上的星星掉下来玩耍。大人们坐在院子里摇扇子，讲故事。冬天呢，下雪的时候，雪花飘下来，轻得像羽毛，落在地上就堆成厚厚的毯子，我们裹着围巾堆雪人，打雪仗，笑声传遍整个村子。\r\n\r\n小朋友，这些美好的东西，现在只能在心里想想了，可我想告诉你，成长不只是等着世界变好，而是学会在灰暗里找到光。你要勇敢，像你妈妈一样，她用双手撑起你们的生活。你要善良，伸出手，帮助别人，世界才会更加美好。你要好奇，不断追问为什么，总有一天天空会因为你们的求索而再次变蓝。我不知道你能不能明白这些，可我希望你长大后，心里有片自己的花园，哪怕外面是废墟，里面也能开满花。你要和妈妈一起好好生活，像太阳还在天上时那样，带着光，带着热。——神秘朋友\r\n"
                    });
                }
                else if (visitedRose && !letterDeliveredToLuke && currentDialogue.Length > 1)
                {
                    letterDeliveredToLuke = true;
                    Debug.Log("Task4: 移除萝丝的信，添加小卢克的信");
                    taskManager.inventoryManager.RemoveLetter("萝丝的信");
                    taskManager.inventoryManager.AddLetter(new Letter
                    {
                        title = "小卢克的信",
                        content = "妈妈，我今天在屋里跑来跑去，想看看风会不会唱歌，可窗户关着，我只能听见它敲玻璃，砰砰砰，像在跟我玩！我好想出去，可你说辐射很坏，不能跑出去。爸爸在远方看着我吗，我好想他，我想问他在哪，想知道他的故事。我画了好多画，有树，有房子，还有你笑的样子，可是我不会画爸爸，因为我没见过他，我只知道妈妈你说他是一个很好很好的人，我想他。\r\n\r\n你老在屋里画画，是不是很累呀？我昨天偷看你，画了一个英俊的脸，那么多线条，那么多颜色，我问你那是啥，你说那是爸爸的样子！我长大也会变成这个样子吗？我觉得你画得真好，比我厉害一百万倍，我怎么画都画不出你那样！我爱你，妈妈，你是最好的妈妈，像故事里的魔法师，能把颜色变成好多东西。能不能画个大太阳给我呀？我想知道它是不是真的像灯笼那么亮，哪怕我只能在屋里看！——卢克\r\n"
                    });
                }
            }
            else
            {
                Debug.LogError("Task4: TaskManager 或 InventoryManager 未正确绑定");
            }

            PlayerController playerController = Object.FindObjectOfType<PlayerController>();
            if (playerController != null && playerController.IsInDialogue())
                playerController.EndDialogue();

            if (IsTaskComplete())
            {
                if (taskManager != null)
                {
                    Debug.Log("Task4: 任务完成，切换到 Task5");
                    Task5 newTask = gameObject.AddComponent<Task5>();
                    taskManager.SetTask(newTask);
                    newTask.SetupTask(taskManager, dialoguePanel, dialogueText, nextButton);
                    taskManager.UpdateTaskDisplay();
                }
                else
                {
                    Debug.LogError("Task4: TaskManager 未找到，无法切换到 Task5");
                }
            }
        }
    }

    private string[] GetDialogueForRose()
    {
        return new string[]
        {
            "【萝丝】哎呀，又是你这送信的小家伙，还是那么硬邦邦的。",
            "【萝丝】简让你来的吧，她还是这么贴心，老想着我这个老婆子。",
            "【萝丝】虽然我和她是远房亲戚，但在我的孙子去世后，她就是我在这个世界上唯一的亲人了。",
            "【萝丝】当年那场浩劫后，我的孩子们都分离了，有的出现在了广播中的死者名单里，有的失踪了，再没音讯，我和孙子相依为命……",
            "【萝丝】……后来连他也没保住，辐射带走了他，留我一个人守着这小屋。",
            "【萝丝】简是那时候找到我的，她爸是我侄子，战前就没了，留下她一个人跑来投奔我。",
            "【萝丝】她从大学逃到这村子时，她才十九岁，背着个破包，瘦得像根柴……",
            "【萝丝】现在都好起来了，她在村子里也有依靠了。",
            "【萝丝】哎呀，我又说多了，忘了要拜托你的事情了。",
            "【萝丝】其实我没啥事，就是想让你帮我送封信给小卢克。那孩子可不容易了，在战争中出生，从来没见过父亲……唉，人啊……",
            "【萝丝】那孩子好奇心强，有点像我那孙子……",
            "【萝丝】我想尽我可能让他多了解一点旧时代的事，他从来没见过太阳……我是不是又说多了。",
            "【萝丝】我没写名字，就叫我“神秘朋友”，别告诉他是我给他的信，我怕他嫌我是这么个老婆子，觉得无趣。",
            "【萝丝】麻烦你这小家伙了。"
        };
    }

    private string[] GetDialogueForLuke()
    {
        return new string[]
        {
            "【小卢克】哇，又是你！我的机器人朋友！你送得好快，有信给我吗？",
            "【……】",
            "【小卢克】神秘朋友讲了好多好玩的东西！水里这种叫“鱼”的是什么，它们不用呼吸吗？",
            "【小卢克】虽然我没见过太阳，但我画了好多张想象中的太阳想给神秘朋友看，可惜妈妈说太阳没了……",
            "【小卢克】真好奇神秘朋友是谁，他怎么懂这么多东西，好厉害，其实我有个猜测……",
            "【小卢克】机器人，我只告诉你哦……",
            "【小卢克】我猜他是——我爸爸！他一定是没法回来悄悄给我写信呢。",
            "【小卢克】他让我勇敢，像妈妈一样，我觉得妈妈真的很厉害，她老给我讲爸爸的故事，说他在远方注视着我。",
            "【小卢克】你知道吗，我和爸爸名字一模一样！他是大卢克，我是小卢克。",
            "【小卢克】这是我给妈妈的信，麻烦你送过去吧！",
            "【小卢克】你问我明明都在家里，为啥不直接给她……可我感觉这样更有意思一点！",
            "【小卢克】你帮我送，这样肯定很有惊喜感。",
            "【小卢克】每当提起爸爸妈妈总是会难过……我想让她开心一点。",
            "【小卢克】你下次还来吗？我画了个大大的你，铁皮亮亮的，可帅了！",
            "【小卢克】下次有机会帮我问问神秘朋友“鱼”是长什么样的！"
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
            taskCompleteText.text = "任务4——神秘朋友";
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

            Debug.Log("任务4 开始面板已显示并隐藏");
        }
        else
        {
            Debug.LogWarning("任务完成面板或文字组件未正确初始化，无法显示任务4开始提示！");
        }
    }
}