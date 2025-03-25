using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public int taskNumber;                  // 当前任务编号
    public Vector3 playerPosition;          // 玩家位置
    public List<Letter> letters;            // 背包信件
    public string taskStateJson;            // 当前任务的状态
    public bool isIndoors;                  // 是否在房屋内
    public int currentHouseIndex;           // 当前房屋索引
    public Vector3 lastPlayerMapPosition;   // 地图上最后位置
}