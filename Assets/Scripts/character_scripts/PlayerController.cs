using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private Text interactionText; // UI文本（提示“按 E 对话”）
    [SerializeField] private Sprite bottomSpriteImage; // Sprite切片资源
    [SerializeField] private Vector3 offset = new Vector3(50f, 50f, 0f); // 屏幕空间偏移
    [SerializeField] private float fadeDuration = 0.1f; // 淡入淡出持续时间
    [SerializeField] private Text dialogueText; // 对话显示文本
    [SerializeField] private GameObject dialoguePanel; // 对话面板
    [SerializeField] private Button optionButtonPrefab; // 按钮预制体
    [SerializeField] private Transform optionsContainer; // 按钮的父对象（用于排列）
    private Transform nearestNPC;
    private NPC currentNPC; // 当前交互的NPC
    private WalkAnimation currentNPCWalkAnimation; // 当前NPC的WalkAnimation组件
    private bool canInteract = false;
    private bool isInDialogue = false;
    private Camera mainCamera;
    private Image bottomSprite;
    private bool isFading = false;
    private Coroutine sendMessageCoroutine; // 存储正在运行的协程

    // QwenChat相关
    private string apiKey = "sk-e11794d89a4f492988f8b2b39a4ddf0a"; // 千问 API Key
    private string apiUrl = "https://dashscope.aliyuncs.com/compatible-mode/v1/chat/completions";
    private List<Message> conversationHistory = new List<Message>();
    private List<Button> optionButtons = new List<Button>(); // 存储动态创建的按钮

    [System.Serializable]
    private class RequestBody
    {
        public string model = "qwen-plus-2025-01-25";
        public Message[] messages;
        public float temperature = 1.2f; // 添加温度参数，设置为1.2以增加多样性
    }

    [System.Serializable]
    public class Message
    {
        public string role;
        public string content;
    }

    [System.Serializable]
    public class QwenResponse
    {
        public Choice[] choices;
    }

    [System.Serializable]
    public class Choice
    {
        public Message message;
    }

    void Start()
    {
        mainCamera = Camera.main;
        interactionText.gameObject.SetActive(false);
        dialoguePanel.SetActive(false);

        GameObject spriteObject = new GameObject("BottomSprite");
        spriteObject.transform.SetParent(interactionText.transform.parent);
        bottomSprite = spriteObject.AddComponent<Image>();
        bottomSprite.sprite = bottomSpriteImage;
        bottomSprite.gameObject.SetActive(false);
        bottomSprite.rectTransform.sizeDelta = new Vector2(80f, 45f);

        interactionText.canvasRenderer.SetAlpha(0f);
        bottomSprite.canvasRenderer.SetAlpha(0f);

        // 初始化对话历史（先留空，具体提示在StartDialogue中设置）
        conversationHistory.Clear();
    }

    void Update()
    {
        CheckForNearbyNPC();

        if (interactionText.gameObject.activeSelf)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);
            interactionText.rectTransform.position = screenPos + offset;

            float textHeight = interactionText.rectTransform.sizeDelta.y / 2;
            float spriteOffsetY = textHeight - 20f;
            Vector3 spriteOffset = new Vector3(0, -spriteOffsetY, 0);
            bottomSprite.rectTransform.position = interactionText.rectTransform.position + spriteOffset;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isInDialogue)
            {
                if (canInteract)
                {
                    StartDialogue();
                }
            }
            else
            {
                EndDialogue();
            }
        }
    }

    // 提供公共方法供其他脚本检查对话状态
    public bool IsInDialogue()
    {
        return isInDialogue;
    }

    void CheckForNearbyNPC()
    {
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
        float closestDistance = Mathf.Infinity;
        nearestNPC = null;
        currentNPC = null;
        currentNPCWalkAnimation = null;

        foreach (GameObject npc in npcs)
        {
            float distance = Vector2.Distance(transform.position, npc.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestNPC = npc.transform;
                currentNPC = npc.GetComponent<NPC>(); // 获取NPC组件
                currentNPCWalkAnimation = npc.GetComponent<WalkAnimation>(); // 获取WalkAnimation组件
            }
        }

        if (closestDistance <= interactionDistance)
        {
            if (!canInteract && !isFading && !isInDialogue)
            {
                canInteract = true;
                isFading = true;
                interactionText.gameObject.SetActive(true);
                bottomSprite.gameObject.SetActive(true);
                interactionText.text = "按 E 对话";
                FadeIn();
            }
        }
        else
        {
            if (canInteract && !isFading && !isInDialogue)
            {
                canInteract = false;
                isFading = true;
                FadeOut();
            }
        }
    }

    void FadeIn()
    {
        interactionText.CrossFadeAlpha(1f, fadeDuration, false);
        bottomSprite.CrossFadeAlpha(1f, fadeDuration, false);
        Invoke(nameof(ResetFade), fadeDuration);
    }

    void FadeOut()
    {
        interactionText.CrossFadeAlpha(0f, fadeDuration, false);
        bottomSprite.CrossFadeAlpha(0f, fadeDuration, false);
        Invoke(nameof(ResetFade), fadeDuration);
    }

    void ResetFade()
    {
        isFading = false;
        if (!canInteract)
        {
            interactionText.gameObject.SetActive(false);
            bottomSprite.gameObject.SetActive(false);
        }
    }

    void StartDialogue()
    {
        if (currentNPC == null || string.IsNullOrEmpty(currentNPC.role))
        {
            Debug.LogError("当前NPC未设置角色！");
            return;
        }

        isInDialogue = true;
        canInteract = false;
        interactionText.gameObject.SetActive(false);
        bottomSprite.gameObject.SetActive(false);
        dialoguePanel.SetActive(true);
        dialogueText.text = "正在思考...";
        ClearOptionButtons();

        // 停止NPC的移动和行走动画
        if (currentNPCWalkAnimation != null)
        {
            currentNPCWalkAnimation.SetWalkingState(false);
        }

        // 清空对话历史并设置新的系统提示
        conversationHistory.Clear();
        string systemPrompt = $"游戏背景：2111年3月16日，一枚导弹击中太阳，引发太阳黑子连锁反应，辐射风暴席卷地球，全球通信崩溃，互联网消失，人类失去阳光，只能躲避辐射，蜷缩室内，永远不能出门，植被疯长淹没城市废墟，FANLU-317 AI机器人成为唯一沟通桥梁，传递书信连接破碎世界，故事发生在“太阳战争”后30年的信火村，村民命运因信件交织，我是他们的信使。角色简介及关系：维克托·凯恩（老兵），退役军官，太阳战争亲历者，妻子辐射病去世因流弹击伤腿伤退役，沉默悲观极度厌恶科技，伊莱亚斯·凯恩的父亲，关系僵硬因儿子弃军从诗，小卢克·伍德亡父的上司间接牵连其孤儿命运，写信严肃简练如军令；伊莱亚斯·凯恩（诗人），年轻诗人反战者，维克托之子，热情洋溢追求艺术自由，母死后与父意见不合，简·怀特的恋人感性与理性碰撞是灵感源泉，但简呆板不解风情有时产生矛盾，写信诗意浓厚情感外露充满隐喻；简·怀特（工程师），理工学生擅长科技制造，冷静理性科技救世主义者，伊莱亚斯的女友受辐射限制无法常聚，萝丝的远房孙女战后受其指引来村，写信简洁理性附技术细节微露关怀，情商低；萝丝（老奶奶），普通老妇小卢克的神秘笔友，温柔神秘心怀悲悯与希望，简的远房祖母战后指引其逃难，小卢克的笔友以“神秘朋友”寄托因辐射病死去孙子的情感，写信温暖柔和充满故事与生活气息；小卢克·伍德（孩子），战争孤儿梦想成为科学家，天真乐观好奇心旺盛，伊芙·伍德的儿子其父死于维克托麾下，不知道笔友是谁受其信件启发成长，与母住在一块，写信童趣跳跃夸张好奇多问号；伊芙·伍德（画家），画家小卢克之母用艺术缅怀亡夫，感性执着坚韧不屈，小卢克的母亲丈夫死于战争与村人疏远唯独信件寄托情感，写信优美细腻色彩感强常附速写，我，机器信使FANLU，不太清楚村中人物关系，没有情感逻辑至上，冰冷客观。任务：你将扮演{currentNPC.role}与我（机器信使FANLU）对话，你无法外出，无法提出送信请求，回复需不超30字，符合场景，对话不要太文本化，要口语化极度贴合人设，不要提问，回复前加“【{currentNPC.role}】：”。你的每次回复后，必须生成两个选项，供 FANLU 继续对话。选项必须符合 FANLU 的身份，不能涉及送信，格式为“选项1：xxx\n选项2：xxx”，选项内容要符合我的人设，简洁明了。以上所有内容全部用中文回答";
        conversationHistory.Add(new Message { role = "system", content = systemPrompt });

        sendMessageCoroutine = StartCoroutine(SendMessageToQwen("你好"));
    }

    void EndDialogue()
    {
        isInDialogue = false;
        dialoguePanel.SetActive(false);
        ClearOptionButtons();
        if (sendMessageCoroutine != null)
        {
            StopCoroutine(sendMessageCoroutine);
            sendMessageCoroutine = null;
        }

        // 恢复NPC的移动和行走动画
        if (currentNPCWalkAnimation != null)
        {
            currentNPCWalkAnimation.SetWalkingState(true);
        }

        CheckForNearbyNPC();
    }

    void ClearOptionButtons()
    {
        foreach (var button in optionButtons)
        {
            Destroy(button.gameObject);
        }
        optionButtons.Clear();
    }

    void CreateOptionButtons(string[] options)
    {
        ClearOptionButtons();
        for (int i = 0; i < options.Length; i++)
        {
            string optionText = options[i];
            Button button = Instantiate(optionButtonPrefab, optionsContainer);
            Text buttonText = button.GetComponentInChildren<Text>();
            buttonText.text = optionText;
            buttonText.fontSize = 20;
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 40);
            int index = i;
            button.onClick.AddListener(() => OnOptionSelected(optionText));
            optionButtons.Add(button);
        }
    }

    void OnOptionSelected(string option)
    {
        dialogueText.text = "正在思考...";
        ClearOptionButtons();
        sendMessageCoroutine = StartCoroutine(SendMessageToQwen(option));
    }

    IEnumerator SendMessageToQwen(string message)
    {
        conversationHistory.Add(new Message { role = "user", content = message });
        var requestBody = new RequestBody { messages = conversationHistory.ToArray() };
        string jsonBody = JsonUtility.ToJson(requestBody);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST")
        {
            uploadHandler = new UploadHandlerRaw(bodyRaw),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string response = request.downloadHandler.text;
            QwenResponse jsonResponse = JsonUtility.FromJson<QwenResponse>(response);
            string aiResponse = jsonResponse.choices[0].message.content;

            // 解析回复和选项
            string[] parts = aiResponse.Split(new[] { "\n选项1：" }, System.StringSplitOptions.None);
            if (parts.Length < 2)
            {
                dialogueText.text = "错误：AI回复格式不正确\n" + aiResponse;
                yield break;
            }

            string dialogue = parts[0].Trim();
            string optionsPart = "选项1：" + parts[1];
            string[] optionLines = optionsPart.Split('\n');
            string[] options = new string[2];

            for (int i = 0; i < 2; i++)
            {
                string line = optionLines[i].Trim();
                options[i] = Regex.Replace(line, @"^选项\d+：", "").Trim();
            }

            dialogueText.text = dialogue;
            CreateOptionButtons(options);
            conversationHistory.Add(new Message { role = "assistant", content = aiResponse });
        }
        else
        {
            dialogueText.text = "错误: " + request.error;
            Debug.LogError("API 请求失败: " + request.error);
        }
    }
}