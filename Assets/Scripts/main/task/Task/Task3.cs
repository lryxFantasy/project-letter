using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[System.Serializable]
public class Task3 : TaskBase
{
    private bool letterDeliveredToJane = false;
    private string[] currentDialogue;
    private int dialogueIndex = 0;
    private TMP_Text dialogueText;
    private GameObject dialoguePanel;
    private Button nextButton;
    private TaskManager taskManager;

    // ����ʼ������
    private GameObject taskCompletePanel;
    private TextMeshProUGUI taskCompleteText;

    public void SetupTask(TaskManager manager, GameObject panel, TMP_Text text, Button button)
    {
        taskManager = manager;
        dialoguePanel = panel;
        dialogueText = text;
        nextButton = button;
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(NextDialogue);
        dialoguePanel.SetActive(false);

        // ��ʼ������ʼ��岢��ʾ
        SetupTaskCompletePanel();
        StartCoroutine(ShowTaskStartPanel());
    }

    public override string GetTaskName() => "���Ե�����";

    public override string GetTaskObjective() => $"�ʹ������˹�������������򡤻��ء����ţ�{(letterDeliveredToJane ? "�����" : "δ���")}";

    public override bool IsTaskComplete() => letterDeliveredToJane;

    public override void DeliverLetter(string targetResident)
    {
        dialogueIndex = 0;
        currentDialogue = targetResident == "�򡤻���" && !letterDeliveredToJane
            ? GetDialogueForJane()
            : new string[] { "���������㻹û����" };

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
            if (!letterDeliveredToJane && currentDialogue.Length > 1)
            {
                letterDeliveredToJane = true;
                if (taskManager != null && taskManager.inventoryManager != null)
                {
                    Debug.Log("Task3: �Ƴ�������˹����");
                    taskManager.inventoryManager.RemoveLetter("������˹����");
                }
                else
                {
                    Debug.LogError("Task3: TaskManager �� InventoryManager δ��ȷ��");
                }
            }

            PlayerController playerController = Object.FindObjectOfType<PlayerController>();
            if (playerController != null && playerController.IsInDialogue())
                playerController.EndDialogue();

            if (IsTaskComplete())
            {
                if (taskManager != null)
                {
                    Debug.Log("Task3: ������ɣ��л��� Task4");
                    Task4 newTask = gameObject.AddComponent<Task4>();
                    taskManager.SetTask(newTask);
                    newTask.SetupTask(taskManager, dialoguePanel, dialogueText, nextButton);
                    taskManager.UpdateTaskDisplay();
                }
                else
                {
                    Debug.LogError("Task3: TaskManager δ�ҵ����޷��л��� Task4");
                }
            }
        }
    }

    private string[] GetDialogueForJane()
    {
        return new string[]
        {
            "���򡤻��ء������ˣ����ҵ��ţ����ҿ�����",
            "��������",
            "���򡤻��ء�������˹�ģ�����д��Щ�������߰���Ĵʣ��ҵ���������ܿ�����",
            "���򡤻��ء���˵���˳���������ǿգ����Ǽǵã����ҵ�ʱֻ������ֵ��������˼������",
            "���򡤻��ء�������д����ôֱ�ӣ����桭����֪����ô��Ӧ�����Ӷ����ˡ�",
            "���򡤻��ء���̸����ʱҲ����������������·�ĵ�أ����е�����ϡ�",
            "���򡤻��ء���������֮ǰ���ҵ���������0��1���ɵģ�һ�����ﶼ���Ա���ʽ��",
            "���򡤻��ء������������ƻ����ҵ��������bug����Щ��е�ӿ������һʱ����Ӧ�ԡ�",
            "���򡤻��ء���ϲ�����ݣ�ϲ�����޺õĶ�������ȴ�ϸ��ҽ�ʫ��������������Щץ��ס�ġ���",
            "���򡤻��ء���ʵ��ʱ�����ܶ���������Щ�����������ݣ����ܻ���á������߰ɡ�",
            "���򡤻��ء���ɵ�ӣ���������ô�찡��",
            "���򡤻��ء��������ٻ�ȥ���������Ҹ���˵����˵�����ţ��Ҳ�̫��˵��Щ�����Ĵ��壬�����ܣ���ȷʵϲ��������",
            "���򡤻��ء��һ���д�Ÿ����ġ�",
            "���򡤻��ء�Ŷ���ˣ�������˿˵������������ţ����ڴ��Ӷ������Ե��С�����лл�㡣"
        };
    }

    // ��ʼ������������
    private void SetupTaskCompletePanel()
    {
        taskCompletePanel = GameObject.Find("TaskCompletePanel");
        if (taskCompletePanel != null)
        {
            taskCompleteText = taskCompletePanel.GetComponentInChildren<TextMeshProUGUI>();
            if (taskCompleteText == null)
            {
                Debug.LogWarning("TaskCompletePanel ��û���ҵ� TextMeshProUGUI �����");
            }
            else
            {
                CanvasGroup canvasGroup = taskCompletePanel.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = taskCompletePanel.AddComponent<CanvasGroup>();
                }
                canvasGroup.alpha = 0f;
            }
        }
        else
        {
            Debug.LogError("δ�ҵ� TaskCompletePanel����ȷ���������Ѵ��ڸ���壡");
        }
    }

    // ��ʾ����ʼ��ʾ
    private IEnumerator ShowTaskStartPanel()
    {
        if (taskCompletePanel != null && taskCompleteText != null)
        {
            taskCompleteText.text = "����3�������Ե�����";
            CanvasGroup canvasGroup = taskCompletePanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = taskCompletePanel.AddComponent<CanvasGroup>();
                canvasGroup.alpha = 0f;
            }

            // ����
            float fadeDuration = 1f;
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
                yield return null;
            }
            canvasGroup.alpha = 1f;

            // ��ʾ 2 ��
            yield return new WaitForSecondsRealtime(2f);

            // ����
            elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                yield return null;
            }
            canvasGroup.alpha = 0f;

            Debug.Log("����3 ��ʼ�������ʾ������");
        }
        else
        {
            Debug.LogWarning("������������������δ��ȷ��ʼ�����޷���ʾ����3��ʼ��ʾ��");
        }
    }
}