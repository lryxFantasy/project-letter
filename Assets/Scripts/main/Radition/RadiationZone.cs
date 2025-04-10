using UnityEngine;
using TMPro;

public class RadiationZone : MonoBehaviour
{
    public float maxRadiation = 500f;   // Բ��������ǿ�� (��Sv/h)
    public float radius = 3f;           // ��������뾶
    private float radiationStrength;    // ��ǰλ�õķ���ǿ�ȣ���̬���㣩
    private RubyController player;      // �������
    private TextMeshProUGUI radiationText; // UI��ʾ����ֵ
    public SpriteRenderer radiationSprite; // ��������ľ���

    public float moveInterval = 2f;     // �ƶ�ʱ����
    public float moveSpeed = 1f;        // �ƶ��ٶ�
    private Vector2 targetPosition;     // Ŀ��λ��
    private float moveTimer;

    private static float totalRadiation; // ȫ���ܷ���ֵ
    private float damageAccumulator = 0f; // �ۻ��˺�
    private float damageTimer = 0f;      // ÿ���ʱ��

    void Start()
    {
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            collider.radius = radius;
        }

        radiationText = GameObject.Find("RadiationText")?.GetComponent<TextMeshProUGUI>();
        if (radiationText == null)
        {
            Debug.LogError("RadiationText δ�ҵ�����ȷ������������Ϊ 'RadiationText' �� TextMeshProUGUI ����");
        }

        SetNewTargetPosition();
        UpdateSpriteColor();

        if (radiationSprite != null)
            radiationSprite.enabled = false;
    }

    void Update()
    {
        moveTimer -= Time.deltaTime;
        if (moveTimer <= 0)
        {
            SetNewTargetPosition();
            moveTimer = moveInterval;
        }
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.Q) && radiationSprite != null)
        {
            radiationSprite.enabled = true;
        }
        else if (radiationSprite != null)
        {
            radiationSprite.enabled = false;
        }
        // ÿ֡�������Ƿ������ڲ����·���ֵ
        if (player != null && player.cameraController != null && player.cameraController.IsIndoors())
        {
            totalRadiation = 0f; // �����������ڣ����÷���ֵ
            if (radiationText != null)
            {
                radiationText.text = "����ֵ��0.0";
            }
        }
    }

    void SetNewTargetPosition()
    {
        float moveRange = 2f;
        targetPosition = (Vector2)transform.position + new Vector2(
            Random.Range(-moveRange, moveRange),
            Random.Range(-moveRange, moveRange)
        );
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<RubyController>();
            Debug.Log("Player entered radiation zone");
            UpdateTotalRadiation();
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && player != null)
        {


            // �������Ѿ��������������˺�
            if (player.health <= 0)
            {
                return;
            }

            UpdateTotalRadiation();
            float damagePerSecond = totalRadiation / 50f; // �˺�ϵ��
            damageAccumulator += damagePerSecond * Time.deltaTime; // �ۻ��˺�
            damageTimer += Time.deltaTime; // ��ʱ������

            if (damageTimer >= 1f) // ÿ����һ��
            {
                int damageToApply = (int)damageAccumulator;
                Debug.Log($"Applying damage: {damageToApply}, Total Radiation: {totalRadiation}, Accumulated Damage: {damageAccumulator}");
                player.ChangeHealth(-damageToApply, true); // ���Ϊ�����˺�
                damageAccumulator = 0f; // �����ۻ��˺�
                damageTimer = 0f; // ���ü�ʱ��
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
            damageAccumulator = 0f; // �����ۻ��˺�
            damageTimer = 0f; // ���ü�ʱ��
            UpdateTotalRadiation();
            if (radiationText != null && totalRadiation == 0)
                radiationText.text = "����ֵ��0.0";
        }
    }

    private void UpdateSpriteColor()
    {
        if (radiationSprite != null)
        {
            radiationSprite.color = Color.Lerp(Color.green, Color.red, maxRadiation / 500f);
        }
    }

    private float CalculateRadiation(Vector2 playerPosition)
    {
        float distance = Vector2.Distance(transform.position, playerPosition);
        if (distance >= radius) return 0f;

        // ʹ��ƽ������ʵ���ȿ������ƽ������
        float normalizedDistance = distance / radius; // ��һ������ (0 �� 1)
        float radiationFactor = 1 - (normalizedDistance * normalizedDistance); // ƽ��˥��
        float radiation = maxRadiation * radiationFactor;

        Debug.Log($"Distance: {distance}, Normalized: {normalizedDistance}, Radiation: {radiation}");
        return Mathf.Max(radiation, 0f);
    }

    private void UpdateTotalRadiation()
    {
        if (player == null) return;

        totalRadiation = 0f;
        RadiationZone[] allZones = FindObjectsOfType<RadiationZone>();
        foreach (RadiationZone zone in allZones)
        {
            if (zone.player != null)
            {
                totalRadiation += zone.CalculateRadiation(player.transform.position);
            }
        }

        if (radiationText != null)
        {
            radiationText.text = $"����ֵ��{totalRadiation:F1}";
        }
    }
}