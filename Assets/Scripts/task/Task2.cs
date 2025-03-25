using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Task2 : TaskBase
{
    private bool letterDeliveredToElias = false;
    private TMP_Text dialogueText;
    private GameObject dialoguePanel;
    private Button nextButton;
    private string currentResident;
    private string[] currentDialogue;
    private int dialogueIndex = 0;

    public override string GetTaskName()
    {
        return "枪杆子和笔";
    }

    public override string GetTaskObjective()
    {
        return $"送达【维克托·凯恩】给【伊莱亚斯·凯恩】的信：{(letterDeliveredToElias ? "已完成" : "未完成")}";
    }

    public override bool IsTaskComplete()
    {
        return letterDeliveredToElias;
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

    public override void DeliverLetter(string targetResident)
    {
        currentResident = targetResident;
        dialogueIndex = 0;

        if (targetResident == "伊莱亚斯·凯恩" && !letterDeliveredToElias)
        {
            currentDialogue = GetDialogueForResident("伊莱亚斯·凯恩");
            letterDeliveredToElias = true;
            // 移除信件
            InventoryManager inventory = FindObjectOfType<InventoryManager>();
            if (inventory != null) inventory.RemoveLetter("维克托给伊莱亚斯的信");
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
            UpdateDisplay();
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null && playerController.IsInDialogue())
            {
                playerController.EndDialogue();
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
            case "伊莱亚斯·凯恩":
                return new string[]
                {
                    "【伊莱亚斯·凯恩】哦，又是你啊，小小的机器人。",
                    "【伊莱亚斯·凯恩】谁的信呢？让我看看。",
                    "【……】",
                    "【伊莱亚斯·凯恩】唉，父亲，他还是那样，父亲的字迹，像枪身一样硬，可这次……多了点什么，他以前的文字不会这么温柔的，也许在书信中，人能更加真实的表达自己吧。",
                    "【伊莱亚斯·凯恩】他提到了母亲，我还记得她读我第一首诗时的笑容，她说我会成为诗人……那时候太阳还在天上。",
                    "【伊莱亚斯·凯恩】后来，当太阳爆发的时候，当战场传来噩耗的时候，父亲带着我离开了城市，来到了信火村。",
                    "【伊莱亚斯·凯恩】母亲没有跟我们一块离开，很多年后我才知道，她暴露在爆发的那瞬间的阳光下，父亲不肯告诉我……他否定我的诗歌——这是我纪念母亲的方式，因此我和他分居了。",
                    "【伊莱亚斯·凯恩】他说战争毁了一切，我也知道，他腿上的伤疤是他从尸体堆中爬出的见证，可他不理解，诗是我的一切，是母亲存在过的证据。",
                    "【伊莱亚斯·凯恩】他问我过得好不好……他竟然会问这个，那个沉默的老兵，战场上的铁人，我以为他早就忘了怎么问。",
                    "【伊莱亚斯·凯恩】你知道吗，他从没说过害怕失去我。",
                    "【伊莱亚斯·凯恩】这是我给简的信，带给她吧。",
                    "【伊莱亚斯·凯恩】告诉她我想她，想她皱眉调试机器的样子，想她眼里那点冷静的光，哪怕她总笑我诗里没逻辑。",
                    "【伊莱亚斯·凯恩】你会再去找他吗？替我带句话吧，就说……我没忘他扛过的枪，但我也不会丢下我的笔。",
                    "【伊莱亚斯·凯恩】我会再给他回信的。"
                };
            default:
                return new string[] { "【……】你还没有信" };
        }
    }
}