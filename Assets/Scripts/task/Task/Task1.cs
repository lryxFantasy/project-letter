using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Task1 : TaskBase
{
    [SerializeField] private int visitCount = 0;
    [SerializeField] private bool letterDeliveredToVictor = false;
    [SerializeField] private bool returnedToJane = false;
    [SerializeField] private string[] residents = { "维克托·凯恩", "伊莱亚斯·凯恩", "简·怀特", "萝丝", "小卢克·伍德", "伊芙·伍德" };
    [SerializeField] private bool[] visitedResidents;

    private TMP_Text dialogueText;
    private GameObject dialoguePanel;
    private Button nextButton;

    private GameObject normalDialoguePanel;
    private Button deliverButton;

    private string[] currentDialogue;
    private int dialogueIndex = 0;
    private string currentResident;

    void Start()
    {
        visitedResidents = new bool[residents.Length];
    }

    public override string GetTaskName()
    {
        return "信火村的第一封信";
    }

    public override string GetTaskObjective()
    {
        return $"拜访信火村的简以外每一位居民（{visitCount}/5），\n" +
               $"送达【简·怀特】给【维克托·凯恩】的信：{(letterDeliveredToVictor ? "已完成" : "未完成")}，\n" +
               $"回去找【简·怀特】：{(returnedToJane ? "已完成" : "未完成")}";
    }

    public override bool IsTaskComplete()
    {
        return visitCount >= 5 && letterDeliveredToVictor && returnedToJane;
    }

    public void SetupDialogueUI(GameObject panel, TMP_Text text, Button button)
    {
        dialoguePanel = panel;
        dialogueText = text;
        nextButton = button;
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(NextDialogue);
        dialoguePanel.SetActive(false);
    }

    public void SetupDeliverButton(GameObject normalPanel, Button deliverBtn)
    {
        normalDialoguePanel = normalPanel;
        deliverButton = deliverBtn;
        deliverButton.onClick.RemoveAllListeners();
        deliverButton.onClick.AddListener(() =>
        {
            normalDialoguePanel.SetActive(false); // 隐藏普通对话框
            GetComponent<TaskManager>().TriggerDeliverLetter(); // 调用TaskManager的送信方法
        });
    }

    public override void DeliverLetter(string targetResident)
    {
        currentResident = targetResident;
        dialogueIndex = 0;

        if (targetResident == "维克托·凯恩" && !letterDeliveredToVictor)
        {
            currentDialogue = GetDialogueForResident("维克托·凯恩");
            letterDeliveredToVictor = true;
            VisitResident(targetResident);
            // 移除简的信并添加维克托的信
            TaskManager taskManager = GetComponent<TaskManager>();
            if (taskManager != null && taskManager.inventoryManager != null)
            {
                Debug.Log("获得新信件");
                taskManager.inventoryManager.RemoveLetter("简给维克托的信");
                taskManager.inventoryManager.AddLetter(new Letter
                {
                    title = "维克托的信",
                    content = "伊莱亚斯，简弄了个破机器送信，FANLU什么型号，满身锈迹，看着不顺眼，我是不喜欢这些科技玩意儿，可她说这是唯一的办法。辐射把人困住了，出不去，防护服又不够，只能靠这堆铁皮跑腿。她说修它花了两天，废墟里捡的零件，也算她有本事……你还在写那些诗吗？我知道你喜欢，从小就爱摆弄纸笔，我不明白那些有什么用，可你妈在的时候总说你有天分，说你能写出她听不懂却喜欢的句子。战争把一切都毁了，太阳没了，日子过得像鬼一样，我腿也不行了，那场战役中流弹打中我，疼得我三天没睡，我眼睁睁看着战友倒了一片，血染红了阵地，你知道的，也包括小卢克的父亲，唉……留下他们母子两人……导弹飞向太阳那天，我在指挥部，眼睁睁看着天塌下来的样子，你妈后来也撑不住辐射走了。我不知道我们的生活是否还有明天，但生活总是得继续的吧……\r\n你那边怎么样？村里人说你跟简在一块了，她脑子清楚，人挺实在，修机器的手艺没得挑，你能跟她学点东西也不坏。我不是说你那些诗没用，就是……想知道你过得好不好，毕竟你是我儿子。诗歌救不了我在战场上死去的战友，可我也不想再失去你。简说这机器靠得住，能跑得动，有话就写回来，别让我老等着。你要没空就算了，别勉强。——维克托\r\n"
                });
            }
            else
            {
                Debug.LogError("TaskManager 或 InventoryManager 未正确绑定，无法更新背包");
            }
        }
        else if (targetResident == "简·怀特")
        {
            currentDialogue = GetDialogueForResident("简·怀特");
            if (visitCount >= 5 && letterDeliveredToVictor && !returnedToJane)
            {
                returnedToJane = true;
            }
        }
        else if (System.Array.IndexOf(residents, targetResident) >= 0 && !visitedResidents[System.Array.IndexOf(residents, targetResident)])
        {
            currentDialogue = GetDialogueForResident(targetResident);
            VisitResident(targetResident);
        }
        else
        {
            currentDialogue = new string[] { "【……】你还没有信" };
        }

        StartDialogue();
    }

    private void VisitResident(string residentName)
    {
        int index = System.Array.IndexOf(residents, residentName);
        if (index >= 0 && !visitedResidents[index])
        {
            visitedResidents[index] = true;
            visitCount++;
            Debug.Log($"拜访了 {residentName}，当前进度：{visitCount}/5");
            UpdateDisplay();
        }
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
            UpdateDisplay();
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null && playerController.IsInDialogue())
            {
                playerController.EndDialogue();
            }
            // 检查任务是否完成并跳转到 Task2
            if (IsTaskComplete())
            {
                TaskManager taskManager = GetComponent<TaskManager>();
                if (taskManager != null)
                {
                    Task2 newTask = gameObject.AddComponent<Task2>();
                    taskManager.SetTask(newTask);
                    newTask.SetupTask(taskManager, dialoguePanel, dialogueText, nextButton); // 修改为 SetupTask
                    taskManager.UpdateTaskDisplay();
                }
            }
        }
    }

    private void UpdateDisplay()
    {
        TaskManager manager = GetComponent<TaskManager>();
        if (manager != null)
        {
            manager.UpdateTaskDisplay();
        }
    }

    private string[] GetDialogueForResident(string residentName)
    {
        switch (residentName)
        {
            case "维克托·凯恩":
                return new string[]
                {
                    "【维克托·凯恩】你是什么东西，这堆破铜烂铁是哪来的？",
                    "【维克托·凯恩】简让你来送信的？",
                    "【维克托·凯恩】哼，这丫头就知道弄这些科技玩意儿……",
                    "【维克托·凯恩】信呢？快拿来，别磨蹭。",
                    "【……】",
                    "【维克托·凯恩】嗯……简说要让你成为信火村的信使。",
                    "【维克托·凯恩】哼，科技毁了我的一切，连太阳都被科技给毁了，鬼知道你会不会出什么乱子。",
                    "【维克托·凯恩】算了，这是给伊莱亚斯的信，告诉他别再写那些没用的诗，男人该干点正事。",
                    "【维克托·凯恩】你要敢打开或弄丢了，我就砸了你。",
                    "【维克托·凯恩】……干嘛还不走，别在这儿碍眼，看着你这铁皮罐子就烦。"
                };
            case "伊莱亚斯·凯恩":
                return new string[]
                {
                    "【伊莱亚斯·凯恩】哦？你是新的送信者吗？看你这模样，活脱脱像从废墟里生长出来的。",
                    "【伊莱亚斯·凯恩】信给我看看吧。",
                    "【……】",
                    "【伊莱亚斯·凯恩】原来是简修好你的？她真是天才……虽然她从不懂我的诗……",
                    "【伊莱亚斯·凯恩】你见过她专注研究时的眼神吗，那可真是……",
                    "【伊莱亚斯·凯恩】可惜因为太阳的陨落，我们很难见面。",
                    "【伊莱亚斯·凯恩】告诉她我想她，想她的认真的表情，哪怕她只会给我讲机械原理和电路……",
                    "【伊莱亚斯·凯恩】你会一直在村里送信吗？也许哪天我能为你写首诗，关于铁与灵魂的交响。"
                };
            case "简·怀特":
                if (visitCount >= 5 && letterDeliveredToVictor && !returnedToJane)
                {
                    returnedToJane = true;
                    return new string[]
                    {
                        "【简·怀特】回来了？比我算的时间快了两分钟，看来你的零件质量还不错。",
                        "【简·怀特】信都送到了？好，干得不错。",
                        "【简·怀特】维克托没把你轰出来吧？他讨厌机器，他的腿就是被人工智能武装射伤的，他的妻子也在那场战争中过世了……",
                        "【简·怀特】……还行，你比我预想的靠谱。",
                        "【简·怀特】老兵要你送信吗，好的，我帮你实时更新一下任务模块。",
                        "【简·怀特】在你的显示屏右边就可以看到了，以后这个会自动更新。",
                        "【简·怀特】别指望我夸你，你就是个工具，干活是你的本分。"
                    };
                }
                else
                {
                    return new string[]
                    {
                        "【简·怀特】你送完信了吗？",
                        "【简·怀特】快去吧，别在这儿浪费时间，村里还有其他人等着你送信呢。"
                    };
                }
            case "萝丝":
                return new string[]
                {
                    "【萝丝】哎呀，你是送信的小家伙吧？",
                    "【萝丝】真可爱，虽然……是铁做的，你的小脑袋一定很坚硬吧。",
                    "【萝丝】简给我的信吗？好，麻烦你了，小家伙。",
                    "【……】",
                    "【萝丝】原来如此，简把你给修好了啊，她可真厉害。",
                    "【萝丝】当年她从大学跟着我来这个村子避难，还担心这闺女会觉得太压抑了，但是看看现在她和伊莱亚斯那小伙子，呵呵……",
                    "【萝丝】路上小心啊，辐射虽然伤不了你，可这村子里的路不好走，别摔倒了。"
                };
            case "小卢克·伍德":
                return new string[]
                {
                    "【小卢克·伍德】哇！你真的是机器人吗？会飞吗？会说话吗？还是会变形的呀？",
                    "【小卢克·伍德】你是从哪儿来的？废墟里吗？妈妈说废墟里有好多怪东西，你是怪东西吗？",
                    "【小卢克·伍德】啊？有我的信！简姐姐的！快让我看看。",
                    "【……】",
                    "【小卢克·伍德】好耶，以后我们村有自己的信使了。",
                    "【小卢克·伍德】之前隔一个月才有分配的机器人来我们村。",
                    "【小卢克·伍德】你下次还来吗？我可以给你画个画，用妈妈的颜料，画个大大的你！"
                };
            case "伊芙·伍德":
                return new string[]
                {
                    "【伊芙·伍德】你是送信的？真稀奇，你是从哪来的。",
                    "【伊芙·伍德】简让你来的？她总能修好这些东西，我却只会用颜色涂抹回忆……让我看看她的信。",
                    "【……】",
                    "【伊芙·伍德】看来以后我们村有信使了，一切都会好起来吧。",
                    "【伊芙·伍德】真希望卢克能看到……谢谢你跑这一趟。",
                    "【伊芙·伍德】别在意那些锈迹，你这样挺好，像幅旧时代的油画，带着时间的痕迹。"
                };
            default:
                return new string[] { "【……】你还没有信" };
        }
    }
}