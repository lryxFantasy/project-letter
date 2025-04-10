using UnityEngine;
using TMPro;

public class RadiationZone : MonoBehaviour
{
    public float maxRadiation = 500f;   // 圆心最大辐射强度 (μSv/h)
    public float radius = 3f;           // 辐射区域半径
    private float radiationStrength;    // 当前位置的辐射强度（动态计算）
    private RubyController player;      // 玩家引用
    private TextMeshProUGUI radiationText; // UI显示辐射值
    public SpriteRenderer radiationSprite; // 辐射区域的精灵

    public float moveInterval = 2f;     // 移动时间间隔
    public float moveSpeed = 1f;        // 移动速度
    private Vector2 targetPosition;     // 目标位置
    private float moveTimer;

    private static float totalRadiation; // 全局总辐射值
    private float damageAccumulator = 0f; // 累积伤害
    private float damageTimer = 0f;      // 每秒计时器

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
            Debug.LogError("RadiationText 未找到！请确保场景中有名为 'RadiationText' 的 TextMeshProUGUI 对象。");
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
        // 每帧检查玩家是否在室内并更新辐射值
        if (player != null && player.cameraController != null && player.cameraController.IsIndoors())
        {
            totalRadiation = 0f; // 如果玩家在室内，重置辐射值
            if (radiationText != null)
            {
                radiationText.text = "辐射值：0.0";
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


            // 如果玩家已经死亡，不计算伤害
            if (player.health <= 0)
            {
                return;
            }

            UpdateTotalRadiation();
            float damagePerSecond = totalRadiation / 50f; // 伤害系数
            damageAccumulator += damagePerSecond * Time.deltaTime; // 累积伤害
            damageTimer += Time.deltaTime; // 计时器增加

            if (damageTimer >= 1f) // 每秒检查一次
            {
                int damageToApply = (int)damageAccumulator;
                Debug.Log($"Applying damage: {damageToApply}, Total Radiation: {totalRadiation}, Accumulated Damage: {damageAccumulator}");
                player.ChangeHealth(-damageToApply, true); // 标记为辐射伤害
                damageAccumulator = 0f; // 重置累积伤害
                damageTimer = 0f; // 重置计时器
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
            damageAccumulator = 0f; // 重置累积伤害
            damageTimer = 0f; // 重置计时器
            UpdateTotalRadiation();
            if (radiationText != null && totalRadiation == 0)
                radiationText.text = "辐射值：0.0";
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

        // 使用平方函数实现先快后慢的平滑曲线
        float normalizedDistance = distance / radius; // 归一化距离 (0 到 1)
        float radiationFactor = 1 - (normalizedDistance * normalizedDistance); // 平方衰减
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
            radiationText.text = $"辐射值：{totalRadiation:F1}";
        }
    }
}