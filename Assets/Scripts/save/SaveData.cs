using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public int taskNumber;                  // ��ǰ������
    public Vector3 playerPosition;          // ���λ��
    public List<Letter> letters;            // �����ż�
    public string taskStateJson;            // ��ǰ�����״̬
    public bool isIndoors;                  // �Ƿ��ڷ�����
    public int currentHouseIndex;           // ��ǰ��������
    public Vector3 lastPlayerMapPosition;   // ��ͼ�����λ��
}