using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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

    public override string GetTaskName() => "�����еĳ���";

    public override string GetTaskObjective() => $"�ʹ��ܽ����¡�����ά���С����������ţ�{(letterDeliveredToVictor ? "�����" : "δ���")}";

    public override bool IsTaskComplete() => letterDeliveredToVictor;

    public override void DeliverLetter(string targetResident)
    {
        dialogueIndex = 0;
        currentDialogue = targetResident == "ά���С�����" && !letterDeliveredToVictor
            ? GetDialogueForVictor()
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
            if (!letterDeliveredToVictor && currentDialogue.Length > 1)
            {
                letterDeliveredToVictor = true;
                if (taskManager != null && taskManager.inventoryManager != null)
                {
                    Debug.Log("Task6: �Ƴ���ܽ���ţ�������ط��ӵ�Կ��");
                    Sprite icon = Resources.Load<Sprite>("key"); // �� Resources ����ͼ��
                    taskManager.inventoryManager.RemoveLetter("��ܽ����");
                    taskManager.inventoryManager.AddLetter(new Letter
                    {
                        title = "�������ݵ�Կ��",
                        content = "�ϱ��ڼ��﷭������Կ�ף��ƺ����Դ򿪴����ϱ��������ӣ�������Ѿ��������������������飬��ÿ���˶��øжȴ���10��Ҳ����Կ���������Щʲô��",
                        icon = icon
                    });
                }
                else
                {
                    Debug.LogError("Task6: TaskManager �� InventoryManager δ��ȷ��");
                }
            }

            PlayerController playerController = Object.FindObjectOfType<PlayerController>();
            if (playerController != null && playerController.IsInDialogue())
                playerController.EndDialogue();

            if (IsTaskComplete())
            {
                if (taskManager != null)
                {
                    Debug.Log("Task6: ������ɣ��л��� Task7");
                    Task7 newTask = gameObject.AddComponent<Task7>();
                    taskManager.SetTask(newTask);
                    newTask.SetupTask(taskManager, dialoguePanel, dialogueText, nextButton);
                    taskManager.UpdateTaskDisplay();
                }
                else
                {
                    Debug.LogError("Task6: TaskManager δ�ҵ����޷��л��� Task7");
                }
            }
        }
    }

    private string[] GetDialogueForVictor()
    {
        return new string[]
        {
            "��ά���С����������㰡����Ƥ��ͷ���������������ˣ�",
            "��ά���С������������ɣ������Ҷ��ӵ��£�����лл���ˡ������أ��ø��ҡ�",
            "��������",
            "��ά���С�������������ܽ��Ů�ˡ�����˵�������ҡ����ҶԲ�������һ�ҡ���",
            "��ά���С�������С¬�˵ĸ��ף�¬�ˣ�����������ǰ����û��ס������",
            "��ά���С�����������ս���Ϻܻ��ң����������ˣ��о�����ָ�Ӳ���",
            "��ά���С��������ӵ����Ҷ��ߺ�Х���ӹ�������������ζ�̱ǵ�������Ϣ��������Ѫ����",
            "��ά���С��������Һ���ſ�£�����û�������������������ؿڣ�Ѫ������һ����",
            "��ά���С�������������������������ȥʱ�����Ѿ�ֻʣһ�����ˣ���������ס�ң���һֻ���ﻹ߬��ǹ��",
            "��ά���С�����������˵Щʲô�����Ѿ�û���ˣ�����֪�������ں�����������",
            "��ά���С���������û��������û�������Ż�ȥ�����Ķ��ӡ�",
            "��ά���С�����������������ʱ��ϵģ��۵���˯���ţ����һ��Ż����ˣ���û�ء�",
            "��ά���С���������ܽ˵�������ң����ҹ������Լ���ء���ս���������Ұ���ܽ���ǽӵ����Ż����ѡ�",
            "��ά���С�������ÿ�ο���С¬���Ǻ��ӣ��Ҿ�����¬�ˣ�������ÿ���ᵽ�������ϵ�Ц�ݡ���",
            "��ά���С���������Ƿ���ǵģ�̫���ˡ�",
            "��ά���С�������Ҳ����ʱ���ͻ��ˣ��һ�����ǻ��ŵģ�����Ѽ������¬��д���������ܡ���������Ҫ��ʱ��ɡ���",
            "��ά���С�������������ܽ�һ���ŵġ����Ҹ���˵����������̫�ۣ�С¬�˻��ÿ�����",
            "��ά���С�������Ŷ���ˣ��������ڼ��﷭������Կ�ף��ƺ����Դ򿪴����ϱ��������ӣ���˵��սʱ���µġ�",
            "��ά���С�����������û��������ˣ�Ҳ������Կ���������Щʲô��"
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
            taskCompleteText.text = "����6���������еĳ���";
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

            Debug.Log("����6 ��ʼ�������ʾ������");
        }
        else
        {
            Debug.LogWarning("������������������δ��ȷ��ʼ�����޷���ʾ����6��ʼ��ʾ��");
        }
    }
}