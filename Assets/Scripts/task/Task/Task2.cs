using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[System.Serializable]
public class Task2 : TaskBase
{
    private bool letterDeliveredToElias = false;
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

    public override string GetTaskName() => "ǹ���Ӻͱ�";

    public override string GetTaskObjective() => $"�ʹά���С�����������������˹�����������ţ�{(letterDeliveredToElias ? "�����" : "δ���")}";

    public override bool IsTaskComplete() => letterDeliveredToElias;

    public override void DeliverLetter(string targetResident)
    {
        dialogueIndex = 0;
        currentDialogue = targetResident == "������˹������" && !letterDeliveredToElias
            ? GetDialogueForElias()
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
            if (!letterDeliveredToElias && currentDialogue.Length > 1)
            {
                letterDeliveredToElias = true;
                if (taskManager != null && taskManager.inventoryManager != null)
                {
                    Debug.Log("Task2: �����ż� - �Ƴ�ά���е��ţ����������˹����");
                    taskManager.inventoryManager.RemoveLetter("ά���е���");
                    taskManager.inventoryManager.AddLetter(new Letter
                    {
                        title = "������˹����",
                        content = "���ҵ�����֮�⣬�ҵ��������ҵ�����������ҵ��ǳ����㲻���ұ��µķ籩��������Ϊ���ڲ����µ��������Ҳ��ں������㻹�ǵ�������������ʶ��ҹ��ս���ս��������仹δ��ȫ�������ӣ��������̫����ȴ��һƬ�д���ǹ⣬������ľ�������ɽ���ϡ�������ˣ����������ĳ�ȹ���������Ź��������ҹ�����ֵ���ƹ�ҡҷ�£������Ӱ��һ�����������ҵ��ġ��������ڸ�ʲô����˵��дʫ������üЦ��һ����˵��ʫ���ܵ�����ҹ������һ�̣��ҿ�������۾����峺�����δ��ս�����۵�ˮ�棬�ҵ��������˽��ࡣ\r\n" +
                                  "�������ҳ�ȥ���㡣����С������ͷ��ɣ���רע�ı��飬����д�����κ�ʫ�ж����ˡ���һ�̣��ҳ��������ˡ��Ұ��㣬�������㣬�ҵ�����ȫ�����Ӱ�ӡ�����������ң�����Ը�����ɶ꣬�����㴰ǰ�ĵ� fusing���ң������ճɻң�ҲҪ��������¶ȡ��㲻���ҵĿ��ȣ��򣬿������Ұ����֤������������Զվ�����Ե�ɽ�����ۿ��ң���ҲҪ��������Ϊ��ȼ�����棬Ϊ��Ϳ����Ұ������ɫ�ʡ�����ݰ�����Ϩ�𡣡���������˹"
                    });
                }
                else
                {
                    Debug.LogError("Task2: TaskManager �� InventoryManager δ��ȷ�󶨣��޷������ż�");
                }
            }

            PlayerController playerController = Object.FindObjectOfType<PlayerController>();
            if (playerController != null && playerController.IsInDialogue())
            {
                playerController.EndDialogue();
            }

            if (IsTaskComplete())
            {
                if (taskManager != null)
                {
                    Debug.Log("Task2: ������ɣ��л��� Task3");
                    Task3 newTask = gameObject.AddComponent<Task3>();
                    taskManager.SetTask(newTask);
                    newTask.SetupTask(taskManager, dialoguePanel, dialogueText, nextButton);
                    taskManager.UpdateTaskDisplay();
                }
                else
                {
                    Debug.LogError("Task2: TaskManager δ�ҵ����޷��л��� Task3");
                }
            }
        }
    }

    private string[] GetDialogueForElias()
    {
        return new string[]
        {
            "��������˹��������Ŷ�������㰡��СС�Ļ����ˡ�˭�����أ����ҿ�����",
            "��������",
            "��������˹���������������ף����������������׵��ּ�����ǹ��һ��Ӳ������Ρ������˵�ʲô������ǰ�����ֲ�����ô����ģ�Ҳ���������У����ܸ�����ʵ�ı���Լ��ɡ�",
            "��������˹�����������ᵽ��ĸ�ף��һ��ǵ������ҵ�һ��ʫʱ��Ц�ݣ���˵�һ��Ϊʫ�ˡ�����ʱ��̫���������ϡ�",
            "��������˹����������������̫��������ʱ�򣬵�ս������ج�ĵ�ʱ�򣬸��״������뿪�˳��У��������Ż�塣",
            "��������˹��������ĸ��û�и�����һ���뿪���ܶ�����Ҳ�֪��������¶�ڱ�������˲��������£����ײ��ϸ����ҡ��������ҵ�ʫ�衪�������Ҽ���ĸ�׵ķ�ʽ������Һ����־��ˡ�",
            "��������˹����������˵ս������һ�У���Ҳ֪���������ϵ��˰�������ʬ����������ļ�֤����������⣬ʫ���ҵ�һ�У���ĸ�״��ڹ���֤�ݡ�",
            "��������˹�������������ҹ��úò��á�������Ȼ����������Ǹ���Ĭ���ϱ���ս���ϵ����ˣ�����Ϊ�����������ô�ʡ�",
            "��������˹����������֪��������û�����һ��˵������ʧȥ�ҡ�",
            "��������˹�������������Ҹ�����ţ��������ɡ���������������������ü���Ի��������ӣ����������ǵ��侲�Ĺ⣬��������Ц��ʫ��û�߼���",
            "��������˹�������������ȥ���������Ҵ��仰�ɣ���˵������û����������ǹ������Ҳ���ᶪ���ҵıʡ�",
            "��������˹���������һ��ٸ������ŵġ�"
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
                taskCompletePanel.SetActive(false);
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
            taskCompleteText.text = "����2����ǹ���Ӻͱ�";
            taskCompletePanel.SetActive(true);

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

            Debug.Log("����2 ��ʼ�������ʾ������");
        }
        else
        {
            Debug.LogWarning("������������������δ��ȷ��ʼ�����޷���ʾ����2��ʼ��ʾ��");
        }
    }
}