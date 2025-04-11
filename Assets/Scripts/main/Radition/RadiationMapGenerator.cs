using UnityEngine;

public class RadiationMapGenerator : MonoBehaviour
{
    public GameObject radiationZonePrefab; // ��������Ԥ����
    public Vector2 mapSize = new Vector2(40f, 40f); // ���ͼ��С
    public int numberOfZones = 20;         // �ܷ���������

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

            // �������������ֵ
            RadiationZone radiationZone = zone.GetComponent<RadiationZone>();
            radiationZone.maxRadiation = Random.Range(100f, 700f); // ÿ������Բ�ķ���ֵ���
            radiationZone.radius = 3f;

            // ͬ����ײ���뾶
            CircleCollider2D collider = zone.GetComponent<CircleCollider2D>();
            if (collider != null)
            {
                collider.radius = radiationZone.radius;
            }
        }
    }
}