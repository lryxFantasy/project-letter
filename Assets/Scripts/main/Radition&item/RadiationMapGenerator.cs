using UnityEngine;

public class RadiationMapGenerator : MonoBehaviour
{
    public GameObject radiationZonePrefab; // ��������Ԥ����
    public Vector2 mapSize = new Vector2(40f, 40f); // ���ͼ��С
    public int numberOfZones = 20; // �ܷ���������
    private Vector2 safeZoneCenter = new Vector2(-7.3f, -3.5f); // �޷����������ģ����� teleportPosition��
    public float safeZoneRadius = 5f; // �޷�������뾶

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

            // �������������ֵ
            RadiationZone radiationZone = zone.GetComponent<RadiationZone>();
            radiationZone.maxRadiation = Random.Range(100f, 700f); // ÿ������Բ�ķ���ֵ���
            radiationZone.radius = 4f;

            // ͬ����ײ���뾶
            CircleCollider2D collider = zone.GetComponent<CircleCollider2D>();
            if (collider != null)
            {
                collider.radius = radiationZone.radius;
            }
        }
    }

    // ��ȡһ�������޷��������ڵĺϷ�����λ��
    private Vector2 GetValidSpawnPosition()
    {
        int maxAttempts = 100; // ����Դ�������ֹ����ѭ��
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            Vector2 position = new Vector2(
                Random.Range(-mapSize.x / 2, mapSize.x / 2),
                Random.Range(-mapSize.y / 2, mapSize.y / 2)
            );

            // ���λ���Ƿ����޷���������
            if (!IsInSafeZone(position))
            {
                return position;
            }

            attempts++;
        }

        // ����ﵽ����Դ���������һ��Ĭ��λ�ã������ͼ��Ե��
        Debug.LogWarning("�޷��ҵ��Ϸ�������λ�ã�ʹ��Ĭ��λ�ã�");
        return new Vector2(mapSize.x / 2, mapSize.y / 2);
    }

    // ���λ���Ƿ����޷��������ڣ�Բ�Σ�
    private bool IsInSafeZone(Vector2 position)
    {
        float distance = Vector2.Distance(position, safeZoneCenter);
        return distance <= safeZoneRadius;
    }

    // ���ӻ��޷����������ڵ��ԣ�
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(safeZoneCenter, safeZoneRadius); // ����Բ���޷�������
    }
}