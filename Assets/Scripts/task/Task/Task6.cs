using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
                    taskManager.inventoryManager.RemoveLetter("��ܽ����");
                    taskManager.inventoryManager.AddLetter(new Letter
                    {
                        title = "���ط��ӵ�Կ��",
                        content = "�ϱ��ڼ��﷭������Կ�ף��ƺ����Դ򿪴����ϱ��������ӣ�Ҳ����Կ���������Щʲô��"
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
            "��ά���С����������㰡����Ƥ��ͷ���������������ˣ������ɣ������Ҷ��ӵ��£�����лл���ˡ������أ��ø��ҡ�",
            "��������",
            "��ά���С�������������ܽ��Ů�ˡ�����˵�������ҡ����ҶԲ�������һ�ң�С¬�˵ĸ��ף�¬�ˣ�����������ǰ����û��ס������",
            "��ά���С�����������ս���Ϻܻ��ң��ӵ����Ҷ��ߺ�Х���ӹ�������������ζ�̱ǵ�������Ϣ��������Ѫ���Һ���ſ�£�����û�������������������ؿڣ�Ѫ������һ����",
            "��ά���С�������������������������ȥʱ�����Ѿ�ֻʣһ�����ˣ���������ס�ң���һֻ���ﻹ߬��ǹ������˵Щʲô�����Ѿ�û���ˣ�����֪�������ں�����������",
            "��ά���С���������û��������û�������Ż�ȥ�����Ķ��ӡ�",
            "��ά���С�����������������ʱ��ϵģ��۵���˯���ţ����һ��Ż����ˣ���û�ء���ܽ˵�������ң����ҹ������Լ���ء���ս���������Ұ���ܽ���ǽӵ����Ż����ѡ�",
            "��ά���С�������ÿ�ο���С¬���Ǻ��ӣ��Ҿ�����¬�ˣ�������ÿ���ᵽ�������ϵ�Ц�ݡ�����Ƿ���ǵģ�̫���ˡ�",
            "��ά���С�������Ҳ����ʱ���ͻ��ˣ��һ�����ǻ��ŵģ�����Ѽ������¬��д����������+��������Ҫ��ʱ��ɡ���������ܽ�һ���ŵġ����Ҹ���˵����������̫�ۣ�С¬�˻��ÿ�����",
            "��ά���С�������Ŷ���ˣ��������ڼ��﷭������Կ�ף��ƺ����Դ򿪴����ϱ��������ӣ�����û��������ˣ�Ҳ������Կ���������Щʲô��"
        };
    }
}