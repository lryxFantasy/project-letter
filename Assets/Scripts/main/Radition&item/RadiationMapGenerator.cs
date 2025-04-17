using UnityEngine;

public class RadiationMapGenerator : MonoBehaviour
{
    public GameObject radiationZonePrefab; // 辐射区域预制体
    public Vector2 mapSize = new Vector2(40f, 40f); // 大地图大小
    public int numberOfZones = 20; // 总辐射区数量
    private Vector2 safeZoneCenter = new Vector2(-7.3f, -3.5f); // 无辐射区域中心（基于 teleportPosition）
    public float safeZoneRadius = 5f; // 无辐射区域半径

    void Start()
    {
        GenerateRadiationMap();
    }

    void GenerateRadiationMap()
    {
        for (int i = 0; i < numberOfZones; i++)
        {
            Vector2 spawnPosition = GetValidSpawnPosition();
            GameObject zone = Instantiate(radiationZonePrefab, spawnPosition, Quaternion.identity);

            // 随机设置最大辐射值
            RadiationZone radiationZone = zone.GetComponent<RadiationZone>();
            radiationZone.maxRadiation = Random.Range(100f, 700f); // 每个区域圆心辐射值随机
            radiationZone.radius = 4f;

            // 同步碰撞器半径
            CircleCollider2D collider = zone.GetComponent<CircleCollider2D>();
            if (collider != null)
            {
                collider.radius = radiationZone.radius;
            }
        }
    }

    // 获取一个不在无辐射区域内的合法生成位置
    private Vector2 GetValidSpawnPosition()
    {
        int maxAttempts = 100; // 最大尝试次数，防止无限循环
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            Vector2 position = new Vector2(
                Random.Range(-mapSize.x / 2, mapSize.x / 2),
                Random.Range(-mapSize.y / 2, mapSize.y / 2)
            );

            // 检查位置是否在无辐射区域外
            if (!IsInSafeZone(position))
            {
                return position;
            }

            attempts++;
        }

        // 如果达到最大尝试次数，返回一个默认位置（例如地图边缘）
        Debug.LogWarning("无法找到合法的生成位置，使用默认位置！");
        return new Vector2(mapSize.x / 2, mapSize.y / 2);
    }

    // 检查位置是否在无辐射区域内（圆形）
    private bool IsInSafeZone(Vector2 position)
    {
        float distance = Vector2.Distance(position, safeZoneCenter);
        return distance <= safeZoneRadius;
    }

    // 可视化无辐射区域（用于调试）
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(safeZoneCenter, safeZoneRadius); // 绘制圆形无辐射区域
    }
}