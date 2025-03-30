using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using System.IO;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float interactionDistance = 2f; // �������루����NPC���ţ�
    [SerializeField] private TMP_Text interactionText; // ������ʾ�ı�
    [SerializeField] private Sprite bottomSpriteImage; // ������ʾ��ͼ
    [SerializeField] private Vector3 offset = new Vector3(50f, 50f, 0f); // UIƫ����
    [SerializeField] private float fadeDuration = 0.1f; // ���뵭��ʱ��
    [SerializeField] private TMP_Text dialogueText; // �Ի��ı�
    [SerializeField] private GameObject dialoguePanel; // �Ի����
    [SerializeField] private Button optionButtonPrefab; // ѡ�ťԤ����
    [SerializeField] private Transform optionsContainer; // ѡ������
    [SerializeField] private Slider favorabilitySlider; // �øжȽ�����
    [SerializeField] private TMP_Text favorabilityText; // �øж���ֵ�ı�

    [SerializeField] private CameraController cameraController; // ����������ƽű�
    private Transform nearestDoor;
    private int nearestDoorIndex = -1; // �ŵ�������0-5����Ӧ�������ݣ�
    private bool isNearExitDoor = false; // �Ƿ񿿽�������
    private bool canEnterHouse; // �Ƿ���Խ��뷿��

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
    private Dictionary<string, int> npcFavorability = new Dictionary<string, int>(); // NPC�øж�

    [System.Serializable] private class RequestBody { public string model = "qwen-plus"; public Message[] messages; public float temperature = 1.2f; }
    [System.Serializable] public class Message { public string role; public string content; }
    [System.Serializable] public class QwenResponse { public Choice[] choices; }
    [System.Serializable] public class Choice { public Message message; }

    void Start()
    {

        mainCamera = Camera.main;
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
        bottomSprite.transform.SetAsFirstSibling();

        // ��ʼ��������
        if (favorabilitySlider != null)
        {
            favorabilitySlider.minValue = -50;
            favorabilitySlider.maxValue = 50;
            favorabilitySlider.value = 0;
        }

        // ��ʼ���øж��ı�
        if (favorabilityText != null)
        {
            favorabilityText.text = "�øж�: 0";
        }
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
        CheckForNearbyDoor();

        if (PauseMenu.IsPaused && Input.GetKeyDown(KeyCode.E))
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isInDialogue)
            {
                EndDialogue();
            }
            else if (canInteract)
            {
                StartDialogue();
            }
            else if (canEnterHouse)
            {
                if (!cameraController.IsIndoors() && !isNearExitDoor)
                {
                    cameraController.EnterHouse(nearestDoorIndex);
                }
                else if (cameraController.IsIndoors() && isNearExitDoor)
                {
                    cameraController.ExitHouse();
                }
            }
        }
    }

    void LateUpdate()
    {
        if (interactionText.gameObject.activeSelf)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);
            Vector3 targetTextPos = screenPos + new Vector3(offset.x * 1.25f, offset.y * 1.2f, 0f);
            Vector3 targetSpritePos = screenPos + offset - new Vector3(0, -5f * Screen.width / 1280f, 0);

            interactionText.rectTransform.position = Vector3.Lerp(interactionText.rectTransform.position, targetTextPos, 0.1f);
            bottomSprite.rectTransform.position = Vector3.Lerp(bottomSprite.rectTransform.position, targetSpritePos, 0.1f);
        }
    }

    public bool IsInDialogue() => isInDialogue;

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
        if (inRange && !canInteract && !isFading && !isInDialogue && !canEnterHouse)
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

    void CheckForNearbyDoor()
    {
        float closestDistance = Mathf.Infinity;
        nearestDoor = null;
        nearestDoorIndex = -1;
        isNearExitDoor = false;

        foreach (GameObject door in GameObject.FindGameObjectsWithTag("Door"))
        {
            float distance = Vector2.Distance(transform.position, door.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestDoor = door.transform;
                string doorName = door.name;
                if (doorName.Contains("Door_"))
                {
                    string indexStr = doorName.Replace("Door_", "");
                    if (int.TryParse(indexStr, out int index))
                    {
                        nearestDoorIndex = index;
                    }
                }
                if (doorName.ToLower().Contains("exit"))
                {
                    isNearExitDoor = true;
                }
                else
                {
                    isNearExitDoor = false;
                }
            }
        }

        bool inRange = closestDistance <= interactionDistance;
        if (inRange && !canEnterHouse && !isFading && !isInDialogue && !canInteract)
        {
            canEnterHouse = true;
            isFading = true;
            interactionText.text = isNearExitDoor ? "�� E �뿪" : "�� E ����";
            interactionText.gameObject.SetActive(true);
            bottomSprite.gameObject.SetActive(true);
            FadeIn();
        }
        else if (!inRange && canEnterHouse && !isFading && !isInDialogue)
        {
            canEnterHouse = false;
            isFading = true;
            FadeOut();
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
        if (!canInteract && !canEnterHouse)
        {
            interactionText.gameObject.SetActive(false);
            bottomSprite.gameObject.SetActive(false);
        }
    }

    void StartDialogue()
    {
        if (currentNPC == null || string.IsNullOrEmpty(currentNPC.role)) return;
        Debug.Log($"PlayerController: ��ʼ�Ի���NPC��ɫ��{currentNPC.role}");
        isInDialogue = true;
        canInteract = false;
        interactionText.gameObject.SetActive(false);
        bottomSprite.gameObject.SetActive(false);
        dialoguePanel.SetActive(true);
        dialogueText.text = "����˼��...";
        ClearOptionButtons();
        if (currentNPCWalkAnimation != null) currentNPCWalkAnimation.SetWalkingState(false);
        conversationHistory.Clear();
        conversationHistory.Add(new Message
        {
            role = "system",
            content = $"��Ϸ������2111��3��16�գ�һö������������̫��������̫������������Ӧ������籩ϯ�����ȫ��ͨ�ű�������������ʧ������ʧȥ���⣬ֻ�ܶ�ܷ��䣬�������ڣ������ܳ��ţ�FANLU�����˳�Ϊ��ͨ���������������������磬���·����ڡ���ʾ¼ս������30����Ż�壬�����������ż���֯���������ǵ���ʹ����ɫ��鼰��ϵ��ά���С��������ϱ��������۾��٣���ʾ¼ս�������ߣ����ӷ��䲡ȥ���������������������ۣ���Ĭ���ۼ������Ƽ���������˹�������ĸ��ף���ϵ��Ӳ�����������ʫ��С¬�ˡ������������˾���ǣ����¶����ˣ���������������˹��������ʫ�ˣ�������ʫ�˷�ս�ߣ�ά����֮�ӣ���������׷�����ɣ�ĸ�����븸������ϣ��򡤻��ص����˸�����������ײ�����ԴȪ��������岻��������ì�ܣ�ʫ��Ũ�������¶���򡤻��أ�����ʦ���������ó��Ƽ����죬�侲���ԿƼ��������壬������˹��Ů���ܷ��������޷����ۣ���˿��Զ����Ůս������ָ�����壬���Ը�����ϸ��΢¶�ػ������̵ͣ���˿�������̣�����ͨ�ϸ�С¬�˵����ر��ѣ������Ļ�������ϣ�������Զ����ĸս������ָ�����ѣ���С¬�˵ġ��������ѡ�����������䲡��ȥ���ӵ���У�д����ů��ͳ�����ʱ��������������Ϣ��С¬�ˡ���£����ӣ���ս���¶������Ϊ��ѧ�ң������ֹۺ�������ʢ����ܽ����µĶ����丸����ά�������£�������������Ϊ�Ǹ��ף������ż������ɳ�����ĸס��һ�飬д��ͯȤ��Ծ���ź�����ʺţ���ܽ����£����ң�������С¬��֮ĸ�������廳���򣬸���ִ�ż��Ͳ�����С¬�˵�ĸ�ף��ɷ�����ս���������ԶΨ���ż�������У�д������ϸ��ɫ�ʸ�ǿ��С¬�˺���ܽס��һ�������ţ��ң�������ʹFANLU����̫������������ϵ������е��߼����ϣ����������ģ�ϲ�������ﾭ���������㽫����{currentNPC.role}���ң�������ʹFANLU���Ի���������Ծ����޷�������ظ��費��30�֣����ϳ������Ի����ﻯ�����������裬��Ҫ���ʣ��ظ�ǰ�ӡ���{currentNPC.role}��������ÿ�λظ�����������ѡ�һ������Ϊ�ң�FANLU���Ļ�Ӧ�������ҵ����裬ѡ����ֱ�ӻظ���ĶԻ���ѯ����Ĺ�ȥ���ɼӱ�㣬��ʽΪ��ѡ��1��xxx\nѡ��2��xxx����ȷ��ѡ����ȷΪFANLU�Ļش𡣴��⣬ÿ�λظ��ڶԻ�������øжȱ仯����������Ÿ�ʽֻ�У���+1����-1����+0�������ݶԻ������ж��ҵĻ�Ӧ�Ƿ���{currentNPC.role}�е�����򲻿죬�����϶����ӺøУ�����Į���ٺøУ���ʽΪ����{currentNPC.role}�����Ի����ݣ��øжȱ仯��������������ȫ�������ģ�����Ӣ�ġ�"
        });

        // ���ºøжȽ��������ı�
        UpdateFavorabilitySlider();

        sendMessageCoroutine = StartCoroutine(SendMessageToQwen("���"));
    }

    public void EndDialogue()
    {
        isInDialogue = false;
        dialoguePanel.SetActive(false);
        ClearOptionButtons();
        if (sendMessageCoroutine != null) { StopCoroutine(sendMessageCoroutine); sendMessageCoroutine = null; }
        if (currentNPCWalkAnimation != null) currentNPCWalkAnimation.SetWalkingState(true);
        CheckForNearbyNPC();
        CheckForNearbyDoor();
    }

    void ClearOptionButtons()
    {
        foreach (var button in optionButtons) Destroy(button.gameObject);
        optionButtons.Clear();
    }

    void CreateOptionButtons(string[] options)
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

    void OnOptionSelected(string option)
    {
        dialogueText.text = "����˼��...";
        ClearOptionButtons();
        sendMessageCoroutine = StartCoroutine(SendMessageToQwen(option));
    }

    IEnumerator SendMessageToQwen(string message)
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
            Debug.Log($"AIԭʼ�ظ�: {aiResponse}"); // ���AI�ظ�

            string[] parts = aiResponse.Split(new[] { "\nѡ��1��" }, System.StringSplitOptions.None);
            if (parts.Length < 2) { dialogueText.text = "����AI�ظ���ʽ����ȷ\n" + aiResponse; yield break; }

            string dialogueWithFavor = parts[0].Trim();
            int favorChange = 0;
            string dialogue = dialogueWithFavor;

            // ʹ��������ʽƥ��øжȱ仯������ȫ��/������źͿո�
            var favorMatch = Regex.Match(dialogueWithFavor, @"[\(��]\s*[+-]\d+\s*[\)��]");
            if (favorMatch.Success)
            {
                string favorText = favorMatch.Value;
                if (favorText.Contains("+1"))
                {
                    favorChange = 1;
                }
                else if (favorText.Contains("-1"))
                {
                    favorChange = -1;
                }
                else if (favorText.Contains("+0")) // ֧�� +0
                {
                    favorChange = 0; // �øжȲ���
                }
                // �Ƴ��øжȱ��
                dialogue = Regex.Replace(dialogueWithFavor, @"[\(��]\s*[+-]\d+\s*[\)��]", "").Trim();
            }

            // ���ºøж�
            if (currentNPC != null)
            {
                string npcRole = currentNPC.role;
                if (!npcFavorability.ContainsKey(npcRole)) npcFavorability[npcRole] = 0;
                npcFavorability[npcRole] += favorChange;
                npcFavorability[npcRole] = Mathf.Clamp(npcFavorability[npcRole], -50, 50); // ���ƺøжȷ�Χ
                currentNPC.favorability = npcFavorability[npcRole];
                Debug.Log($"{npcRole} �øж�: {npcFavorability[npcRole]}");

                // ���ºøжȽ��������ı�
                UpdateFavorabilitySlider();
            }

            string[] optionLines = ("ѡ��1��" + parts[1]).Split('\n');
            string[] options = new string[2];
            for (int i = 0; i < 2; i++) options[i] = Regex.Replace(optionLines[i].Trim(), @"^ѡ��\d+��", "");
            dialogueText.text = dialogue;
            CreateOptionButtons(options);
            conversationHistory.Add(new Message { role = "assistant", content = aiResponse });
        }
        else dialogueText.text = "����: " + request.error;
    }

    // ���ºøжȽ��������ı�
    private void UpdateFavorabilitySlider()
    {
        if (favorabilitySlider != null && currentNPC != null)
        {
            string npcRole = currentNPC.role;
            int favorability = npcFavorability.ContainsKey(npcRole) ? npcFavorability[npcRole] : 0;
            favorabilitySlider.value = favorability; // ���½�����

            // ���ºøж��ı�
            if (favorabilityText != null)
            {
                favorabilityText.text = $"�øж�: {favorability}";
            }


        }
    }

    public string GetCurrentNPCRole()
    {
        return currentNPC != null ? currentNPC.role : null;
    }

    // �ṩ�øж����ݸ� TaskManager
    public List<SerializableFavorability> GetFavorabilityData()
    {
        List<SerializableFavorability> favorabilityList = new List<SerializableFavorability>();
        foreach (var pair in npcFavorability)
        {
            favorabilityList.Add(new SerializableFavorability
            {
                npcRole = pair.Key,
                favorability = pair.Value
            });
        }
        return favorabilityList;
    }

    // ���غøж�����
    public void LoadFavorabilityData(List<SerializableFavorability> favorabilityList)
    {
        npcFavorability.Clear();
        if (favorabilityList != null)
        {
            foreach (var favor in favorabilityList)
            {
                npcFavorability[favor.npcRole] = favor.favorability;
            }
        }

        // ͬ���øжȵ� NPC ʵ��
        foreach (GameObject npc in GameObject.FindGameObjectsWithTag("NPC"))
        {
            NPC npcComponent = npc.GetComponent<NPC>();
            if (npcFavorability.ContainsKey(npcComponent.role))
            {
                npcComponent.favorability = npcFavorability[npcComponent.role];
            }
            else
            {
                npcComponent.favorability = 0; // δ�����NPC��ʼ��Ϊ0
                npcFavorability[npcComponent.role] = 0;
            }
        }

        // ���غ���½��������ı�
        UpdateFavorabilitySlider();
    }

    // ��� getter �����Է��� interactionText �� bottomSprite
    public TMP_Text GetInteractionText()
    {
        return interactionText;
    }

    public Image GetBottomSprite()
    {
        return bottomSprite;
    }
}