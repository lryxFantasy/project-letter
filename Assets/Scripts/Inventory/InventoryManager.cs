using UnityEngine;
using System.Collections.Generic;
using TMPro; // ���� TextMeshPro �����ռ�

public class InventoryManager : MonoBehaviour
{
    public List<Letter> letters = new List<Letter>(); // �ż��б�
    public GameObject itemSlotPrefab;                 // �ż���Ԥ����
    public Transform gridTransform;                   // ����� Transform
    public GameObject letterDetailPanel;              // �ż��������
    public TMP_Text letterContentText;                // ʹ�� TMP_Text 
    
    void Start()
    {
        // ��ʼ��һЩ�����ż�
        letters.Add(new Letter { title = "�ż�1", content = "���ǵ�һ���ŵ�����", icon = null });
        letters.Add(new Letter { title = "�ż�2", content = "���ǵڶ����ŵ�����", icon = null });
        letters.Add(new Letter { title = "�ż�3", content = "���ǵ������ŵ�����", icon = null });
        letters.Add(new Letter { title = "�ż�4", content = "���ǵ��ķ��ŵ�����", icon = null });
        letters.Add(new Letter { title = "�ż�5", content = "���ǵ�����ŵ�����", icon = null });
        letters.Add(new Letter { title = "�ż�6", content = "���ǵ������ŵ�����", icon = null });
        letters.Add(new Letter { title = "�ż�7", content = "���ǵ��߷��ŵ�����", icon = null });
        letters.Add(new Letter { title = "�ż�8", content = "���ǵڰ˷��ŵ�����", icon = null });
        letters.Add(new Letter { title = "�ż�9", content = "���ǵھŷ��ŵ�����", icon = null });
        letters.Add(new Letter { title = "�ż�10", content = "���ǵ�ʮ���ŵ�����", icon = null });
        letters.Add(new Letter { title = "�ż�11", content = "���ǵ�ʮһ���ŵ�����", icon = null });
        letters.Add(new Letter { title = "�ż�12", content = "���ǵ�ʮ�����ŵ�����", icon = null });
        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        // ������в�
        foreach (Transform child in gridTransform)
        {
            Destroy(child.gameObject);
        }

        // Ϊÿ��������һ����
        foreach (Letter letter in letters)
        {
            GameObject slot = Instantiate(itemSlotPrefab, gridTransform);
            slot.GetComponentInChildren<TMP_Text>().text = letter.title; 
            UnityEngine.UI.Button button = slot.GetComponent<UnityEngine.UI.Button>();
            button.onClick.AddListener(() => ShowLetterDetail(letter));
        }
    }

    public void ShowLetterDetail(Letter letter)
    {
        letterDetailPanel.SetActive(true);
        letterContentText.text = letter.content;
    }
}