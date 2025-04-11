using UnityEngine;

public class RadiationMapGenerator : MonoBehaviour
{
    public GameObject radiationZonePrefab; // 辐射区域预制体
    public Vector2 mapSize = new Vector2(40f, 40f); // 大地图大小
    public int numberOfZones = 20;         // 总辐射区数量

    void Start()
    {
        GenerateRadiationMap();
    }

    void GenerateRadiationMap()
    {
        for (int i = 0; i < numberOfZones; i++)
        {
            Vector2 spawnPosition = new Vector2(
                Random.Range(-mapSize.x / 2, mapSize.x / 2),
                Random.Range(-mapSize.y / 2, mapSize.y / 2)
            );
            GameObject zone = Instantiate(radiationZonePrefab, spawnPosition, Quaternion.identity);

            // 随机设置最大辐射值
            RadiationZone radiationZone = zone.GetComponent<RadiationZone>();
            radiationZone.maxRadiation = Random.Range(100f, 700f); // 每个区域圆心辐射值随机
            radiationZone.radius = 3f;

            // 同步碰撞器半径
            CircleCollider2D collider = zone.GetComponent<CircleCollider2D>();
            if (collider != null)
            {
                collider.radius = radiationZone.radius;
            }
        }
    }
}