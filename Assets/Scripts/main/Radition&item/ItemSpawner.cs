using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject batteryPrefab; // 电池预制体
    [SerializeField] private GameObject shieldPrefab; // 护盾预制体
    [SerializeField] private int maxBatteries = 15; // 最大电池数量
    [SerializeField] private int maxShields = 7; // 最大护盾数量
    [SerializeField] private BoxCollider2D[] spawnZones; // 可生成道具的区域
    [SerializeField] private float checkRadius = 0.3f; // 碰撞检测半径
    [SerializeField] private LayerMask obstacleLayer; // 障碍物图层
    [SerializeField] private float spawnInterval = 10f; // 刷新间隔（秒）
    [SerializeField] private int maxItemsPerZone = 2; // 每区域最大道具数
    private CameraController cameraController;
    private float spawnTimer = 0f;
    private int currentBatteries = 0;
    private int currentShields = 0;
    private Dictionary<BoxCollider2D, int> zoneItemCounts; // 跟踪每区域总道具数

    void Start()
    {
        cameraController = FindObjectOfType<CameraController>();
        zoneItemCounts = new Dictionary<BoxCollider2D, int>();

        if (spawnZones == null || spawnZones.Length == 0)
        {
            return;
        }

        foreach (var zone in spawnZones)
        {
            if (zone != null)
            {
                zoneItemCounts[zone] = 0;
            }

        }

        // 初始生成
        int totalItems = 0;
        while (totalItems < maxBatteries + maxShields)
        {
            Item.ItemType type = (currentBatteries < maxBatteries && (currentShields >= maxShields || Random.value < 0.6f))
                ? Item.ItemType.Battery
                : Item.ItemType.Shield;
            if (type == Item.ItemType.Battery && currentBatteries >= maxBatteries) continue;
            if (type == Item.ItemType.Shield && currentShields >= maxShields) continue;
            if (TrySpawnItem(type))
            {
                totalItems++;
            }
            else
            {
                break;
            }
        }
    }

    void Update()
    {
        if (cameraController != null && cameraController.IsIndoors()) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            if (currentBatteries < maxBatteries)
            {
                TrySpawnItem(Item.ItemType.Battery);
            }
            if (currentShields < maxShields)
            {
                TrySpawnItem(Item.ItemType.Shield);
            }
            spawnTimer = 0f;
        }
    }

    private bool TrySpawnItem(Item.ItemType type)
    {
        Vector2 spawnPos = GetValidSpawnPosition();
        if (spawnPos != Vector2.zero)
        {
            GameObject prefab = type == Item.ItemType.Battery ? batteryPrefab : shieldPrefab;
            if (prefab == null)
            {
                return false;
            }
            Instantiate(prefab, spawnPos, Quaternion.identity);
            if (type == Item.ItemType.Battery)
            {
                currentBatteries++;
            }
            else
            {
                currentShields++;
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    private Vector2 GetValidSpawnPosition()
    {
        if (spawnZones == null || spawnZones.Length == 0)
        {
            return Vector2.zero;
        }

        var sortedZones = spawnZones
            .Where(z => z != null && zoneItemCounts.ContainsKey(z) && zoneItemCounts[z] < maxItemsPerZone)
            .OrderBy(z => zoneItemCounts[z])
            .ThenBy(_ => Random.value)
            .ToArray();

        if (sortedZones.Length == 0)
        {
            return Vector2.zero;
        }

        foreach (BoxCollider2D zone in sortedZones)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector2 randomPoint = GetRandomPointInBounds(zone.bounds);
                if (!Physics2D.OverlapCircle(randomPoint, checkRadius, obstacleLayer))
                {
                    zoneItemCounts[zone]++;
                    return randomPoint;
                }
            }
        }

        return Vector2.zero;
    }

    private Vector2 GetRandomPointInBounds(Bounds bounds)
    {
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector2(x, y);
    }

    public void OnItemPickedUp(Item.ItemType type, Vector2 itemPos)
    {
        if (type == Item.ItemType.Battery)
        {
            currentBatteries = Mathf.Max(0, currentBatteries - 1);
        }
        else if (type == Item.ItemType.Shield)
        {
            currentShields = Mathf.Max(0, currentShields - 1);
        }

        // 尝试精确匹配区域
        bool foundZone = false;
        foreach (var zone in spawnZones)
        {
            if (zone != null && zoneItemCounts.ContainsKey(zone) && zoneItemCounts[zone] > 0 && zone.bounds.Contains(itemPos))
            {
                zoneItemCounts[zone]--;
                foundZone = true;
                break;
            }
        }

        // 如果未找到区域，尝试找到最近的区域
        if (!foundZone)
        {
            BoxCollider2D closestZone = null;
            float minDistance = float.MaxValue;
            foreach (var zone in spawnZones)
            {
                if (zone != null && zoneItemCounts.ContainsKey(zone) && zoneItemCounts[zone] > 0)
                {
                    float distance = Vector2.Distance(itemPos, zone.bounds.center);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestZone = zone;
                    }
                }
            }

            if (closestZone != null)
            {
                zoneItemCounts[closestZone]--;
            }

        }

        // 打印所有区域状态
        foreach (var zone in spawnZones)
        {
            if (zone != null && zoneItemCounts.ContainsKey(zone))
            {
            }
        }
    }

    void OnDrawGizmos()
    {
        if (spawnZones == null) return;
        foreach (BoxCollider2D zone in spawnZones)
        {
            if (zone != null)
            {
                Gizmos.color = zoneItemCounts != null && zoneItemCounts.ContainsKey(zone) && zoneItemCounts[zone] >= maxItemsPerZone ? Color.red : Color.green;
                Gizmos.DrawWireCube(zone.bounds.center, zone.bounds.size);
                if (Application.isEditor && zoneItemCounts != null && zoneItemCounts.ContainsKey(zone))
                {
                    UnityEditor.Handles.Label(zone.bounds.center, $"Items: {zoneItemCounts[zone]}/{maxItemsPerZone}");
                }
            }
        }
    }
}