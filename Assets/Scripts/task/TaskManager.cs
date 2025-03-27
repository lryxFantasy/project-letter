using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class TaskManager : MonoBehaviour
{
    // UI ����ֶ�
    public GameObject taskPanel, taskMask, dialoguePanel, normalDialoguePanel;
    public Button taskButton, closeButton, nextButton, deliverButton, saveButton, loadButton;
    public TMP_Text taskTitle, taskObjective, dialogueText;

    // �����������
    public PlayerController playerController;
    public InventoryManager inventoryManager;
    public CameraController cameraController;
    private RubyController rubyController;

    // ״̬����
    private bool isPanelOpen;
    private float previousTimeScale;
    public TaskBase currentTask;
    private string currentNPCName;
    private string savePath;

    void Start()
    {
        // ��ʼ�� UI
        taskPanel.SetActive(false);
        taskMask.SetActive(false);
        dialoguePanel.SetActive(false);

        // �󶨰�ť�¼�
        taskButton.onClick.AddListener(ToggleTaskPanel);
        closeButton.onClick.AddListener(ToggleTaskPanel);
        InitializeDeliverButton();
        saveButton.onClick.AddListener(SaveGame);
        loadButton.onClick.AddListener(LoadGame);

        // ������ʼ����
        currentTask = gameObject.AddComponent<Task0>();
        (currentTask as Task0)?.SetupDialogueUI(dialoguePanel, dialogueText, nextButton);
        (currentTask as Task0)?.StartTaskDialogue();
        UpdateTaskDisplay();

        previousTimeScale = Time.timeScale;
        savePath = Path.Combine(Application.persistentDataPath, "saveData.json");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) ToggleTaskPanel();

        // ���¶Ի�״̬
        currentNPCName = playerController.IsInDialogue() ? playerController.GetCurrentNPCRole() : null;
        if (!string.IsNullOrEmpty(currentNPCName)) Debug.Log($"��ǰNPC: {currentNPCName}");
    }

    // �л��������
    public void ToggleTaskPanel()
    {
        isPanelOpen = !isPanelOpen;
        Time.timeScale = isPanelOpen ? 0f : previousTimeScale;
        taskPanel.SetActive(isPanelOpen);
        taskMask.SetActive(isPanelOpen);
        dialoguePanel.SetActive(!isPanelOpen && dialoguePanel.activeSelf);
    }

    // ��ʼ�����Ű�ť
    private void InitializeDeliverButton()
    {
        if (deliverButton == null) Debug.LogError("deliverButton δ��");
        else deliverButton.onClick.AddListener(TriggerDeliverLetter);
    }

    // ���������߼�
    public void TriggerDeliverLetter()
    {
        if (currentTask == null || string.IsNullOrEmpty(currentNPCName))
        {
            Debug.LogWarning("����ʧ��: �����NPCδ����");
            return;
        }
        if (normalDialoguePanel != null) normalDialoguePanel.SetActive(false);
        currentTask.DeliverLetter(currentNPCName);
    }

    // ����������ʾ
    public void UpdateTaskDisplay()
    {
        if (currentTask != null)
        {
            taskTitle.text = currentTask.GetTaskName();
            taskObjective.text = currentTask.GetTaskObjective();
        }
    }

    // ����������
    public void SetTask(TaskBase newTask)
    {
        if (currentTask != null) Destroy(currentTask);
        currentTask = newTask;
        UpdateTaskDisplay();
        Sprite icon = Resources.Load<Sprite>("jane"); // �� Resources ����ͼ��

        if (inventoryManager != null && !(newTask is Task0) && newTask is Task1)
            inventoryManager.AddLetter(new Letter 
            { 
                title = "���ά���е���", 
                content = "ά���У����޺���һ̨FANLU-317�������ͺţ��ӷ�������������ƴ�����ģ������е㲻�ȶ����������������á��Ҵ����������Ż�����ţ�����̫ǿ����û�������ţ��������ֲ�����������ĿǰΨһ�İ취����֮ǰ������������˹д�ţ��еĻ��ͽ������ɣ���ʱ�������������͹�ȥ����֪����������Щ�Ƽ���������������ǻ���һ�У����ⶫ�����ٲ��ᱻ�����ջ����ܰ��ϵ�æ���һ����о���Я�͹���֧�ܣ��������������˻�����͹�ȥ���б����Ҫ��д�����������ͻ���������ҿ�����������" ,
                icon = icon
            });
    }

    // ������Ϸ״̬
    public void SaveGame()
    {
        SaveData data = new SaveData
        {
            taskNumber = GetTaskNumber(),
            playerPosition = playerController.transform.position,
            letters = inventoryManager.letters,
            taskStateJson = JsonUtility.ToJson(currentTask),
            isIndoors = cameraController?.IsIndoors() ?? false,
            currentHouseIndex = cameraController?.currentHouseIndex ?? -1,
            lastPlayerMapPosition = cameraController?.lastPlayerMapPosition ?? Vector3.zero,
            npcFavorabilityList = playerController.GetFavorabilityData() // ��ȡ�øж�����
        };

        File.WriteAllText(savePath, JsonUtility.ToJson(data));
        Debug.Log("�ѱ��浽: " + savePath);
    }

    // ������Ϸ״̬
    public void LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("�浵�ļ�������");
            return;
        }

        SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(savePath));
        SetTaskFromNumber(data.taskNumber);
        JsonUtility.FromJsonOverwrite(data.taskStateJson, currentTask);

        // ���³�ʼ������
        if (currentTask is Task0 t0) t0.SetupDialogueUI(dialoguePanel, dialogueText, nextButton);
        else if (currentTask is Task1 t1)
        {
            t1.SetupDialogueUI(dialoguePanel, dialogueText, nextButton);
            t1.SetupDeliverButton(normalDialoguePanel, deliverButton);
        }
        else if (currentTask is Task2 t2) t2.SetupTask(this, dialoguePanel, dialogueText, nextButton);
        else if (currentTask is Task3 t3) t3.SetupTask(this, dialoguePanel, dialogueText, nextButton);
        else if (currentTask is Task4 t4) t4.SetupTask(this, dialoguePanel, dialogueText, nextButton);
        else if (currentTask is Task5 t5) t5.SetupTask(this, dialoguePanel, dialogueText, nextButton);
        else if (currentTask is Task6 t6) t6.SetupTask(this, dialoguePanel, dialogueText, nextButton);
        else if (currentTask is Task7 t7) t7.SetupTask(this, dialoguePanel, dialogueText, nextButton);

        // �ָ���Һ����
        if (cameraController != null)
        {
            playerController.transform.position = data.playerPosition;
            cameraController.isIndoors = data.isIndoors;
            cameraController.currentHouseIndex = data.currentHouseIndex;
            cameraController.lastPlayerMapPosition = data.lastPlayerMapPosition;
            cameraController.transform.position = data.isIndoors
                ? cameraController.housePositions[data.currentHouseIndex]
                : new Vector3(data.playerPosition.x, data.playerPosition.y, cameraController.transform.position.z) + cameraController.offset;
        }

        // ���غøж�����
        playerController.LoadFavorabilityData(data.npcFavorabilityList);

        inventoryManager.letters = data.letters;
        inventoryManager.UpdateInventoryUI();
        InitializeDeliverButton();
        UpdateTaskDisplay();

        rubyController = FindObjectOfType<RubyController>(); // ��ȡ RubyController
        rubyController.pauseHealthUpdate = false; // �ָ�Ѫ������

        Debug.Log("��Ϸ�Ѽ���");
    }

    // ��ȡ������
    private int GetTaskNumber() => currentTask switch
    {
        Task0 => 0,
        Task1 => 1,
        Task2 => 2,
        Task3 => 3,
        Task4 => 4,
        Task5 => 5,
        Task6 => 6,
        Task7 => 7,
        _ => -1
    };

    // ���ݱ����������
    private void SetTaskFromNumber(int taskNumber)
    {
        if (currentTask != null) Destroy(currentTask);
        currentTask = taskNumber switch
        {
            0 => gameObject.AddComponent<Task0>(),
            1 => gameObject.AddComponent<Task1>(),
            2 => gameObject.AddComponent<Task2>(),
            3 => gameObject.AddComponent<Task3>(),
            4 => gameObject.AddComponent<Task4>(),
            5 => gameObject.AddComponent<Task5>(),
            6 => gameObject.AddComponent<Task6>(),
            7 => gameObject.AddComponent<Task7>(),
            _ => null
        };
        if (currentTask == null) Debug.LogError("δ֪������: " + taskNumber);
    }
}