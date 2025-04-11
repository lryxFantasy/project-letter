using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Task0 : TaskBase
{
    private TMP_Text dialogueText;
    private GameObject dialoguePanel;
    private Button nextButton;

    private RubyController rubyController;
    private PlayerController playerController;
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
        "�򡤻��أ���ʾ�����Ͻ������ʣ�������",
        "�򡤻��أ��㻹���Բ鿴�����������ķ���ֵ��",
        "�򡤻��أ���¶�ڷ��价���»᲻��������ĵ�����",
        "�򡤻��أ��Ұ��㰲�˸��Ǹ�����������ո�����Բ鿴��Χ�ķ�������",
        "�򡤻��أ�ǧ��Ҫ�߽����������������Ѱ��·��",
        "�򡤻��أ���û��ʱ������ȥ�����˼���Ѱ�������",
        "�򡤻��أ�����������ľ������Ҿ�ֻ�ܴ��Ϸ����������ϻ��Ҽ��ˡ�",
        "�򡤻��أ����ԡ���ע����ĵ������Ҳ����˷�������",
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
        // ȷ����ʼ������ȷ
        AudioManager audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            CameraController cameraController = FindObjectOfType<CameraController>();
            if (cameraController != null && cameraController.IsIndoors())
            {
                audioManager.ForceSwitchBGM(true); // ǿ�Ʋ�����������
            }
        }
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
            rubyController = FindObjectOfType<RubyController>();
            playerController = FindObjectOfType<PlayerController>();
            rubyController.pauseHealthUpdate = true;
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
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        dialoguePanel.SetActive(true);
        dialogueText.text = currentDialogue[dialogueIndex];
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

            // �л�����������
            AudioManager audioManager = AudioManager.Instance;
            if (audioManager != null)
            {
                audioManager.ForceSwitchBGM(false); // ǿ�Ʋ�����������
            }

            // ���� CameraController �� isIndoors ״̬
            CameraController cameraController = FindObjectOfType<CameraController>();
            if (cameraController != null)
            {
                cameraController.ExitHouse(); // ģ���˳����ݣ����� isIndoors = false
            }
        }, 1f));
    }

    private void TeleportPlayer()
    {
        rubyController = FindObjectOfType<RubyController>();
        rubyController.pauseHealthUpdate = false;
        if (playerController != null)
        {
            playerController.transform.position = teleportPosition;
            playerController.enabled = true;
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