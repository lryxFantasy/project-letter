using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public List<Letter> letters = new List<Letter>(); // �ż��б�
    public GameObject itemSlotPrefab;                 // �ż���Ԥ����
    public Transform gridTransform;                   // ����� Transform
    public GameObject letterDetailPanel;              // �ż��������
    public TMP_Text letterContentText;                // ʹ�� TMP_Text 

    void Start()
    {
        // ��ʼ��ʱ����Ӳ����ż������յȴ��������
        UpdateInventoryUI();
    }

    // ����ż�������
    public void AddLetter(Letter letter)
    {
        letters.Add(letter);
        UpdateInventoryUI();
    }

    // ���ݱ����Ƴ��ż�
    public void RemoveLetter(string title)
    {
        Letter letterToRemove = letters.Find(l => l.title == title);
        if (letterToRemove != null)
        {
            letters.Remove(letterToRemove);
            UpdateInventoryUI();
        }
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

            // ���ñ���
            TMP_Text titleText = slot.GetComponentInChildren<TMP_Text>();
            if (titleText != null)
            {
                titleText.text = letter.title;
            }

            // ����ͼ��
            UnityEngine.UI.Image iconImage = slot.transform.Find("Icon")?.GetComponent<UnityEngine.UI.Image>();
            if (iconImage != null)
            {
                iconImage.sprite = letter.icon;
                iconImage.gameObject.SetActive(letter.icon != null); // ���û��ͼ�꣬���� Image
            }
            else
            {
                Debug.LogWarning("ItemSlotPrefab ��δ�ҵ���Ϊ 'Icon' �� Image �����");
            }

            // ���ð�ť����¼�
            UnityEngine.UI.Button button = slot.GetComponent<UnityEngine.UI.Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => ShowLetterDetail(letter));
            }
        }
    }

    public void ShowLetterDetail(Letter letter)
    {
        letterDetailPanel.SetActive(true);
        letterContentText.text = letter.content;
    }
}