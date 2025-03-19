using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private Text interactionText; // UI�ı�����ʾ���� E �Ի�����
    [SerializeField] private Sprite bottomSpriteImage; // Sprite��Ƭ��Դ
    [SerializeField] private Vector3 offset = new Vector3(50f, 50f, 0f); // ��Ļ�ռ�ƫ��
    [SerializeField] private float fadeDuration = 0.1f; // ���뵭������ʱ��
    [SerializeField] private Text dialogueText; // �Ի���ʾ�ı�
    [SerializeField] private GameObject dialoguePanel; // �Ի����
    [SerializeField] private Button optionButtonPrefab; // ��ťԤ����
    [SerializeField] private Transform optionsContainer; // ��ť�ĸ������������У�
    private Transform nearestNPC;
    private NPC currentNPC; // ��ǰ������NPC
    private WalkAnimation currentNPCWalkAnimation; // ��ǰNPC��WalkAnimation���
    private bool canInteract = false;
    private bool isInDialogue = false;
    private Camera mainCamera;
    private Image bottomSprite;
    private bool isFading = false;
    private Coroutine sendMessageCoroutine; // �洢�������е�Э��

    // QwenChat���
    private string apiKey = "sk-e11794d89a4f492988f8b2b39a4ddf0a"; // ǧ�� API Key
    private string apiUrl = "https://dashscope.aliyuncs.com/compatible-mode/v1/chat/completions";
    private List<Message> conversationHistory = new List<Message>();
    private List<Button> optionButtons = new List<Button>(); // �洢��̬�����İ�ť

    [System.Serializable]
    private class RequestBody
    {
        public string model = "qwen-plus-2025-01-25";
        public Message[] messages;
        public float temperature = 1.2f; // ����¶Ȳ���������Ϊ1.2�����Ӷ�����
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

        // ��ʼ���Ի���ʷ�������գ�������ʾ��StartDialogue�����ã�
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

    // �ṩ���������������ű����Ի�״̬
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
                currentNPC = npc.GetComponent<NPC>(); // ��ȡNPC���
                currentNPCWalkAnimation = npc.GetComponent<WalkAnimation>(); // ��ȡWalkAnimation���
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
                interactionText.text = "�� E �Ի�";
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
            Debug.LogError("��ǰNPCδ���ý�ɫ��");
            return;
        }

        isInDialogue = true;
        canInteract = false;
        interactionText.gameObject.SetActive(false);
        bottomSprite.gameObject.SetActive(false);
        dialoguePanel.SetActive(true);
        dialogueText.text = "����˼��...";
        ClearOptionButtons();

        // ֹͣNPC���ƶ������߶���
        if (currentNPCWalkAnimation != null)
        {
            currentNPCWalkAnimation.SetWalkingState(false);
        }

        // ��նԻ���ʷ�������µ�ϵͳ��ʾ
        conversationHistory.Clear();
        string systemPrompt = $"��Ϸ������2111��3��16�գ�һö��������̫��������̫������������Ӧ������籩ϯ�����ȫ��ͨ�ű�������������ʧ������ʧȥ���⣬ֻ�ܶ�ܷ��䣬�������ڣ���Զ���ܳ��ţ�ֲ���賤��û���з��棬FANLU-317 AI�����˳�ΪΨһ��ͨ�������������������������磬���·����ڡ�̫��ս������30����Ż�壬�����������ż���֯���������ǵ���ʹ����ɫ��鼰��ϵ��ά���С��������ϱ��������۾��٣�̫��ս�������ߣ����ӷ��䲡ȥ�������������������ۣ���Ĭ���ۼ������Ƽ���������˹�������ĸ��ף���ϵ��Ӳ�����������ʫ��С¬�ˡ������������˾���ǣ����¶����ˣ�д�������������������˹��������ʫ�ˣ�������ʫ�˷�ս�ߣ�ά����֮�ӣ���������׷���������ɣ�ĸ�����븸������ϣ��򡤻��ص����˸�����������ײ�����ԴȪ��������岻�������ʱ����ì�ܣ�д��ʫ��Ũ�������¶�����������򡤻��أ�����ʦ������ѧ���ó��Ƽ����죬�侲���ԿƼ����������ߣ�������˹��Ů���ܷ��������޷����ۣ���˿��Զ����Ůս������ָ�����壬д�ż�����Ը�����ϸ��΢¶�ػ������̵ͣ���˿�������̣�����ͨ�ϸ�С¬�˵����ر��ѣ����������Ļ�������ϣ�������Զ����ĸս��ָ�������ѣ�С¬�˵ı����ԡ��������ѡ���������䲡��ȥ���ӵ���У�д����ů��ͳ���������������Ϣ��С¬�ˡ���£����ӣ���ս���¶������Ϊ��ѧ�ң������ֹۺ�������ʢ����ܽ����µĶ����丸����ά�������£���֪��������˭�����ż������ɳ�����ĸס��һ�飬д��ͯȤ��Ծ���ź�����ʺţ���ܽ����£����ң�������С¬��֮ĸ�������廳���򣬸���ִ�ż��Ͳ�����С¬�˵�ĸ���ɷ�����ս���������ԶΨ���ż�������У�д������ϸ��ɫ�ʸ�ǿ������д���ң�������ʹFANLU����̫������������ϵ��û������߼����ϣ�����͹ۡ������㽫����{currentNPC.role}���ң�������ʹFANLU���Ի������޷�������޷�����������󣬻ظ��費��30�֣����ϳ������Ի���Ҫ̫�ı�����Ҫ���ﻯ�����������裬��Ҫ���ʣ��ظ�ǰ�ӡ���{currentNPC.role}�����������ÿ�λظ��󣬱�����������ѡ��� FANLU �����Ի���ѡ�������� FANLU ����ݣ������漰���ţ���ʽΪ��ѡ��1��xxx\nѡ��2��xxx����ѡ������Ҫ�����ҵ����裬������ˡ�������������ȫ�������Ļش�";
        conversationHistory.Add(new Message { role = "system", content = systemPrompt });

        sendMessageCoroutine = StartCoroutine(SendMessageToQwen("���"));
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

        // �ָ�NPC���ƶ������߶���
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
        dialogueText.text = "����˼��...";
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

            // �����ظ���ѡ��
            string[] parts = aiResponse.Split(new[] { "\nѡ��1��" }, System.StringSplitOptions.None);
            if (parts.Length < 2)
            {
                dialogueText.text = "����AI�ظ���ʽ����ȷ\n" + aiResponse;
                yield break;
            }

            string dialogue = parts[0].Trim();
            string optionsPart = "ѡ��1��" + parts[1];
            string[] optionLines = optionsPart.Split('\n');
            string[] options = new string[2];

            for (int i = 0; i < 2; i++)
            {
                string line = optionLines[i].Trim();
                options[i] = Regex.Replace(line, @"^ѡ��\d+��", "").Trim();
            }

            dialogueText.text = dialogue;
            CreateOptionButtons(options);
            conversationHistory.Add(new Message { role = "assistant", content = aiResponse });
        }
        else
        {
            dialogueText.text = "����: " + request.error;
            Debug.LogError("API ����ʧ��: " + request.error);
        }
    }
}