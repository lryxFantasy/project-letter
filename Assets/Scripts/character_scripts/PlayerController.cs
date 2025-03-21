using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float interactionDistance = 2f; // NPC��������
    [SerializeField] private TMP_Text interactionText; // ������ʾ�ı�
    [SerializeField] private Sprite bottomSpriteImage; // ������ʾ��ͼ
    [SerializeField] private Vector3 offset = new Vector3(50f, 50f, 0f); // UIƫ����
    [SerializeField] private float fadeDuration = 0.1f; // ���뵭��ʱ��
    [SerializeField] private TMP_Text dialogueText; // �Ի��ı�
    [SerializeField] private GameObject dialoguePanel; // �Ի����
    [SerializeField] private Button optionButtonPrefab; // ѡ�ťԤ����
    [SerializeField] private Transform optionsContainer; // ѡ������

    private Transform nearestNPC;
    private NPC currentNPC;
    private WalkAnimation currentNPCWalkAnimation;
    private bool canInteract, isInDialogue, isFading;
    private Camera mainCamera;
    private Image bottomSprite;
    private Coroutine sendMessageCoroutine;

    private const string apiKey = "sk-e11794d89a4f492988f8b2b39a4ddf0a"; // API��Կ
    private const string apiUrl = "https://dashscope.aliyuncs.com/compatible-mode/v1/chat/completions"; // API��ַ
    private List<Message> conversationHistory = new List<Message>();
    private List<Button> optionButtons = new List<Button>();

    [System.Serializable] private class RequestBody { public string model = "qwen-plus-2025-01-25"; public Message[] messages; public float temperature = 1.2f; }
    [System.Serializable] public class Message { public string role; public string content; }
    [System.Serializable] public class QwenResponse { public Choice[] choices; }
    [System.Serializable] public class Choice { public Message message; }

    void Start()
    {
        mainCamera = Camera.main;
        if (interactionText == null) Debug.LogError("interactionText δ��ֵ��");
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
        bottomSprite.transform.SetAsFirstSibling(); // Ĭ�Ϸŵ���ײ�
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

            // ƽ���ƶ� UI
            interactionText.rectTransform.position = Vector3.Lerp(interactionText.rectTransform.position, targetTextPos, 0.1f);
            bottomSprite.rectTransform.position = Vector3.Lerp(bottomSprite.rectTransform.position, targetSpritePos, 0.1f);
        }
    }

    public bool IsInDialogue() => isInDialogue; // ��ȡ�Ի�״̬

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
            interactionText.text = "�� E �Ի�";
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

    void FadeIn() // ���뽻����ʾ
    {
        interactionText.CrossFadeAlpha(1f, fadeDuration, false);
        bottomSprite.CrossFadeAlpha(1f, fadeDuration, false);
        Invoke(nameof(ResetFade), fadeDuration);
    }

    void FadeOut() // ����������ʾ
    {
        interactionText.CrossFadeAlpha(0f, fadeDuration, false);
        bottomSprite.CrossFadeAlpha(0f, fadeDuration, false);
        Invoke(nameof(ResetFade), fadeDuration);
    }

    void ResetFade() // ���õ��뵭��״̬
    {
        isFading = false;
        if (!canInteract)
        {
            interactionText.gameObject.SetActive(false);
            bottomSprite.gameObject.SetActive(false);
        }
    }

    void StartDialogue() // ��ʼ�Ի�
    {
        if (currentNPC == null || string.IsNullOrEmpty(currentNPC.role)) { Debug.LogError("��ǰNPCδ���ý�ɫ��"); return; }
        isInDialogue = true;
        canInteract = false;
        interactionText.gameObject.SetActive(false);
        bottomSprite.gameObject.SetActive(false);
        dialoguePanel.SetActive(true);
        dialogueText.text = "����˼��...";
        ClearOptionButtons();
        if (currentNPCWalkAnimation != null) currentNPCWalkAnimation.SetWalkingState(false);
        conversationHistory.Clear();
        conversationHistory.Add(new Message { role = "system", content = $"��Ϸ������2111��3��16�գ�һö��������̫��������̫������������Ӧ������籩ϯ�����ȫ��ͨ�ű�������������ʧ������ʧȥ���⣬ֻ�ܶ�ܷ��䣬�������ڣ���Զ���ܳ��ţ�ֲ���賤��û���з��棬FANLU-317 AI�����˳�ΪΨһ��ͨ�������������������������磬���·����ڡ�̫��ս������30����Ż�壬�����������ż���֯���������ǵ���ʹ����ɫ��鼰��ϵ��ά���С��������ϱ��������۾��٣�̫��ս�������ߣ����ӷ��䲡ȥ�������������������ۣ���Ĭ���ۼ������Ƽ���������˹�������ĸ��ף���ϵ��Ӳ�����������ʫ��С¬�ˡ������������˾���ǣ����¶����ˣ�д�������������������˹��������ʫ�ˣ�������ʫ�˷�ս�ߣ�ά����֮�ӣ���������׷���������ɣ�ĸ�����븸������ϣ��򡤻��ص����˸�����������ײ�����ԴȪ��������岻�������ʱ����ì�ܣ�д��ʫ��Ũ�������¶�����������򡤻��أ�����ʦ������ѧ���ó��Ƽ����죬�侲���ԿƼ����������ߣ�������˹��Ů���ܷ��������޷����ۣ���˿��Զ����Ůս������ָ�����壬д�ż�����Ը�����ϸ��΢¶�ػ������̵ͣ���˿�������̣�����ͨ�ϸ�С¬�˵����ر��ѣ����������Ļ�������ϣ�������Զ����ĸս������ָ�����ѣ�С¬�˵ı����ԡ��������ѡ���������䲡��ȥ���ӵ���У�д����ů��ͳ���������������Ϣ��С¬�ˡ���£����ӣ���ս���¶������Ϊ��ѧ�ң������ֹۺ�������ʢ����ܽ����µĶ����丸����ά�������£������ر��ѣ����Բ�֪��������˭�����ż������ɳ�����ĸס��һ�飬д��ͯȤ��Ծ���ź�����ʺţ���ܽ����£����ң�������С¬��֮ĸ�������廳���򣬸���ִ�ż��Ͳ�����С¬�˵�ĸ���ɷ�����ս���������ԶΨ���ż�������У�д������ϸ��ɫ�ʸ�ǿ������д��С¬�˺���ܽס��һ�������ţ��ң�������ʹFANLU����̫������������ϵ��ȱ������߼����ϣ�����͹ۡ������㽫����{currentNPC.role}���ң�������ʹFANLU���Ի������޷�������޷�����������󣬻ظ��費��30�֣����ϳ������Ի����ﻯ�����������裬��Ҫ���ʣ��ظ�ǰ�ӡ���{currentNPC.role}��������ÿ�λظ�����������ѡ�һ������Ϊ�ң�FANLU���Ļ�Ӧ�������ҵ����裬ѡ����ֱ�ӻظ���ĶԻ���ѯ����Ĺ�ȥ���ɼӱ�㣬��ʽΪ��ѡ��1��xxx\nѡ��2��xxx����ȷ��ѡ����ȷΪFANLU�Ļش���������ȫ�������ġ�" });
        sendMessageCoroutine = StartCoroutine(SendMessageToQwen("���"));
    }

    void EndDialogue() // �����Ի�
    {
        isInDialogue = false;
        dialoguePanel.SetActive(false);
        ClearOptionButtons();
        if (sendMessageCoroutine != null) { StopCoroutine(sendMessageCoroutine); sendMessageCoroutine = null; }
        if (currentNPCWalkAnimation != null) currentNPCWalkAnimation.SetWalkingState(true);
        CheckForNearbyNPC();
    }

    void ClearOptionButtons() // ���ѡ�ť
    {
        foreach (var button in optionButtons) Destroy(button.gameObject);
        optionButtons.Clear();
    }

    void CreateOptionButtons(string[] options) // ����ѡ�ť
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

    void OnOptionSelected(string option) // ѡ��ѡ��
    {
        dialogueText.text = "����˼��...";
        ClearOptionButtons();
        sendMessageCoroutine = StartCoroutine(SendMessageToQwen(option));
    }

    IEnumerator SendMessageToQwen(string message) // ������Ϣ��API
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
            string[] parts = aiResponse.Split(new[] { "\nѡ��1��" }, System.StringSplitOptions.None);
            if (parts.Length < 2) { dialogueText.text = "����AI�ظ���ʽ����ȷ\n" + aiResponse; yield break; }
            string dialogue = parts[0].Trim();
            string[] optionLines = ("ѡ��1��" + parts[1]).Split('\n');
            string[] options = new string[2];
            for (int i = 0; i < 2; i++) options[i] = Regex.Replace(optionLines[i].Trim(), @"^ѡ��\d+��", "");
            dialogueText.text = dialogue;
            CreateOptionButtons(options);
            conversationHistory.Add(new Message { role = "assistant", content = aiResponse });
        }
        else dialogueText.text = "����: " + request.error;
    }
}