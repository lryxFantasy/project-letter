using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[System.Serializable]
public class Task7 : TaskBase
{
    private bool taskCondition = false; // �滻Ϊ�����������
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

    public override string GetTaskName() => "�Ż��ഫ"; 

    public override string GetTaskObjective() => "���û��ʲôҪ�������ˣ���ȥ��ͼ�ϱ������ķ������ӿ�����"; 

    public override bool IsTaskComplete() => taskCondition;

    public override void DeliverLetter(string targetResident)
    {
        dialogueIndex = 0;
        currentDialogue = targetResident == "Ŀ��NPC" && !taskCondition // �滻Ϊ����NPC������
            ? GetDialogueForTarget()
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
            if (!taskCondition && currentDialogue.Length > 1)
            {
                taskCondition = true;
                if (taskManager != null && taskManager.inventoryManager != null)
                {
                    Debug.Log("Task7: ���±��� - �Ƴ�ǰ���ż���������ż�");
                    taskManager.inventoryManager.RemoveLetter("ǰ���ż�����"); // �滻Ϊ�����ż�
                    taskManager.inventoryManager.AddLetter(new Letter
                    {
                        title = "���ż�����", // �滻Ϊ�������
                        content = "���ż�����" // �滻Ϊ��������
                    });
                }
                else
                {
                    Debug.LogError("Task7: TaskManager �� InventoryManager δ��ȷ��");
                }
            }

            PlayerController playerController = Object.FindObjectOfType<PlayerController>();
            if (playerController != null && playerController.IsInDialogue())
                playerController.EndDialogue();

            if (IsTaskComplete())
            {
                if (taskManager != null)
                {
                    Debug.Log("Task7: �������");
                    // ����� Task8�����������ת�߼�
                    // Task8 newTask = gameObject.AddComponent<Task8>();
                    // taskManager.SetTask(newTask);
                    // newTask.SetupTask(taskManager, dialoguePanel, dialogueText, nextButton);
                    taskManager.UpdateTaskDisplay();
                }
                else
                {
                    Debug.LogError("Task7: TaskManager δ�ҵ�");
                }
            }
        }
    }

    private string[] GetDialogueForTarget()
    {
        return new string[]
        {
            "��Ŀ��NPC���Ի�1", // �滻Ϊ����Ի�
            "��Ŀ��NPC���Ի�2"
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
            taskCompleteText.text = "����7�����Ż��ഫ"; // �滻Ϊ��������
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

            Debug.Log("����7 ��ʼ�������ʾ������");
        }
        else
        {
            Debug.LogWarning("������������������δ��ȷ��ʼ�����޷���ʾ����7��ʼ��ʾ��");
        }
    }
}