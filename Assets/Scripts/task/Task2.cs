using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Task2 : TaskBase
{
    private bool letterDeliveredToElias = false;
    private TMP_Text dialogueText;
    private GameObject dialoguePanel;
    private Button nextButton;
    private string currentResident;
    private string[] currentDialogue;
    private int dialogueIndex = 0;

    public override string GetTaskName()
    {
        return "ǹ���Ӻͱ�";
    }

    public override string GetTaskObjective()
    {
        return $"�ʹά���С�����������������˹�����������ţ�{(letterDeliveredToElias ? "�����" : "δ���")}";
    }

    public override bool IsTaskComplete()
    {
        return letterDeliveredToElias;
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

    public override void DeliverLetter(string targetResident)
    {
        currentResident = targetResident;
        dialogueIndex = 0;

        if (targetResident == "������˹������" && !letterDeliveredToElias)
        {
            currentDialogue = GetDialogueForResident("������˹������");
            letterDeliveredToElias = true;
            // �Ƴ��ż�
            InventoryManager inventory = FindObjectOfType<InventoryManager>();
            if (inventory != null) inventory.RemoveLetter("ά���и�������˹����");
        }
        else
        {
            currentDialogue = new string[] { "���������㻹û����" };
        }

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
            UpdateDisplay();
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null && playerController.IsInDialogue())
            {
                playerController.EndDialogue();
            }
        }
    }

    private void UpdateDisplay()
    {
        TaskManager manager = GetComponent<TaskManager>();
        if (manager != null)
        {
            manager.UpdateTaskDisplay();
        }
    }

    private string[] GetDialogueForResident(string residentName)
    {
        switch (residentName)
        {
            case "������˹������":
                return new string[]
                {
                    "��������˹��������Ŷ�������㰡��СС�Ļ����ˡ�",
                    "��������˹��������˭�����أ����ҿ�����",
                    "��������",
                    "��������˹���������������ף����������������׵��ּ�����ǹ��һ��Ӳ������Ρ������˵�ʲô������ǰ�����ֲ�����ô����ģ�Ҳ���������У����ܸ�����ʵ�ı���Լ��ɡ�",
                    "��������˹�����������ᵽ��ĸ�ף��һ��ǵ������ҵ�һ��ʫʱ��Ц�ݣ���˵�һ��Ϊʫ�ˡ�����ʱ��̫���������ϡ�",
                    "��������˹����������������̫��������ʱ�򣬵�ս������ج�ĵ�ʱ�򣬸��״������뿪�˳��У��������Ż�塣",
                    "��������˹��������ĸ��û�и�����һ���뿪���ܶ�����Ҳ�֪��������¶�ڱ�������˲��������£����ײ��ϸ����ҡ��������ҵ�ʫ�衪�������Ҽ���ĸ�׵ķ�ʽ������Һ����־��ˡ�",
                    "��������˹����������˵ս������һ�У���Ҳ֪���������ϵ��˰�������ʬ����������ļ�֤����������⣬ʫ���ҵ�һ�У���ĸ�״��ڹ���֤�ݡ�",
                    "��������˹�������������ҹ��úò��á�������Ȼ����������Ǹ���Ĭ���ϱ���ս���ϵ����ˣ�����Ϊ�����������ô�ʡ�",
                    "��������˹����������֪��������û˵������ʧȥ�ҡ�",
                    "��������˹�������������Ҹ�����ţ��������ɡ�",
                    "��������˹����������������������������ü���Ի��������ӣ����������ǵ��侲�Ĺ⣬��������Ц��ʫ��û�߼���",
                    "��������˹�������������ȥ���������Ҵ��仰�ɣ���˵������û����������ǹ������Ҳ���ᶪ���ҵıʡ�",
                    "��������˹���������һ��ٸ������ŵġ�"
                };
            default:
                return new string[] { "���������㻹û����" };
        }
    }
}