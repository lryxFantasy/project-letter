using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // 主角的 Transform
    public Vector3 offset;   // 摄像机相对于主角的偏移量
    public Vector3[] housePositions; // 存储六个房屋的相机位置
    public Vector3[] housePlayerPositions; // 存储六个房屋的主角传送位置
    private bool isIndoors = false;  // 是否在房屋内
    private int currentHouseIndex = -1; // 当前房屋的索引，-1 表示在地图上
    private Vector3 mapPosition;     // 地图的默认位置
    private Vector3 lastPlayerMapPosition; // 主角在地图上的最后位置（用于退出时传送）

    void Start()
    {
        // 初始化房屋位置
        if (housePositions == null || housePositions.Length == 0)
        {
            housePositions = new Vector3[6];
            housePositions[0] = new Vector3(100, 0, -10);  // 房屋 1
            housePositions[1] = new Vector3(200, 0, -10);  // 房屋 2
            housePositions[2] = new Vector3(300, 0, -10);  // 房屋 3
            housePositions[3] = new Vector3(400, 0, -10);  // 房屋 4
            housePositions[4] = new Vector3(500, 0, -10);  // 房屋 5
            housePositions[5] = new Vector3(600, 0, -10);  // 房屋 6
        }

        // 初始化主角在房屋内的传送位置
        if (housePlayerPositions == null || housePlayerPositions.Length == 0)
        {
            housePlayerPositions = new Vector3[6];
            housePlayerPositions[0] = new Vector3(100, 0, 0);  // 房屋 1 主角位置
            housePlayerPositions[1] = new Vector3(200, 0, 0);  // 房屋 2 主角位置
            housePlayerPositions[2] = new Vector3(300, 0, 0);  // 房屋 3 主角位置
            housePlayerPositions[3] = new Vector3(400, 0, 0);  // 房屋 4 主角位置
            housePlayerPositions[4] = new Vector3(500, 0, 0);  // 房屋 5 主角位置
            housePlayerPositions[5] = new Vector3(600, 0, 0);  // 房屋 6 主角位置
        }

        mapPosition = transform.position; // 保存初始位置作为地图位置
    }

    void LateUpdate()
    {
        if (target != null && !isIndoors)
        {
            // 相机跟随主角（地图模式）
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z) + offset;
            transform.position = targetPosition;
            lastPlayerMapPosition = target.position; // 记录主角在地图上的位置
        }
    }

    // 进入房屋
    public void EnterHouse(int houseIndex)
    {
        if (houseIndex >= 0 && houseIndex < housePositions.Length)
        {
            transform.position = housePositions[houseIndex]; // 移动相机
            target.position = housePlayerPositions[houseIndex]; // 传送主角
            isIndoors = true;
            currentHouseIndex = houseIndex;
        }
    }

    // 退出房屋
    public void ExitHouse()
    {
        if (isIndoors)
        {
            isIndoors = false;
            currentHouseIndex = -1;
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z) + offset;
            target.position = lastPlayerMapPosition; // 传送主角回到地图上的最后位置
            target.position = new Vector3(target.position.x, target.position.y - 1f, target.position.z); // 向下移动 2 个单位
        }
    }

    // 检查是否在房屋内
    public bool IsIndoors()
    {
        return isIndoors;
    }
}