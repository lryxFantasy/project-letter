using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Task6 : TaskBase
{
    private bool letterDeliveredToVictor = false;
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

    public override string GetTaskName() => "硝烟中的尘埃";

    public override string GetTaskObjective() => $"送达【伊芙·伍德】给【维克托·凯恩】的信：{(letterDeliveredToVictor ? "已完成" : "未完成")}";

    public override bool IsTaskComplete() => letterDeliveredToVictor;

    public override void DeliverLetter(string targetResident)
    {
        dialogueIndex = 0;
        currentDialogue = targetResident == "维克托·凯恩" && !letterDeliveredToVictor
            ? GetDialogueForVictor()
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
            if (!letterDeliveredToVictor && currentDialogue.Length > 1)
            {
                letterDeliveredToVictor = true;
                if (taskManager != null && taskManager.inventoryManager != null)
                {
                    Debug.Log("Task6: 移除伊芙的信，添加神秘房子的钥匙");
                    taskManager.inventoryManager.RemoveLetter("伊芙的信");
                    taskManager.inventoryManager.AddLetter(new Letter
                    {
                        title = "神秘房子的钥匙",
                        content = "老兵在家里翻出来的钥匙，似乎可以打开村子上边那座房子，也许可以看看里面有些什么。"
                    });
                }
                else
                {
                    Debug.LogError("Task6: TaskManager 或 InventoryManager 未正确绑定");
                }
            }

            PlayerController playerController = Object.FindObjectOfType<PlayerController>();
            if (playerController != null && playerController.IsInDialogue())
                playerController.EndDialogue();

            if (IsTaskComplete())
            {
                if (taskManager != null)
                {
                    Debug.Log("Task6: 任务完成，切换到 Task7");
                    Task7 newTask = gameObject.AddComponent<Task7>();
                    taskManager.SetTask(newTask);
                    newTask.SetupTask(taskManager, dialoguePanel, dialogueText, nextButton);
                    taskManager.UpdateTaskDisplay();
                }
                else
                {
                    Debug.LogError("Task6: TaskManager 未找到，无法切换到 Task7");
                }
            }
        }
    }

    private string[] GetDialogueForVictor()
    {
        return new string[]
        {
            "【维克托·凯恩】是你啊，铁皮罐头……又跑来送信了？进来吧，关于我儿子的事，还得谢谢你了……信呢？拿给我。",
            "【……】",
            "【维克托·凯恩】唉，伊芙这女人……她说她不怪我。可我对不起他们一家，小卢克的父亲，卢克，他死在我面前，我没保住他……",
            "【维克托·凯恩】那天战场上很混乱，子弹在我耳边呼啸着掠过，空气中硝烟味刺鼻到让人窒息，天红得像血，我喊他趴下，可他没听见，流弹穿过他的胸口，血溅了我一身……",
            "【维克托·凯恩】我拖着这条烂腿爬过去时，他已经只剩一口气了，他伸手握住我，另一只手里还攥着枪，他想说些什么，可已经没声了，但我知道，他在乎他们娘俩。",
            "【维克托·凯恩】我没做到……没让他活着回去见他的儿子。",
            "【维克托·凯恩】这条腿是那时候废的，疼得我睡不着，可我活着回来了，他没回。伊芙说她不怪我，可我过不了自己这关……战争结束后，我把伊芙他们接到了信火村避难。",
            "【维克托·凯恩】每次看到小卢克那孩子，我就想起卢克，想起他每次提到儿子脸上的笑容……我欠他们的，太多了。",
            "【维克托·凯恩】也许是时候释怀了，我会给他们回信的，我想把记忆里的卢克写下来，可能+，可能需要点时间吧……告诉伊芙我会回信的。替我跟她说……让她别太累，小卢克还得靠她。",
            "【维克托·凯恩】哦对了，这是我在家里翻出来的钥匙，似乎可以打开村子上边那座房子，我是没机会出门了，也许你可以看看里面有些什么。"
        };
    }
}