using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[System.Serializable]
public class Task5 : TaskBase
{
    private bool letterDeliveredToEve = false;
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

    public override string GetTaskName() => "���еĹ�";

    public override string GetTaskObjective() => $"�ʹС¬�ˡ���¡�������ܽ����¡����ţ�{(letterDeliveredToEve ? "�����" : "δ���")}";

    public override bool IsTaskComplete() => letterDeliveredToEve;

    public override void DeliverLetter(string targetResident)
    {
        dialogueIndex = 0;
        currentDialogue = targetResident == "��ܽ�����" && !letterDeliveredToEve
            ? GetDialogueForEve()
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
            if (!letterDeliveredToEve && currentDialogue.Length > 1)
            {
                letterDeliveredToEve = true;
                if (taskManager != null && taskManager.inventoryManager != null)
                {
                    Debug.Log("Task5: �Ƴ�С¬�˵��ţ������ܽ����");
                    taskManager.inventoryManager.RemoveLetter("С¬�˵���");
                    taskManager.inventoryManager.AddLetter(new Letter
                    {
                        title = "��ܽ����",
                        content = "ά�����������þ�û����˵���ˣ�Ҳ��֪���������ô������˵��Ū��֧�ܣ�����Ȼ������Ҽǵ���˵����������ĥ�㣬���������ˣ���ö�ע�⣬��Ӳ���š�����������˹����㣬��˵�㻹�������ӣ���Ӳ����ʯͷ������дʫ������ܵ����ˣ��ǲ�������ү����ϵ�õ��ˣ���ͦ�������˵ģ�����˵�㲻������ʫ�����ҿ��ó��������ں���ġ�\r\n\r\n���С¬�����������ְ֡��������ˣ��������أ��ϲ������������ģ�����֪�����Ĺ��¡���֪��������ս���ϣ�����Ĳ��£������������������Ҵ�û�ֹ��㣬ս������������أ����ܻ��Ż����Ѿ����漣����Щ������˲��٣������������ʣ�������ķݣ���һֱ��������顣������˹˵�����ﲻ���ܣ�����û��ס�����Ҳ�ϣ���㱳��������������ߵ�ʱ����û�������һ�棬����һ���ǵ����ɣ�����Ц������Ƣ��������������˵��ʲô������������������ֻ�Ǽ��仰���Һø���¬�ˣ�����֪���ְ��Ǹ�ʲô�����ˡ�\r\n\r\n¬�����һ�̫�������һ���������ֻ�ܻ���������ģ���ģ�Խ��Խģ������Ҫ�Ǽǵ�ʲô��д�����ɣ��������ҵ��š�лл�㣬ά���У���ô�����ˣ��㻹�������ֵܡ�������ܽ\r\n"
                    });
                }
                else
                {
                    Debug.LogError("Task5: TaskManager �� InventoryManager δ��ȷ��");
                }
            }

            PlayerController playerController = Object.FindObjectOfType<PlayerController>();
            if (playerController != null && playerController.IsInDialogue())
                playerController.EndDialogue();

            if (IsTaskComplete())
            {
                if (taskManager != null)
                {
                    Debug.Log("Task5: ������ɣ��л��� Task6");
                    Task6 newTask = gameObject.AddComponent<Task6>();
                    taskManager.SetTask(newTask);
                    newTask.SetupTask(taskManager, dialoguePanel, dialogueText, nextButton);
                    taskManager.UpdateTaskDisplay();
                }
                else
                {
                    Debug.LogError("Task5: TaskManager δ�ҵ����޷��л��� Task6");
                }
            }
        }
    }

    private string[] GetDialogueForEve()
    {
        return new string[]
        {
            "����ܽ����¡��������ˣ�С�����ˡ�����¬�˸��ҵ��ţ��⺢�����ǵģ�����סһ�黹Ҫ�鷳�㡭��",
            "��������",
            "����ܽ����¡���������˵��֪���ְֵĹ��£������һ�̫�������⺢�ӣ�������������ۡ�",
            "����ܽ����¡����ְ�����ս���ϣ������ϱ�ά���еĲ��£���Ӧ����ʶά���аɡ�",
            "����ܽ����¡��ҡ���û�м����������һ�棬��ֻ֪����������������",
            "����ܽ����¡�¬�����ʰְ����ģ���ֻ�ø������ְ���Զ�������������������˾ͻ��������",
            "����ܽ����¡�������Ҳ��֪����ô���ǲ��ǶԵģ��Ͼ���Ҳֻ�ử���ˡ���",
            "����ܽ����¡���һֱ�����뻭һЩ���������뻭��ȥ�������Щ��Ұ���������������ְֻ���ʱ��Ц����",
            "����ܽ����¡���ÿ���±ʣ��ֶ��ڶ�����������ֻ��ģ����Ӱ�ӡ�",
            "����ܽ����¡����������һ��û��ս�������죬������ƣ�̫���߹ң��������ڲݵ����ܡ���",
            "����ܽ����¡���Ŀǰ�һ���������Ҳ��δ���ҿ��Ի������ɡ���",
            "����ܽ����¡����Ǹ�ά���е��ţ��鷳���͹�ȥ�ɡ�",
            "����ܽ����¡�������˹����˵����ά����һֱ�����ܣ���һֱΪ����ս����û���������ھΡ���",
            "����ܽ����¡��Ҳ��������Һܸ�л����ô������ɷ�Ĺ��ա�",
            "����ܽ����¡�лл������һ�ˣ�С�һ�������������æ���˰ɡ�"
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
            taskCompleteText.text = "����5�������еĹ�";
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

            Debug.Log("����5 ��ʼ�������ʾ������");
        }
        else
        {
            Debug.LogWarning("������������������δ��ȷ��ʼ�����޷���ʾ����5��ʼ��ʾ��");
        }
    }
}