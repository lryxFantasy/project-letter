using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Task0 : TaskBase
{
    private TMP_Text dialogueText;
    private GameObject dialoguePanel;
    private Button nextButton;

    // 引导面板相关
    private GameObject guidePanel;
    private Button guideContinueButton;
    private GameObject tutorial1; // 新手教程1图片对象
    private GameObject tutorial2; // 新手教程2图片对象
    private bool isTutorial1Active = true; // 跟踪当前显示的教程
    private RubyController rubyController;
    private PlayerController playerController;
    private string[] currentDialogue;
    private int dialogueIndex = 0;
    private bool hasStarted = false;

    private string[] janeDialogue = new string[]
    {
        "简·怀特：你醒了？",
        "简·怀特：我花了两天时间修好你，用了些废墟里收集来的零件，可能不太稳定，但应该够用。",
        "简·怀特：听着，你是FANLU-317，对吧？",
        "简·怀特：我不知道你还记不记得自己的功能，不过从现在开始，你有新任务了。",
        "简·怀特：在你的显示屏右边，有你的任务模块和信件背包。",
        "简·怀特：任务模块可以跟踪当前任务进度。",
        "简·怀特：信件背包可以查看获得的信件。",
        "简·怀特：显示屏左上角是你的剩余电量，还可以查看你所处环境的辐射值，暴露在辐射环境下会不断消耗你的电量。",
        "简·怀特：我帮你安了个盖革计数器，按空格键可以查看周围的辐射区域——千万不要走进辐射区域里，这是自寻死路。",
        "简·怀特：不同的天气会影响你的电量损耗……你最好祈祷别遇到辐射风暴天气。",
        "简·怀特：快没电时，可以去其他人家中寻求帮助，家家都有发电机，可以帮你充电。",
        "简·怀特：假如你电量耗尽……那我就只能穿上防护服把你拖回我家了。",
        "简·怀特：所以……注意你的电量，我不想浪费力气。",
        "简·怀特：我们在信火村，这地方不大，但人跟人之间……隔得挺远。",
        "简·怀特：辐射让大家没法随便出门，村里的人只能靠书信联系。",
        "简·怀特：可惜送信这事总不能让人亲自跑——防护服成本太高了。",
        "简·怀特：所以，你来干这个。",
        "简·怀特：去村里每个人的住处走一趟认认路吧，之前老兵似乎说想送封信给他儿子，你可以去看看……",
        "简·怀特：别问我为什么不用无线电——那玩意儿早没用了，辐射把一切都烧坏了。",
        "简·怀特：你是唯一的办法。",
        "简·怀特：这是我写的，给老维克托的，另外我给每个村民写了封简信。",
        "简·怀特：走吧，别磨蹭，任务完成后回来找我。"
    };

    private Vector3 teleportPosition = new Vector3(-7.3f, -3.5f, -6.1f);

    void Start()
    {
        // 确保初始音乐正确
        AudioManager audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            CameraController cameraController = FindObjectOfType<CameraController>();
            if (cameraController != null && cameraController.IsIndoors())
            {
                audioManager.ForceSwitchBGM(true); // 强制播放屋内音乐
            }
        }
    }

    public void SetupDialogueUI(GameObject panel, TMP_Text text, Button button)
    {
        dialoguePanel = panel;
        dialogueText = text;
        nextButton = button;
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(NextDialogue);
        dialoguePanel.SetActive(false);

        // 动态查找引导面板和按钮
        guidePanel = GameObject.Find("GuidePanel");
        if (guidePanel != null)
        {
            guidePanel.SetActive(false);
            guideContinueButton = guidePanel.GetComponentInChildren<Button>();
            if (guideContinueButton != null)
            {
                guideContinueButton.onClick.RemoveAllListeners();
                guideContinueButton.onClick.AddListener(OnGuideContinue);
            }

            // 查找新手教程1和教程2的图片对象
            tutorial1 = guidePanel.transform.Find("Tutorial1")?.gameObject;
            tutorial2 = guidePanel.transform.Find("Tutorial2")?.gameObject;

            // 确保找到教程对象并设置初始状态
            if (tutorial1 != null && tutorial2 != null)
            {
                tutorial1.SetActive(true);
                tutorial2.SetActive(false);
            }
            else
            {
                Debug.LogWarning("未找到 Tutorial1 或 Tutorial2 对象！");
            }
        }
    }

    public void StartTaskDialogue()
    {
        if (!hasStarted)
        {
            rubyController = FindObjectOfType<RubyController>();
            playerController = FindObjectOfType<PlayerController>();
            rubyController.pauseHealthUpdate = true;
            hasStarted = true;
            currentDialogue = janeDialogue;
            dialogueIndex = 0;
            TaskManager taskManager = GetComponent<TaskManager>();
            if (taskManager != null && taskManager.normalDialoguePanel != null)
            {
                taskManager.normalDialoguePanel.SetActive(false);
            }
            StartCoroutine(StartDialogueWithFadeOut());
        }
    }

    private IEnumerator StartDialogueWithFadeOut()
    {
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        dialoguePanel.SetActive(true);
        dialogueText.text = currentDialogue[dialogueIndex];
        yield return StartCoroutine(FadeManager.Instance.FadeOut(3f));
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
            ShowGuidePanel(); // 显示引导面板

        }
    }

    private void ShowGuidePanel()
    {
        if (guidePanel != null)
        {
            guidePanel.SetActive(true);
            // 重置教程状态
            isTutorial1Active = true;
            if (tutorial1 != null && tutorial2 != null)
            {
                tutorial1.SetActive(true);
                tutorial2.SetActive(false);
            }
            Time.timeScale = 0f; // 暂停游戏时间
        }
        else
        {
            Debug.LogWarning("GuidePanel 未找到，直接进入传送！");
            StartCoroutine(TransitionAndTeleport());
        }
    }

    private void OnGuideContinue()
    {
        if (isTutorial1Active)
        {
            // 从教程1切换到教程2
            if (tutorial1 != null && tutorial2 != null)
            {
                tutorial1.SetActive(false);
                tutorial2.SetActive(true);
                isTutorial1Active = false;
            }
        }
        else
        {
            // 关闭引导面板并继续传送
            if (guidePanel != null)
            {
                guidePanel.SetActive(false);
                Time.timeScale = 1f; // 恢复游戏时间
            }
            StartCoroutine(TransitionAndTeleport());
        }
    }

    private IEnumerator TransitionAndTeleport()
    {
        yield return StartCoroutine(FadeManager.Instance.FadeToBlack(() =>
        {
            dialoguePanel.SetActive(false);
            TeleportPlayer();
            SetupNextTask();

            // 切换到屋外音乐
            AudioManager audioManager = AudioManager.Instance;
            if (audioManager != null)
            {
                audioManager.ForceSwitchBGM(false); // 强制播放屋外音乐
            }

            // 更新 CameraController 的 isIndoors 状态
            CameraController cameraController = FindObjectOfType<CameraController>();
            if (cameraController != null)
            {
                cameraController.ExitHouse(); // 模拟退出房屋，设置 isIndoors = false
            }
        }, 1f));
    }

    private void TeleportPlayer()
    {
        rubyController = FindObjectOfType<RubyController>();
        rubyController.pauseHealthUpdate = false;
        if (playerController != null)
        {
            playerController.transform.position = teleportPosition;
            playerController.enabled = true;
            if (playerController.IsInDialogue())
            {
                playerController.EndDialogue();
            }
        }
        else
        {
            Debug.LogWarning("未找到 PlayerController，无法传送玩家！");
        }
    }

    private void SetupNextTask()
    {
        TaskManager taskManager = GetComponent<TaskManager>();
        if (taskManager != null)
        {
            Task1 newTask = gameObject.AddComponent<Task1>();
            taskManager.SetTask(newTask);
            newTask.SetupDialogueUI(dialoguePanel, dialogueText, nextButton); // 仅传递3个参数
            taskManager.UpdateTaskDisplay();
        }
    }

    public override string GetTaskName()
    {
        return "初始对话";
    }

    public override string GetTaskObjective()
    {
        return "与简·怀特对话";
    }

    public override bool IsTaskComplete()
    {
        return dialogueIndex >= currentDialogue.Length;
    }

    public override void DeliverLetter(string targetResident)
    {
        Debug.Log("Task0: 这只是初始对话任务，无法送信。");
        if (dialoguePanel != null && dialogueText != null)
        {
            dialoguePanel.SetActive(true);
            dialogueText.text = "简·怀特：我已经给你任务了，快去送信吧。";
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(() => dialoguePanel.SetActive(false));
        }
    }
}