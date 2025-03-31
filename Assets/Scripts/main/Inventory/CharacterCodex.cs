using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterCodex : MonoBehaviour
{
    [SerializeField] private GameObject codexPanel;          // ��ͼ�����
    [SerializeField] private Button openButton;             // ��ͼ���İ�ť
    [SerializeField] private Button closeButton;            // �����رհ�ť
    [SerializeField] private GameObject characterInfoPanel; // ��ɫ��Ϣ���
    [SerializeField] private Button infoCloseButton;        // ��ɫ��Ϣ���رհ�ť

    // ���ﰴť����
    [SerializeField] private Button[] characterButtons = new Button[6];

    [SerializeField] private TextMeshProUGUI characterName; // ����������ʾ
    [SerializeField] private TextMeshProUGUI characterStory;// ���ﱳ��������ʾ

    private bool isMainPanelOpen = false;
    private bool isInfoPanelOpen = false;

    void Start()
    {
        // ��ʼ��ʱ�����������
        codexPanel.SetActive(false);
        characterInfoPanel.SetActive(false);

        // ��Ӱ�ť����
        openButton.onClick.AddListener(OpenCodex);
        closeButton.onClick.AddListener(CloseCodex);
        infoCloseButton.onClick.AddListener(CloseInfoPanel);

        // Ϊÿ����ɫ��ť��Ӽ���
        characterButtons[0].onClick.AddListener(() => ShowCharacterInfo(0));
        characterButtons[1].onClick.AddListener(() => ShowCharacterInfo(1));
        characterButtons[2].onClick.AddListener(() => ShowCharacterInfo(2));
        characterButtons[3].onClick.AddListener(() => ShowCharacterInfo(3));
        characterButtons[4].onClick.AddListener(() => ShowCharacterInfo(4));
        characterButtons[5].onClick.AddListener(() => ShowCharacterInfo(5));
    }

    void OpenCodex()
    {
        codexPanel.SetActive(true);
        isMainPanelOpen = true;
        Time.timeScale = 0f; // ��ͣʱ��
    }

    void CloseCodex()
    {
        codexPanel.SetActive(false);
        characterInfoPanel.SetActive(false); // ȷ����Ϣ���Ҳ�ر�
        isMainPanelOpen = false;
        isInfoPanelOpen = false;
        Time.timeScale = 1f; // �ָ�ʱ��
    }

    void CloseInfoPanel()
    {
        characterInfoPanel.SetActive(false);
        isInfoPanelOpen = false;
    }

    void ShowCharacterInfo(int characterIndex)
    {
        characterInfoPanel.SetActive(true);
        isInfoPanelOpen = true;

        switch (characterIndex)
        {
            case 0: // �ϱ� - ά���С�����
                characterName.text = "ά���С�����";
                characterStory.text = "��ݣ����۾��٣���������ʾ¼ս������ս���Ȳ��������ء�\n" +
                    "�Ը����ࡢ��Ĭ���ԣ���δ���ֱ���̬�ȣ����Ƽ�������Ȼ�����Լ��Ķ��ӡ�\n" +
                    "������\n      ά���С��������ǡ�̫��ս�����еľ��٣����������ʢ���ս�ۣ���ս���У�������������ս�ѵ��ڻ��ҵ�ǹ���У�������Ϊ���䲡ɥ�����������µ�һ������ʿ������С¬�ˡ���µĸ��ס������������Ⱥ�����ս�����ۺ󣬻ص����Ż�塣\n" +
                    "      ����Ĭ���ԣ�ʱ������һ�˾����ڷ����У������żᶨ���������¶������Կ�ʹ��ķ�ʽ����ÿ��������������Ϣ������������������ż�������ֽ�ŵ��¶ȣ������Ƽ��������ǻ���һ�еĵ�����ȴ���벻��ai��ʹ�������������������˹������Ψһ��Ŧ�������Ͷ��ӹ�ϵ�ȽϽ�Ӳ������ϲ������дʫ�����ֺ�����������飬���Ͷ���Ŀǰ�ֿ���ס�������������������ǣ���Ŷ��ӡ�\n" +
                    "д�ŷ��\n      ���ࡢ���ƣ�����û�ж�������Ρ��ôʼ����������һ��ֱ���˵�����ϲ������Ĩ�ǡ�";
                break;

            case 1: // ʫ�� - ������˹������
                characterName.text = "������˹������";
                characterStory.text = "��ݣ������ʫ�ˣ��ϱ��Ķ��ӣ����к�ƽ������ս����\n" +
                    "�Ը񣺸��ԡ�������������׷��������˼�����ɡ�\n" +
                    "������\n      ������˹���������ϱ�ά���С������Ķ��ӣ�����ս��׷ɵ��������С���Ÿ��׵�ս�ع��³���ĸ����Ϊ���䲡ȥ������ЩѪ�����Ļ��������������¶Ժ�ƽ�Ŀ�����\n" +
                    "      ���ܾ��̳и��׵ľ��˴�ͳ��ѡ����ʫ��Կ�ս������Ӱ������Ů�Ѽ򡤻��صİ���������������У����侲�������������ȵĸ�����ײ������˵�����Ĺ�ʽ�㲻���ҵ�ʫ��������Ҫ���Ĺ⡱���ڲ����£�����ÿһ��ʫ���֯���룬ϣ��������Ϊ���������������һĨ��⡣\n" +
                    "д�ŷ��\n      ����ʫ�⣬���������������������¶���������磬�����������б���Լ����������硣����������ÿһ���Ŷ�����ʫ����д���򡤻��ص��ż��У����������в������������㺢�����ı�Թ��";
                break;

            case 2: // ����ʦ - �򡤻���
                characterName.text = "�򡤻���";
                characterStory.text = "��ݣ��������ѧ����ʫ�˵�Ů�ѡ�\n" +
                    "�Ը��侲���ԣ����ó������У������ڿƼ����ó�����һЩ�Ƽ���Ʒ��\n" +
                    "������\n      �򡤻�����һλ�������ѧ����ս���ݻ�������У԰����ȡԶ����ĸ��˿�Ľ����������һ���ս��ıʼǱ��ӵ����Ż�塣\n" +
                    "      ���ó��òд���������ʵ�õ�С�Ƽ���Ʒ�����侲�����ԣ������ڽ�����⣬���ſƼ�����������á����ڷ��䣬�������ˡ���ʫ��������˹�������޷�ʱ�����棬���������ƻ𣬶���ȴ���ó�������Ц��˵�������ɱ�ʫ��ʵ�ڡ������ս���ĺƽ�����Ϊ����������һ�����ֹ���Ʒ�ػ����ӣ�����Ŀ��򵥶��ᶨ���ÿƼ���΢�⣬��ů�����������硣\n" +
                    "д�ŷ��\n      �߼�������ƫ�����ԣ���������ʵ�鱨�档���Լ��������Ӳ���̫�����ɫ�ʣ�����ͨ��ϸ����¶���ġ����������и���һЩ�ֻ��ͼ���豸����ƻ���С�����Ľ��ȱ��档��д��������˹���������һ�֡������Կ��Ƹ��ԡ���΢��������";
                break;

            case 3: // ������ - ��˿
                characterName.text = "��˿";
                characterStory.text = "��ݣ�һλ��ͨ���ϸ��ˣ����Զ����ĸ��С¬�˵����ر��ѡ�\n" +
                    "�Ը����ᡢ���أ�ӵ��һ�ű������ġ�\n" +
                    "������\n      ��˿��һλ��ͨ���ϸ��ˣ�ס���Ż���Ե��С������ƽ��ȴ������������ص��ģ������������ڷ��䲡������Ҳ��ս��������з��룬������������Բ����µĹ¼š�ÿÿ�뵽�Լ���ȥ�����ӣ����ܻᱯ�ˣ������Ӳ����磬�����ܴ����ֹ۵���������С¬�ˡ����ס���һλ��δı��ĺ��ӳ�Ϊ���������ر��ѡ�\n" +
                    "      С¬��Ҳʧȥ�˸��ף���˿������д����ů�Ļ����սǰ�Ļ����������ĸ���������Щ��ȥ�����ã������������Լ�����ݣ��������ϵ��Լ��뺢���д�����ֻ�ԡ��������ѡ�Ϊ���������Ž����¸ҡ����������������Լ����ӵ�Ӱ�ӡ���˿��ʧȥ���ˣ�ȴ���ǻ��밮֧����������ӣ��ڲ����������У�����¬�˿��������ػ��ߣ���ÿ���ű�֯ϣ����΢�⡣\n" +
                    "д�ŷ��\n      ��ů�����й����ԣ�������ͣ������м����������Ϣ������һ�־�ʱ�������ź����á������ﱯ�ˣ����Ǵ���ϣ������ů��ʱ���ἰ��ȥ������ʱ�⡣��д��С¬�˵����У�ʹ�á��������ѡ���������";
                break;

            case 4: // С¬�ˡ����
                characterName.text = "С¬�ˡ����";
                characterStory.text = "��ݣ���ܽ�Ķ��ӣ������ָ��к����ġ�\n" +
                    "�Ը����桢�ֹۣ����ǿ���̽�����硣\n" +
                    "������\n      ���ĸ�����ս����������ĸ��Ϊ�˼�������������ȡ�����׵�ͬ����С¬�˴�δ�������ס�����֪���Լ��ı�����˭��ֻ֪���Ǹ����������ѡ����ܻ��ţ�����������ȥ�Ĺ��¡��������ų�Ϊ��ѧ�ң��޸����ƻ������磬����\n" +
                    "д�ŷ��\n      ����ͯȤ���ʾ���Ծ����ʱ�﷨���ҵ�����ɰ������ÿ��ŵı��������г���������ĺ��棬������һ�������ʺš���д�����������ѡ������У�����ޱ����ر���Լ���������";
                break;

            case 5: // ���� - ��ܽ�����
                characterName.text = "��ܽ�����";
                characterStory.text = "��ݣ������ң����ң�С¬�˵�ĸ�ף��ɫ���봴�졣\n" +
                    "�Ը񣺸��ԡ�ִ�ţ�����ʵ���ε���Ը������\n" +
                    "������\n      ��ܽ��������Ż��Ļ��ң�һλ�ɫ���봴��ĵ���ĸ�ף������ɷ��ڡ�̫��ս������ɥ����������������С¬�ˡ���������Ϊ������ͨ�������˴ӷ������Ѽ���������֭����������Ϊ¬�˻�һ����û��ս�������족��\n" +
                    "      ��ܽ���Զ�ִ�ţ����ڶ���˯�º�ҹ������������������������ÿһĨɫ���ػ������밮����¬������������͵�������\n" +
                    "д�ŷ��\n      ��������������ɫ�ʸУ���������滭�档������ϸ��ıʴ������ճ���������һ�ٷ���һ�ȴ���ķ羰������д�ø�������������������һЩ��д��";
                break;
        }
    }

    void Update()
    {
        // ��ESC���ر����
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isInfoPanelOpen)
            {
                CloseInfoPanel();
            }
            else if (isMainPanelOpen)
            {
                CloseCodex();
            }
        }
    }
}