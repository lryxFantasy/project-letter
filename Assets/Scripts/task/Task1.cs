using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Task1 : TaskBase
{
    private int visitCount = 0;
    private bool letterDeliveredToVictor = false;
    private bool returnedToJane = false;
    private string[] residents = { "ά���С�����", "������˹������", "�򡤻���", "��˿", "С¬�ˡ����", "��ܽ�����" };
    private bool[] visitedResidents;

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
        return $"�ݷ��Ż���ÿһλ����{visitCount}/6����\n" +
               $"�ʹ�򡤻��ء�����ά���С����������ţ�{(letterDeliveredToVictor ? "�����" : "δ���")}��\n" +
               $"��ȥ�ҡ��򡤻��ء���{(returnedToJane ? "�����" : "δ���")}";
    }

    public override bool IsTaskComplete()
    {
        return visitCount >= 6 && letterDeliveredToVictor && returnedToJane;
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

    public void DeliverLetter(string targetResident)
    {
        currentResident = targetResident;
        dialogueIndex = 0;

        if (targetResident == "ά���С�����" && !letterDeliveredToVictor)
        {
            currentDialogue = GetDialogueForResident("ά���С�����");
            letterDeliveredToVictor = true;
            VisitResident(targetResident);
        }
        else if (targetResident == "�򡤻���" && visitCount >= 6 && letterDeliveredToVictor && !returnedToJane)
        {
            currentDialogue = GetDialogueForResident("�򡤻���");
            returnedToJane = true;
        }
        else if (System.Array.IndexOf(residents, targetResident) >= 0 && !visitedResidents[System.Array.IndexOf(residents, targetResident)])
        {
            currentDialogue = GetDialogueForResident(targetResident);
            VisitResident(targetResident);
        }
        else
        {
            currentDialogue = new string[] { "�㻹û����" };
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
            Debug.Log($"�ݷ��� {residentName}����ǰ���ȣ�{visitCount}/6");
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
                    "����ʲô�����������ͭ�����������ģ�",
                    "�����������ŵģ��ߣ���Ѿͷ��֪��Ū��Щ�Ƽ�������������أ�����������ĥ�䡣",
                    "�š�����˵Ҫ�����Ϊ�Ż�����ʹ���ߣ��Ƽ������ҵ�һ�У���̫�������Ƽ������ˣ���֪����᲻���ʲô���ӡ�",
                    "���ˣ����Ǹ�������˹���ţ�����������д��Щû�õ�ʫ�����˸øɵ����¡���Ҫ�Ҵ򿪻�Ū���ˣ��Ҿ������㡣",
                    "�������ﻹ���ߣ�����������ۣ�����������Ƥ���Ӿͷ���"
                };
            case "������˹������":
                return new string[]
                {
                    "Ŷ�������µ��������𣿿�����ģ������������ӷ��������������ġ��Ÿ��ҿ����ɡ�",
                    "ԭ���Ǽ��޺���ģ���������š�����Ȼ���Ӳ����ҵ�ʫ�����������רע�о�ʱ���������ǿ����ǡ�����ϧ��Ϊ̫�������䣬���Ǻ��Ѽ��档",
                    "���Ǹ������ţ�������������������������ı��飬������ֻ����ҽ���еԭ��͵�·����",
                    "���һֱ�ڴ���������Ҳ����������Ϊ��д��ʫ�������������Ľ��졣"
                };
            case "�򡤻���":
                return new string[]
                {
                    "�����ˣ��������ʱ����������ӣ���������������������",
                    "�Ŷ��͵��ˣ��ã��ɵò���ά����û���������ɣ�����������������Ⱦ��Ǳ��˹�������װ���˵ģ���������Ҳ���ǳ�ս���й����ˡ���",
                    "�������У������Ԥ��Ŀ��ס��ϱ�Ҫ�������𣬺õģ��Ұ���ʵʱ����һ������ģ�飬�������ʾ���ұ߾Ϳ��Կ����ˣ��Ժ�������Զ����¡�",
                    "��ָ���ҿ��㣬����Ǹ����ߣ��ɻ�����ı��֡�"
                };
            case "��˿":
                return new string[]
                {
                    "��ѽ���������ŵ�С�һ�ɣ���ɰ�����Ȼ�����������ģ����С�Դ�һ���ܼ�Ӳ�ɡ�",
                    "����ҵ����𣿺ã��鷳���ˣ�С�һ",
                    "ԭ����ˣ��������޺��˰����������������������Ӵ�ѧ��������������ӱ��ѣ����������Ů�����̫ѹ���ˣ����ǿ�����������������˹��С���ӣ��Ǻǡ���",
                    "·��С�İ���������Ȼ�˲����㣬����������·�����ߣ���ˤ���ˡ�"
                };
            case "С¬�ˡ����":
                return new string[]
                {
                    "�ۣ�������ǻ������𣿻���𣿻�˵���𣿻��ǻ���ε�ѽ��",
                    "���Ǵ��Ķ����ģ�������������˵�������кö�ֶ��������ǹֶ�����",
                    "�������ҵ��ţ�����ģ������ҿ�����",
                    "��Ү���Ժ����Ǵ����Լ�����ʹ�ˣ�֮ǰ��һ���²��з���Ļ����������Ǵ塣",
                    "���´λ������ҿ��Ը��㻭����������������ϣ����������㣡"
                };
            case "��ܽ�����":
                return new string[]
                {
                    "�������ŵģ���ϡ�棬���Ǵ������ġ�",
                    "���������ģ��������޺���Щ��������ȴֻ������ɫͿĨ���䡭�����ҿ��������š�",
                    "�����Ժ����Ǵ�����ʹ�ˣ�һ�ж���������ɣ���ϣ��¬���ܿ�������лл������һ�ˡ�",
                    "��������Щ�⼣��������ͦ�ã������ʱ�����ͻ�������ʱ��ĺۼ���"
                };
            default:
                return new string[] { "�㻹û����" };
        }
    }
}