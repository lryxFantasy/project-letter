using UnityEngine;
using UnityEngine.UI; // ����Image���

public class NPCImageLoader : MonoBehaviour
{
    [SerializeField] private Image npcImage; // ������ʾNPCͼƬ��UI Image���
    [SerializeField] private PlayerController playerController; // ����PlayerController�Ի�ȡ��ǰNPC

    private string currentNPCRole; // ��ǰNPC�Ľ�ɫ����

    void Start()
    {
        // ���δ��Inspector�и�ֵ�������Զ��������
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
            if (playerController == null)
            {
                Debug.LogError("NPCImageLoader: δ�ҵ� PlayerController��");
            }
        }

        if (npcImage == null)
        {
            Debug.LogError("NPCImageLoader: ����Inspector�и�ֵ npcImage��");
        }

        // ��ʼ��ʱ����ͼƬ
        if (npcImage != null)
        {
            npcImage.enabled = false;
        }
    }

    void Update()
    {
        // ��ȡ��ǰNPC��ɫ
        string newNPCRole = playerController?.GetCurrentNPCRole();

        // ����ڶԻ��У�ȷ������ͼƬ
        if (playerController.IsInDialogue() && !string.IsNullOrEmpty(newNPCRole))
        {
            if (newNPCRole != currentNPCRole || !npcImage.enabled) // NPC�仯��ͼƬδ��ʾʱ����
            {
                currentNPCRole = newNPCRole;
                LoadNPCImage(currentNPCRole);
            }
        }
        else if (!playerController.IsInDialogue() && npcImage.enabled)
        {
            // ������ڶԻ��У�����ͼƬ
            npcImage.enabled = false;
            currentNPCRole = null; // ���õ�ǰNPC��ɫ
        }
    }

    // ����NPC���Ƽ���ͼƬ
    private void LoadNPCImage(string npcRole)
    {
        if (string.IsNullOrEmpty(npcRole) || npcImage == null)
        {
            Debug.LogWarning("NPCImageLoader: NPC��ɫ����Ϊ�ջ�Image���δ��ֵ");
            return;
        }

        // ��Resources�ļ��м���Sprite�������ļ�����npcRoleһ��
        Sprite npcSprite = Resources.Load<Sprite>($"NPCImages/{npcRole}");
        if (npcSprite != null)
        {
            npcImage.sprite = npcSprite;
            npcImage.enabled = true; // ��ʾͼƬ
            Debug.Log($"NPCImageLoader: �Ѽ��� {npcRole} ��ͼƬ");
        }
        else
        {
            npcImage.enabled = false; // ����ͼƬ
            Debug.LogWarning($"NPCImageLoader: δ�ҵ� {npcRole} ��ͼƬ������Resources/NPCImages�ļ���");
        }
    }
}