using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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
        "�򡤻��أ��������ʾ���ұߣ����������ģ����ż�������",
        "�򡤻��أ�����ģ����Ը��ٵ�ǰ������ȡ�",
        "�򡤻��أ��ż��������Բ鿴��õ��ż���",
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

    private Vector3 teleportPosition = new Vector3(-7.3f, -2.5f, -6.1f);

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
            StartCoroutine(StartDialogueWithFadeOut());
        }
    }

    private IEnumerator StartDialogueWithFadeOut()
    {
        // ֱ����ʾ�Ի����
        dialoguePanel.SetActive(true);
        dialogueText.text = currentDialogue[dialogueIndex];
        // �Ӻ�������
        yield return StartCoroutine(FadeManager.Instance.FadeOut(3f));
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
            StartCoroutine(TransitionAndTeleport());
        }
    }

    private IEnumerator TransitionAndTeleport()
    {
        yield return StartCoroutine(FadeManager.Instance.FadeToBlack(() =>
        {
            dialoguePanel.SetActive(false);
            TeleportPlayer();
            SetupNextTask();
        }, 1f));
    }

    private void TeleportPlayer()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.transform.position = teleportPosition;
            if (playerController.IsInDialogue())
            {
                playerController.EndDialogue();
            }
        }
        else
        {
            Debug.LogWarning("δ�ҵ� PlayerController���޷�������ң�");
        }
    }

    private void SetupNextTask()
    {
        TaskManager taskManager = GetComponent<TaskManager>();
        if (taskManager != null)
        {
            Task1 newTask = gameObject.AddComponent<Task1>();
            taskManager.SetTask(newTask);
            newTask.SetupDialogueUI(dialoguePanel, dialogueText, nextButton);
            taskManager.UpdateTaskDisplay();
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