using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Task0 : TaskBase
{
    private TMP_Text dialogueText;
    private GameObject dialoguePanel;
    private Button nextButton;

    private string[] currentDialogue;
    private int dialogueIndex = 0;
    private bool hasStarted = false;

    private string[] janeDialogue = new string[]
    {
    "�򡤻��أ������ˣ�",
    "�򡤻��أ��һ�������ʱ���޺��㣬����Щ�������ռ�������������ܲ�̫�ȶ�����Ӧ�ù��á�",
    "�򡤻��أ����ţ�����FANLU-317���԰ɣ�",
    "�򡤻��أ��Ҳ�֪���㻹�ǲ��ǵ��Լ��Ĺ��ܣ����������ڿ�ʼ�������������ˡ�",
    "�򡤻��أ��������ʾ����ߣ����������ģ����ż�������������Ҫ�򿪱��˵��ţ���ܲ���ò��",
    "�򡤻��أ��������Ż�壬��ط����󣬵��˸���֮�䡭������ͦԶ��",
    "�򡤻��أ������ô��û�������ţ��������ֻ�ܿ�������ϵ��",
    "�򡤻��أ���ϧ���������ܲ������������ܡ����������ɱ�̫���ˡ�",
    "�򡤻��أ����ԣ������������",
    "�򡤻��أ�ȥ����ÿ���˵�ס����һ������·�ɣ�֮ǰ�ϱ��ƺ�˵���ͷ��Ÿ������ӣ������ȥ��������",
    "�򡤻��أ�������Ϊʲô�������ߵ硪�����������û���ˣ������һ�ж��ջ��ˡ�",
    "�򡤻��أ�����Ψһ�İ취��",
    "�򡤻��أ�������д�ģ�����ά���еģ������Ҹ�ÿ������д�˷���š�",
    "�򡤻��أ��߰ɣ���ĥ�䣬������ɺ�������ҡ�"
    };

    void Start()
    {
    }

    public void SetupDialogueUI(GameObject panel, TMP_Text text, Button button)
    {
        dialoguePanel = panel;
        dialogueText = text;
        nextButton = button;
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(NextDialogue);
        dialoguePanel.SetActive(false);
    }

    public void StartTaskDialogue()
    {
        if (!hasStarted)
        {
            hasStarted = true;
            currentDialogue = janeDialogue;
            dialogueIndex = 0;
            TaskManager taskManager = GetComponent<TaskManager>();
            if (taskManager != null && taskManager.normalDialoguePanel != null)
            {
                taskManager.normalDialoguePanel.SetActive(false);
            }
            StartDialogue();
        }
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
            TaskManager taskManager = GetComponent<TaskManager>();
            if (taskManager != null)
            {
                Task1 newTask = gameObject.AddComponent<Task1>();
                taskManager.SetTask(newTask);
                newTask.SetupDialogueUI(dialoguePanel, dialogueText, nextButton);
                taskManager.UpdateTaskDisplay();
            }
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null && playerController.IsInDialogue())
            {
                playerController.EndDialogue();
            }
        }
    }

    public override string GetTaskName()
    {
        return "��ʼ�Ի�";
    }

    public override string GetTaskObjective()
    {
        return "��򡤻��ضԻ�";
    }

    public override bool IsTaskComplete()
    {
        return dialogueIndex >= currentDialogue.Length;
    }

    // ʵ�� DeliverLetter������ Task0 û��ʵ�����Ź���
    public override void DeliverLetter(string targetResident)
    {
        Debug.Log("Task0: ��ֻ�ǳ�ʼ�Ի������޷����š�");
        if (dialoguePanel != null && dialogueText != null)
        {
            dialoguePanel.SetActive(true);
            dialogueText.text = "�򡤻��أ����Ѿ����������ˣ���ȥ���Űɡ�";
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(() => dialoguePanel.SetActive(false));
        }
    }
}