using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public void SetupTask(TaskManager manager, GameObject panel, TMP_Text text, Button button)
    {
        taskManager = manager;
        dialoguePanel = panel;
        dialogueText = text;
        nextButton = button;
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(NextDialogue);
        dialoguePanel.SetActive(false);
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
            "���򡿻����ˣ����ҵ��ţ����ҿ�����",
            "��������",
            "����������˹�ģ�����д��Щ�������߰���Ĵʣ��ҵ���������ܿ�����",
            "������˵���˳���������ǿգ����Ǽǵã����ҵ�ʱֻ������ֵ��������˼������",
            "����������д����ôֱ�ӣ����桭����֪����ô��Ӧ�����Ӷ����ˡ���̸����ʱҲ����������������·�ĵ�أ����е�����ϡ�",
            "������������֮ǰ���ҵ���������0��1���ɵģ�һ�����ﶼ���Ա���ʽ�������������ƻ����ҵ��������bug����Щ��е�ӿ������һʱ����Ӧ�ԡ�",
            "������ϲ�����ݣ�ϲ�����޺õĶ�������ȴ�ϸ��ҽ�ʫ��������������Щץ��ס�ġ�����ʵ��ʱ�����ܶ���������Щ�����������ݣ����ܻ���á������߰ɡ�",
            "������ɵ�ӣ���������ô�찡���������ٻ�ȥ���������Ҹ���˵����˵�����ţ��Ҳ�̫��˵��Щ�����Ĵ��壬�����ܣ���ȷʵϲ���������һ���д�Ÿ����ġ�",
            "����Ŷ���ˣ�������˿˵������������ţ����ڴ��Ӷ������Ե��С�����лл�㡣"
        };
    }
}