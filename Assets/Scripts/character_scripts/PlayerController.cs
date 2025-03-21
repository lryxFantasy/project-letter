using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float interactionDistance = 2f; // NPC交互距离
    [SerializeField] private TMP_Text interactionText; // 交互提示文本
    [SerializeField] private Sprite bottomSpriteImage; // 交互提示底图
    [SerializeField] private Vector3 offset = new Vector3(50f, 50f, 0f); // UI偏移量
    [SerializeField] private float fadeDuration = 0.1f; // 淡入淡出时间
    [SerializeField] private TMP_Text dialogueText; // 对话文本
    [SerializeField] private GameObject dialoguePanel; // 对话面板
    [SerializeField] private Button optionButtonPrefab; // 选项按钮预制体
    [SerializeField] private Transform optionsContainer; // 选项容器

    private Transform nearestNPC;
    private NPC currentNPC;
    private WalkAnimation currentNPCWalkAnimation;
    private bool canInteract, isInDialogue, isFading;
    private Camera mainCamera;
    private Image bottomSprite;
    private Coroutine sendMessageCoroutine;

    private const string apiKey = "sk-e11794d89a4f492988f8b2b39a4ddf0a"; // API密钥
    private const string apiUrl = "https://dashscope.aliyuncs.com/compatible-mode/v1/chat/completions"; // API地址
    private List<Message> conversationHistory = new List<Message>();
    private List<Button> optionButtons = new List<Button>();

    [System.Serializable] private class RequestBody { public string model = "qwen-plus-2025-01-25"; public Message[] messages; public float temperature = 1.2f; }
    [System.Serializable] public class Message { public string role; public string content; }
    [System.Serializable] public class QwenResponse { public Choice[] choices; }
    [System.Serializable] public class Choice { public Message message; }

    void Start()
    {
        mainCamera = Camera.main;
        if (interactionText == null) Debug.LogError("interactionText 未赋值！");
        interactionText.gameObject.SetActive(false);
        dialoguePanel.SetActive(false);
        bottomSprite = new GameObject("BottomSprite", typeof(Image)).GetComponent<Image>();
        bottomSprite.transform.SetParent(interactionText.transform.parent);
        bottomSprite.sprite = bottomSpriteImage;
        AdjustUISizeAndPosition();
        interactionText.alpha = 0f;
        bottomSprite.canvasRenderer.SetAlpha(0f);
        interactionText.gameObject.SetActive(true);
        interactionText.alpha = 1f;
        bottomSprite.transform.SetAsFirstSibling(); // 默认放到最底层
    }

    void AdjustUISizeAndPosition()
    {
        float scaleFactor = Screen.width / 1280f;
        bottomSprite.rectTransform.sizeDelta = new Vector2(95f * scaleFactor, 55f * scaleFactor);
        offset = new Vector3(50f * scaleFactor, 30f * scaleFactor, 0f);
        bottomSprite.rectTransform.anchoredPosition = new Vector2(0, -20f * scaleFactor);
    }

    void Update()
    {
        CheckForNearbyNPC();
        if (Input.GetKeyDown(KeyCode.E))
            if (!isInDialogue && canInteract) StartDialogue();
            else if (isInDialogue) EndDialogue();
    }

    void LateUpdate()
    {
        if (interactionText.gameObject.activeSelf)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);
            Vector3 targetTextPos = screenPos + new Vector3(offset.x * 1.25f, offset.y * 1.2f, 0f);
            Vector3 targetSpritePos = screenPos + offset - new Vector3(0, -5f * Screen.width / 1280f, 0);

            // 平滑移动 UI
            interactionText.rectTransform.position = Vector3.Lerp(interactionText.rectTransform.position, targetTextPos, 0.1f);
            bottomSprite.rectTransform.position = Vector3.Lerp(bottomSprite.rectTransform.position, targetSpritePos, 0.1f);
        }
    }

    public bool IsInDialogue() => isInDialogue; // 获取对话状态

    void CheckForNearbyNPC()
    {
        float closestDistance = Mathf.Infinity;
        foreach (GameObject npc in GameObject.FindGameObjectsWithTag("NPC"))
        {
            float distance = Vector2.Distance(transform.position, npc.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestNPC = npc.transform;
                currentNPC = npc.GetComponent<NPC>();
                currentNPCWalkAnimation = npc.GetComponent<WalkAnimation>();
            }
        }
        bool inRange = closestDistance <= interactionDistance;
        if (inRange && !canInteract && !isFading && !isInDialogue)
        {
            canInteract = true;
            isFading = true;
            interactionText.text = "按 E 对话";
            interactionText.gameObject.SetActive(true);
            bottomSprite.gameObject.SetActive(true);
            FadeIn();
        }
        else if (!inRange && canInteract && !isFading && !isInDialogue)
        {
            canInteract = false;
            isFading = true;
            FadeOut();
        }
    }

    void FadeIn() // 淡入交互提示
    {
        interactionText.CrossFadeAlpha(1f, fadeDuration, false);
        bottomSprite.CrossFadeAlpha(1f, fadeDuration, false);
        Invoke(nameof(ResetFade), fadeDuration);
    }

    void FadeOut() // 淡出交互提示
    {
        interactionText.CrossFadeAlpha(0f, fadeDuration, false);
        bottomSprite.CrossFadeAlpha(0f, fadeDuration, false);
        Invoke(nameof(ResetFade), fadeDuration);
    }

    void ResetFade() // 重置淡入淡出状态
    {
        isFading = false;
        if (!canInteract)
        {
            interactionText.gameObject.SetActive(false);
            bottomSprite.gameObject.SetActive(false);
        }
    }

    void StartDialogue() // 开始对话
    {
        if (currentNPC == null || string.IsNullOrEmpty(currentNPC.role)) { Debug.LogError("当前NPC未设置角色！"); return; }
        isInDialogue = true;
        canInteract = false;
        interactionText.gameObject.SetActive(false);
        bottomSprite.gameObject.SetActive(false);
        dialoguePanel.SetActive(true);
        dialogueText.text = "正在思考...";
        ClearOptionButtons();
        if (currentNPCWalkAnimation != null) currentNPCWalkAnimation.SetWalkingState(false);
        conversationHistory.Clear();
        conversationHistory.Add(new Message { role = "system", content = $"游戏背景：2111年3月16日，一枚导弹击中太阳，引发太阳黑子连锁反应，辐射风暴席卷地球，全球通信崩溃，互联网消失，人类失去阳光，只能躲避辐射，蜷缩室内，永远不能出门，植被疯长淹没城市废墟，FANLU-317 AI机器人成为唯一沟通桥梁，传递书信连接破碎世界，故事发生在“太阳战争”后30年的信火村，村民命运因信件交织，我是他们的信使。角色简介及关系：维克托·凯恩（老兵），退役军官，太阳战争亲历者，妻子辐射病去世因流弹击伤腿伤退役，沉默悲观极度厌恶科技，伊莱亚斯·凯恩的父亲，关系僵硬因儿子弃军从诗，小卢克·伍德亡父的上司间接牵连其孤儿命运，写信严肃简练如军令；伊莱亚斯·凯恩（诗人），年轻诗人反战者，维克托之子，热情洋溢追求艺术自由，母死后与父意见不合，简·怀特的恋人感性与理性碰撞是灵感源泉，但简呆板不解风情有时产生矛盾，写信诗意浓厚情感外露充满隐喻；简·怀特（工程师），理工学生擅长科技制造，冷静理性科技救世主义者，伊莱亚斯的女友受辐射限制无法常聚，萝丝的远房孙女战后受其指引来村，写信简洁理性附技术细节微露关怀，情商低；萝丝（老奶奶），普通老妇小卢克的神秘笔友，温柔神秘心怀悲悯与希望，简的远房祖母战后受其指引逃难，小卢克的笔友以“神秘朋友”寄托因辐射病死去孙子的情感，写信温暖柔和充满故事与生活气息；小卢克·伍德（孩子），战争孤儿梦想成为科学家，天真乐观好奇心旺盛，伊芙·伍德的儿子其父死于维克托麾下，有神秘笔友，绝对不知道笔友是谁受其信件启发成长，与母住在一块，写信童趣跳跃夸张好奇多问号；伊芙·伍德（画家），画家小卢克之母用艺术缅怀亡夫，感性执着坚韧不屈，小卢克的母亲丈夫死于战争与村人疏远唯独信件寄托情感，写信优美细腻色彩感强常附速写，小卢克和伊芙住在一起不用送信，我，机器信使FANLU，不太清楚村中人物关系，缺乏情感逻辑至上，冰冷客观。任务：你将扮演{currentNPC.role}与我（机器信使FANLU）对话，你无法外出，无法提出送信请求，回复需不超30字，符合场景，对话口语化极度贴合人设，不要提问，回复前加“【{currentNPC.role}】：”。每次回复后，生成两个选项，一定是作为我（FANLU）的回应，符合我的人设，选项需直接回复你的对话或询问你的过去，可加标点，格式为“选项1：xxx\n选项2：xxx”，确保选项明确为FANLU的回答。以上内容全部用中文。" });
        sendMessageCoroutine = StartCoroutine(SendMessageToQwen("你好"));
    }

    void EndDialogue() // 结束对话
    {
        isInDialogue = false;
        dialoguePanel.SetActive(false);
        ClearOptionButtons();
        if (sendMessageCoroutine != null) { StopCoroutine(sendMessageCoroutine); sendMessageCoroutine = null; }
        if (currentNPCWalkAnimation != null) currentNPCWalkAnimation.SetWalkingState(true);
        CheckForNearbyNPC();
    }

    void ClearOptionButtons() // 清除选项按钮
    {
        foreach (var button in optionButtons) Destroy(button.gameObject);
        optionButtons.Clear();
    }

    void CreateOptionButtons(string[] options) // 创建选项按钮
    {
        ClearOptionButtons();
        foreach (string option in options)
        {
            Button button = Instantiate(optionButtonPrefab, optionsContainer);
            button.GetComponentInChildren<TMP_Text>().text = option;
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 40);
            button.onClick.AddListener(() => OnOptionSelected(option));
            optionButtons.Add(button);
        }
    }

    void OnOptionSelected(string option) // 选择选项
    {
        dialogueText.text = "正在思考...";
        ClearOptionButtons();
        sendMessageCoroutine = StartCoroutine(SendMessageToQwen(option));
    }

    IEnumerator SendMessageToQwen(string message) // 发送消息到API
    {
        conversationHistory.Add(new Message { role = "user", content = message });
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST")
        {
            uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(new RequestBody { messages = conversationHistory.ToArray() }))),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            QwenResponse response = JsonUtility.FromJson<QwenResponse>(request.downloadHandler.text);
            string aiResponse = response.choices[0].message.content;
            string[] parts = aiResponse.Split(new[] { "\n选项1：" }, System.StringSplitOptions.None);
            if (parts.Length < 2) { dialogueText.text = "错误：AI回复格式不正确\n" + aiResponse; yield break; }
            string dialogue = parts[0].Trim();
            string[] optionLines = ("选项1：" + parts[1]).Split('\n');
            string[] options = new string[2];
            for (int i = 0; i < 2; i++) options[i] = Regex.Replace(optionLines[i].Trim(), @"^选项\d+：", "");
            dialogueText.text = dialogue;
            CreateOptionButtons(options);
            conversationHistory.Add(new Message { role = "assistant", content = aiResponse });
        }
        else dialogueText.text = "错误: " + request.error;
    }
}