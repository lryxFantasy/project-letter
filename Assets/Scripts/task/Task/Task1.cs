using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Task1 : TaskBase
{
    [SerializeField] private int visitCount = 0;
    [SerializeField] private bool letterDeliveredToVictor = false;
    [SerializeField] private bool returnedToJane = false;
    [SerializeField] private string[] residents = { "ά���С�����", "������˹������", "�򡤻���", "��˿", "С¬�ˡ����", "��ܽ�����" };
    [SerializeField] private bool[] visitedResidents;

    private TMP_Text dialogueText;
    private GameObject dialoguePanel;
    private Button nextButton;

    private GameObject normalDialoguePanel;
    private Button deliverButton;

    private string[] currentDialogue;
    private int dialogueIndex = 0;
    private string currentResident;

    void Start()
    {
        visitedResidents = new bool[residents.Length];
    }

    public override string GetTaskName()
    {
        return "�Ż��ĵ�һ����";
    }

    public override string GetTaskObjective()
    {
        return $"�ݷ��Ż��ļ�����ÿһλ����{visitCount}/5����\n" +
               $"�ʹ�򡤻��ء�����ά���С����������ţ�{(letterDeliveredToVictor ? "�����" : "δ���")}��\n" +
               $"��ȥ�ҡ��򡤻��ء���{(returnedToJane ? "�����" : "δ���")}";
    }

    public override bool IsTaskComplete()
    {
        return visitCount >= 5 && letterDeliveredToVictor && returnedToJane;
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

    public void SetupDeliverButton(GameObject normalPanel, Button deliverBtn)
    {
        normalDialoguePanel = normalPanel;
        deliverButton = deliverBtn;
        deliverButton.onClick.RemoveAllListeners();
        deliverButton.onClick.AddListener(() =>
        {
            normalDialoguePanel.SetActive(false); // ������ͨ�Ի���
            GetComponent<TaskManager>().TriggerDeliverLetter(); // ����TaskManager�����ŷ���
        });
    }

    public override void DeliverLetter(string targetResident)
    {
        currentResident = targetResident;
        dialogueIndex = 0;

        if (targetResident == "ά���С�����" && !letterDeliveredToVictor)
        {
            currentDialogue = GetDialogueForResident("ά���С�����");
            letterDeliveredToVictor = true;
            VisitResident(targetResident);
            // �Ƴ�����Ų����ά���е���
            TaskManager taskManager = GetComponent<TaskManager>();
            if (taskManager != null && taskManager.inventoryManager != null)
            {
                Debug.Log("������ż�");
                taskManager.inventoryManager.RemoveLetter("���ά���е���");
                taskManager.inventoryManager.AddLetter(new Letter
                {
                    title = "ά���е���",
                    content = "������˹����Ū�˸��ƻ������ţ�FANLUʲô�ͺţ������⼣�����Ų�˳�ۣ����ǲ�ϲ����Щ�Ƽ������������˵����Ψһ�İ취�����������ס�ˣ�����ȥ���������ֲ�����ֻ�ܿ������Ƥ���ȡ���˵�����������죬�������������Ҳ�����б��¡����㻹��д��Щʫ����֪����ϲ������С�Ͱ���Ūֽ�ʣ��Ҳ�������Щ��ʲô�ã��������ڵ�ʱ����˵������֣�˵����д����������ȴϲ���ľ��ӡ�ս����һ�ж����ˣ�̫��û�ˣ����ӹ������һ��������Ҳ�����ˣ��ǳ�ս�������������ң��۵�������û˯��������������ս�ѵ���һƬ��ѪȾ������أ���֪���ģ�Ҳ����С¬�˵ĸ��ף���������������ĸ�����ˡ�����������̫�����죬����ָ�Ӳ��������������������������ӣ��������Ҳ�Ų�ס�������ˡ��Ҳ�֪�����ǵ������Ƿ������죬���������ǵü����İɡ���\r\n���Ǳ���ô����������˵�������һ���ˣ��������������ͦʵ�ڣ��޻���������û���������ܸ���ѧ�㶫��Ҳ�������Ҳ���˵����Щʫû�ã����ǡ�����֪������úò��ã��Ͼ������Ҷ��ӡ�ʫ��Ȳ�������ս������ȥ��ս�ѣ�����Ҳ������ʧȥ�㡣��˵���������ס�����ܵö����л���д�������������ϵ��š���Ҫû�վ����ˣ�����ǿ������ά����\r\n"
                });
            }
            else
            {
                Debug.LogError("TaskManager �� InventoryManager δ��ȷ�󶨣��޷����±���");
            }
        }
        else if (targetResident == "�򡤻���")
        {
            currentDialogue = GetDialogueForResident("�򡤻���");
            if (visitCount >= 5 && letterDeliveredToVictor && !returnedToJane)
            {
                returnedToJane = true;
            }
        }
        else if (System.Array.IndexOf(residents, targetResident) >= 0 && !visitedResidents[System.Array.IndexOf(residents, targetResident)])
        {
            currentDialogue = GetDialogueForResident(targetResident);
            VisitResident(targetResident);
        }
        else
        {
            currentDialogue = new string[] { "���������㻹û����" };
        }

        StartDialogue();
    }

    private void VisitResident(string residentName)
    {
        int index = System.Array.IndexOf(residents, residentName);
        if (index >= 0 && !visitedResidents[index])
        {
            visitedResidents[index] = true;
            visitCount++;
            Debug.Log($"�ݷ��� {residentName}����ǰ���ȣ�{visitCount}/5");
            UpdateDisplay();
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
            UpdateDisplay();
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null && playerController.IsInDialogue())
            {
                playerController.EndDialogue();
            }
            // ��������Ƿ���ɲ���ת�� Task2
            if (IsTaskComplete())
            {
                TaskManager taskManager = GetComponent<TaskManager>();
                if (taskManager != null)
                {
                    Task2 newTask = gameObject.AddComponent<Task2>();
                    taskManager.SetTask(newTask);
                    newTask.SetupTask(taskManager, dialoguePanel, dialogueText, nextButton); // �޸�Ϊ SetupTask
                    taskManager.UpdateTaskDisplay();
                }
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
            case "ά���С�����":
                return new string[]
                {
                    "��ά���С�����������ʲô�����������ͭ�����������ģ�",
                    "��ά���С������������������ŵģ�",
                    "��ά���С��������ߣ���Ѿͷ��֪��Ū��Щ�Ƽ����������",
                    "��ά���С����������أ�����������ĥ�䡣",
                    "��������",
                    "��ά���С��������š�����˵Ҫ�����Ϊ�Ż�����ʹ��",
                    "��ά���С��������ߣ��Ƽ������ҵ�һ�У���̫�������Ƽ������ˣ���֪����᲻���ʲô���ӡ�",
                    "��ά���С����������ˣ����Ǹ�������˹���ţ�����������д��Щû�õ�ʫ�����˸øɵ����¡�",
                    "��ά���С���������Ҫ�Ҵ򿪻�Ū���ˣ��Ҿ������㡣",
                    "��ά���С��������������ﻹ���ߣ�����������ۣ�����������Ƥ���Ӿͷ���"
                };
            case "������˹������":
                return new string[]
                {
                    "��������˹��������Ŷ�������µ��������𣿿�����ģ������������ӷ��������������ġ�",
                    "��������˹���������Ÿ��ҿ����ɡ�",
                    "��������",
                    "��������˹��������ԭ���Ǽ��޺���ģ���������š�����Ȼ���Ӳ����ҵ�ʫ����",
                    "��������˹���������������רע�о�ʱ���������ǿ����ǡ���",
                    "��������˹����������ϧ��Ϊ̫�������䣬���Ǻ��Ѽ��档",
                    "��������˹��������������������������������ı��飬������ֻ����ҽ���еԭ��͵�·����",
                    "��������˹�����������һֱ�ڴ���������Ҳ����������Ϊ��д��ʫ�������������Ľ��졣"
                };
            case "�򡤻���":
                if (visitCount >= 5 && letterDeliveredToVictor && !returnedToJane)
                {
                    returnedToJane = true;
                    return new string[]
                    {
                        "���򡤻��ء������ˣ��������ʱ����������ӣ���������������������",
                        "���򡤻��ء��Ŷ��͵��ˣ��ã��ɵò���",
                        "���򡤻��ء�ά����û���������ɣ�����������������Ⱦ��Ǳ��˹�������װ���˵ģ���������Ҳ���ǳ�ս���й����ˡ���",
                        "���򡤻��ء��������У������Ԥ��Ŀ��ס�",
                        "���򡤻��ء��ϱ�Ҫ�������𣬺õģ��Ұ���ʵʱ����һ������ģ�顣",
                        "���򡤻��ء��������ʾ���ұ߾Ϳ��Կ����ˣ��Ժ�������Զ����¡�",
                        "���򡤻��ء���ָ���ҿ��㣬����Ǹ����ߣ��ɻ�����ı��֡�"
                    };
                }
                else
                {
                    return new string[]
                    {
                        "���򡤻��ء�������������",
                        "���򡤻��ء���ȥ�ɣ���������˷�ʱ�䣬���ﻹ�������˵����������ء�"
                    };
                }
            case "��˿":
                return new string[]
                {
                    "����˿����ѽ���������ŵ�С�һ�ɣ�",
                    "����˿����ɰ�����Ȼ�����������ģ����С�Դ�һ���ܼ�Ӳ�ɡ�",
                    "����˿������ҵ����𣿺ã��鷳���ˣ�С�һ",
                    "��������",
                    "����˿��ԭ����ˣ��������޺��˰���������������",
                    "����˿���������Ӵ�ѧ��������������ӱ��ѣ����������Ů�����̫ѹ���ˣ����ǿ�����������������˹��С���ӣ��Ǻǡ���",
                    "����˿��·��С�İ���������Ȼ�˲����㣬����������·�����ߣ���ˤ���ˡ�"
                };
            case "С¬�ˡ����":
                return new string[]
                {
                    "��С¬�ˡ���¡��ۣ�������ǻ������𣿻���𣿻�˵���𣿻��ǻ���ε�ѽ��",
                    "��С¬�ˡ���¡����Ǵ��Ķ����ģ�������������˵�������кö�ֶ��������ǹֶ�����",
                    "��С¬�ˡ���¡��������ҵ��ţ�����ģ������ҿ�����",
                    "��������",
                    "��С¬�ˡ���¡���Ү���Ժ����Ǵ����Լ�����ʹ�ˡ�",
                    "��С¬�ˡ���¡�֮ǰ��һ���²��з���Ļ����������Ǵ塣",
                    "��С¬�ˡ���¡����´λ������ҿ��Ը��㻭����������������ϣ����������㣡"
                };
            case "��ܽ�����":
                return new string[]
                {
                    "����ܽ����¡��������ŵģ���ϡ�棬���Ǵ������ġ�",
                    "����ܽ����¡����������ģ��������޺���Щ��������ȴֻ������ɫͿĨ���䡭�����ҿ��������š�",
                    "��������",
                    "����ܽ����¡������Ժ����Ǵ�����ʹ�ˣ�һ�ж���������ɡ�",
                    "����ܽ����¡���ϣ��¬���ܿ�������лл������һ�ˡ�",
                    "����ܽ����¡���������Щ�⼣��������ͦ�ã������ʱ�����ͻ�������ʱ��ĺۼ���"
                };
            default:
                return new string[] { "���������㻹û����" };
        }
    }
}