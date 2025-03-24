using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Task1 : TaskBase
{
    private int visitCount = 0;
    private bool letterDeliveredToVictor = false;
    private bool returnedToJane = false;
    private string[] residents = { "维克托·凯恩", "伊莱亚斯·凯恩", "简·怀特", "萝丝", "小卢克·伍德", "伊芙·伍德" };
    private bool[] visitedResidents;

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
        return $"拜访信火村的每一位居民（{visitCount}/6），\n" +
               $"送达【简·怀特】给【维克托·凯恩】的信：{(letterDeliveredToVictor ? "已完成" : "未完成")}，\n" +
               $"回去找【简·怀特】：{(returnedToJane ? "已完成" : "未完成")}";
    }

    public override bool IsTaskComplete()
    {
        return visitCount >= 6 && letterDeliveredToVictor && returnedToJane;
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

    public void DeliverLetter(string targetResident)
    {
        currentResident = targetResident;
        dialogueIndex = 0;

        if (targetResident == "维克托·凯恩" && !letterDeliveredToVictor)
        {
            currentDialogue = GetDialogueForResident("维克托·凯恩");
            letterDeliveredToVictor = true;
            VisitResident(targetResident);
        }
        else if (targetResident == "简·怀特" && visitCount >= 6 && letterDeliveredToVictor && !returnedToJane)
        {
            currentDialogue = GetDialogueForResident("简·怀特");
            returnedToJane = true;
        }
        else if (System.Array.IndexOf(residents, targetResident) >= 0 && !visitedResidents[System.Array.IndexOf(residents, targetResident)])
        {
            currentDialogue = GetDialogueForResident(targetResident);
            VisitResident(targetResident);
        }
        else
        {
            currentDialogue = new string[] { "你还没有信" };
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
            Debug.Log($"拜访了 {residentName}，当前进度：{visitCount}/6");
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
                    "你是什么东西，这堆破铜烂铁是哪来的？",
                    "简让你来送信的？哼，这丫头就知道弄这些科技玩意儿……信呢？快拿来，别磨蹭。",
                    "嗯……简说要让你成为信火村的信使。哼，科技毁了我的一切，连太阳都被科技给毁了，鬼知道你会不会出什么乱子。",
                    "算了，这是给伊莱亚斯的信，告诉他别再写那些没用的诗，男人该干点正事。你要敢打开或弄丢了，我就砸了你。",
                    "……干嘛还不走，别在这儿碍眼，看着你这铁皮罐子就烦。"
                };
            case "伊莱亚斯·凯恩":
                return new string[]
                {
                    "哦？你是新的送信者吗？看你这模样，活脱脱像从废墟里生长出来的。信给我看看吧。",
                    "原来是简修好你的？她真是天才……虽然她从不懂我的诗……你见过她专注研究时的眼神吗，那可真是……可惜因为太阳的陨落，我们很难见面。",
                    "这是给她的信，告诉她我想她，想她的认真的表情，哪怕她只会给我讲机械原理和电路……",
                    "你会一直在村里送信吗？也许哪天我能为你写首诗，关于铁与灵魂的交响。"
                };
            case "简·怀特":
                return new string[]
                {
                    "回来了？比我算的时间快了两分钟，看来你的零件质量还不错。",
                    "信都送到了？好，干得不错。维克托没把你轰出来吧？他讨厌机器，他的腿就是被人工智能武装射伤的，他的妻子也在那场战争中过世了……",
                    "……还行，你比我预想的靠谱。老兵要你送信吗，好的，我帮你实时更新一下任务模块，在你的显示屏右边就可以看到了，以后这个会自动更新。",
                    "别指望我夸你，你就是个工具，干活是你的本分。"
                };
            case "萝丝":
                return new string[]
                {
                    "哎呀，你是送信的小家伙吧？真可爱，虽然……是铁做的，你的小脑袋一定很坚硬吧。",
                    "简给我的信吗？好，麻烦你了，小家伙。",
                    "原来如此，简把你给修好了啊，她可真厉害，当年她从大学跟着我来这个村子避难，还担心这闺女会觉得太压抑了，但是看看现在她和伊莱亚斯那小伙子，呵呵……",
                    "路上小心啊，辐射虽然伤不了你，可这村子里的路不好走，别摔倒了。"
                };
            case "小卢克·伍德":
                return new string[]
                {
                    "哇！你真的是机器人吗？会飞吗？会说话吗？还是会变形的呀？",
                    "你是从哪儿来的？废墟里吗？妈妈说废墟里有好多怪东西，你是怪东西吗？",
                    "啊？有我的信！简姐姐的！快让我看看。",
                    "好耶，以后我们村有自己的信使了，之前隔一个月才有分配的机器人来我们村。",
                    "你下次还来吗？我可以给你画个画，用妈妈的颜料，画个大大的你！"
                };
            case "伊芙·伍德":
                return new string[]
                {
                    "你是送信的？真稀奇，你是从哪来的。",
                    "简让你来的？她总能修好这些东西，我却只会用颜色涂抹回忆……让我看看她的信。",
                    "看来以后我们村有信使了，一切都会好起来吧，真希望卢克能看到……谢谢你跑这一趟。",
                    "别在意那些锈迹，你这样挺好，像幅旧时代的油画，带着时间的痕迹。"
                };
            default:
                return new string[] { "你还没有信" };
        }
    }
}