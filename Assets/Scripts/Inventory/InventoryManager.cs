using UnityEngine;
using System.Collections.Generic;
using TMPro; // 引入 TextMeshPro 命名空间

public class InventoryManager : MonoBehaviour
{
    public List<Letter> letters = new List<Letter>(); // 信件列表
    public GameObject itemSlotPrefab;                 // 信件槽预制体
    public Transform gridTransform;                   // 网格的 Transform
    public GameObject letterDetailPanel;              // 信件详情面板
    public TMP_Text letterContentText;                // 使用 TMP_Text 
    
    void Start()
    {
        // 初始化一些测试信件
        letters.Add(new Letter { title = "信件1", content = "这是第一封信的内容", icon = null });
        letters.Add(new Letter { title = "信件2", content = "这是第二封信的内容", icon = null });
        letters.Add(new Letter { title = "信件3", content = "这是第三封信的内容", icon = null });
        letters.Add(new Letter { title = "信件4", content = "这是第四封信的内容", icon = null });
        letters.Add(new Letter { title = "信件5", content = "这是第五封信的内容", icon = null });
        letters.Add(new Letter { title = "信件6", content = "这是第六封信的内容", icon = null });
        letters.Add(new Letter { title = "信件7", content = "这是第七封信的内容", icon = null });
        letters.Add(new Letter { title = "信件8", content = "这是第八封信的内容", icon = null });
        letters.Add(new Letter { title = "信件9", content = "这是第九封信的内容", icon = null });
        letters.Add(new Letter { title = "信件10", content = "这是第十封信的内容", icon = null });
        letters.Add(new Letter { title = "信件11", content = "这是第十一封信的内容", icon = null });
        letters.Add(new Letter { title = "信件12", content = "这是第十二封信的内容", icon = null });
        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        // 清空现有槽
        foreach (Transform child in gridTransform)
        {
            Destroy(child.gameObject);
        }

        // 为每封信生成一个槽
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