using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[System.Serializable]
public class Task4 : TaskBase
{
    private bool visitedRose = false;
    private bool letterDeliveredToLuke = false;
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

    public override string GetTaskName() => "��������";

    public override string GetTaskObjective() => $"�ݷá���˿����\n" +
                                                 $"�ʹ��˿������С¬�ˡ���¡����ţ�{(letterDeliveredToLuke ? "�����" : "δ���")}";

    public override bool IsTaskComplete() => visitedRose && letterDeliveredToLuke;

    public override void DeliverLetter(string targetResident)
    {
        dialogueIndex = 0;
        if (targetResident == "��˿" && !visitedRose)
        {
            currentDialogue = GetDialogueForRose();
        }
        else if (targetResident == "С¬�ˡ����" && visitedRose && !letterDeliveredToLuke)
        {
            currentDialogue = GetDialogueForLuke();
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
            if (taskManager != null && taskManager.inventoryManager != null)
            {
                if (!visitedRose && currentDialogue.Length > 1)
                {
                    visitedRose = true;
                    Debug.Log("Task4: �����˿����");
                    taskManager.inventoryManager.AddLetter(new Letter
                    {
                        title = "��˿����",
                        content = "�װ���С���ѣ�����ҹ�������ڴ��ߣ���������������������˴�ǰ�����ӡ���ʱ�����绹û���������֣����������һ��ϴ���Ĳ���̫��������ͷ����Ӳӵģ�����������ů�����ڻ��˵��ġ����ϣ�¶�����ڲ�Ҷ�ϣ���һ�ſ�С���飬���Ź⣬��һ�����ݾ������裬����ɳɳ�ĸ������ӱ���������������֦����������������ͷ�����������ˮ�棬ˮ����ܿ������������ȥ���һ��ǵ������ҹ�������ڲݴ��ﳪ�裬ө�������С���������ɣ������ϵ����ǵ�������ˣ������������Ժ����ҡ���ӣ������¡������أ���ѩ��ʱ��ѩ��Ʈ�������������ë�����ڵ��ϾͶѳɺ���̺�ӣ����ǹ���Χ���ѩ�ˣ���ѩ�̣�Ц�������������ӡ�\r\n\r\nС���ѣ���Щ���õĶ���������ֻ�������������ˣ�����������㣬�ɳ���ֻ�ǵ��������ã�����ѧ���ڻҰ����ҵ��⡣��Ҫ�¸ң���������һ��������˫�ֳ������ǵ������Ҫ����������֣��������ˣ�����Ż�������á���Ҫ���棬����׷��Ϊʲô������һ����ջ���Ϊ���ǵ��������ٴα������Ҳ�֪�����ܲ���������Щ������ϣ���㳤���������Ƭ�Լ��Ļ�԰�����������Ƿ��棬����Ҳ�ܿ���������Ҫ������һ��ú������̫����������ʱ���������Ź⣬�����ȡ�������������\r\n"
                    });
                }
                else if (visitedRose && !letterDeliveredToLuke && currentDialogue.Length > 1)
                {
                    letterDeliveredToLuke = true;
                    Debug.Log("Task4: �Ƴ���˿���ţ����С¬�˵���");
                    taskManager.inventoryManager.RemoveLetter("��˿����");
                    taskManager.inventoryManager.AddLetter(new Letter
                    {
                        title = "С¬�˵���",
                        content = "���裬�ҽ���������������ȥ���뿴����᲻�ᳪ�裬�ɴ������ţ���ֻ���������ò����������飬���ڸ����棡�Һ����ȥ������˵����ܻ��������ܳ�ȥ���ְ���Զ�����������Һ������������������ģ���֪�����Ĺ��¡��һ��˺ö໭���������з��ӣ�������Ц�����ӣ������Ҳ��ử�ְ֣���Ϊ��û����������ֻ֪��������˵����һ���ܺúܺõ��ˣ���������\r\n\r\n���������ﻭ�����ǲ��Ǻ���ѽ��������͵���㣬����һ��Ӣ����������ô����������ô����ɫ������������ɶ����˵���ǰְֵ����ӣ��ҳ���Ҳ��������������Ҿ����㻭����ã���������һ���򱶣�����ô�������������������Ұ��㣬���裬������õ����裬��������ħ��ʦ���ܰ���ɫ��ɺöණ�����ܲ��ܻ�����̫������ѽ������֪�����ǲ�������������ô����������ֻ�������￴������¬��\r\n"
                    });
                }
            }
            else
            {
                Debug.LogError("Task4: TaskManager �� InventoryManager δ��ȷ��");
            }

            PlayerController playerController = Object.FindObjectOfType<PlayerController>();
            if (playerController != null && playerController.IsInDialogue())
                playerController.EndDialogue();

            if (IsTaskComplete())
            {
                if (taskManager != null)
                {
                    Debug.Log("Task4: ������ɣ��л��� Task5");
                    Task5 newTask = gameObject.AddComponent<Task5>();
                    taskManager.SetTask(newTask);
                    newTask.SetupTask(taskManager, dialoguePanel, dialogueText, nextButton);
                    taskManager.UpdateTaskDisplay();
                }
                else
                {
                    Debug.LogError("Task4: TaskManager δ�ҵ����޷��л��� Task5");
                }
            }
        }
    }

    private string[] GetDialogueForRose()
    {
        return new string[]
        {
            "����˿����ѽ�������������ŵ�С�һ������ôӲ���ġ�",
            "����˿�����������İɣ���������ô���ģ�����������������ӡ�",
            "����˿����Ȼ�Һ�����Զ�����ݣ������ҵ�����ȥ�����������������������Ψһ�������ˡ�",
            "����˿�������ǳ��ƽٺ��ҵĺ����Ƕ������ˣ��еĳ������˹㲥�е�����������е�ʧ���ˣ���û��Ѷ���Һ���������Ϊ������",
            "����˿��������������Ҳû��ס�������������������һ����������С�ݡ�",
            "����˿��������ʱ���ҵ��ҵģ���������ֶ�ӣ�սǰ��û�ˣ�������һ��������Ͷ���ҡ�",
            "����˿�����Ӵ�ѧ�ӵ������ʱ������ʮ���꣬���Ÿ��ư����ݵ�����񡭡�",
            "����˿�����ڶ��������ˣ����ڴ�����Ҳ�������ˡ�",
            "����˿����ѽ������˵���ˣ�����Ҫ������������ˡ�",
            "����˿����ʵ��ûɶ�£���������������ͷ��Ÿ�С¬�ˡ��Ǻ��ӿɲ������ˣ���ս���г���������û�������ס��������˰�����",
            "����˿���Ǻ��Ӻ�����ǿ���е����������ӡ���",
            "����˿�����뾡�ҿ����������˽�һ���ʱ�����£�������û����̫���������ǲ�����˵���ˡ�",
            "����˿����ûд���֣��ͽ��ҡ��������ѡ�������������Ҹ������ţ���������������ô�������ӣ�������Ȥ��",
            "����˿���鷳����С�һ��ˡ�"
        };
    }

    private string[] GetDialogueForLuke()
    {
        return new string[]
        {
            "��С¬�ˡ��ۣ������㣡�ҵĻ��������ѣ����͵úÿ죬���Ÿ�����",
            "��������",
            "��С¬�ˡ��������ѽ��˺ö����Ķ�����ˮ�����ֽС��㡱����ʲô�����ǲ��ú�����",
            "��С¬�ˡ���Ȼ��û����̫�������һ��˺ö��������е�̫������������ѿ�����ϧ����˵̫��û�ˡ���",
            "��С¬�ˡ����������������˭������ô����ô�ණ��������������ʵ���и��²⡭��",
            "��С¬�ˡ������ˣ���ֻ������Ŷ����",
            "��С¬�ˡ��Ҳ����ǡ����Ұְ֣���һ����û���������ĸ���д���ء�",
            "��С¬�ˡ��������¸ң�������һ�����Ҿ���������ĺ����������ϸ��ҽ��ְֵĹ��£�˵����Զ��ע�����ҡ�",
            "��С¬�ˡ���֪�����ҺͰְ�����һģһ�������Ǵ�¬�ˣ�����С¬�ˡ�",
            "��С¬�ˡ������Ҹ�������ţ��鷳���͹�ȥ�ɣ�",
            "��С¬�ˡ��������������ڼ��Ϊɶ��ֱ�Ӹ����������Ҹо�����������˼һ�㣡",
            "��С¬�ˡ�������ͣ������϶����о�ϲ�С�",
            "��С¬�ˡ�ÿ������ְ��������ǻ��ѹ�����������������һ�㡣",
            "��С¬�ˡ����´λ������һ��˸������㣬��Ƥ�����ģ���˧�ˣ�",
            "��С¬�ˡ��´��л�����������������ѡ��㡱�ǳ�ʲô���ģ�"
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
            taskCompleteText.text = "����4������������";
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

            Debug.Log("����4 ��ʼ�������ʾ������");
        }
        else
        {
            Debug.LogWarning("������������������δ��ȷ��ʼ�����޷���ʾ����4��ʼ��ʾ��");
        }
    }
}