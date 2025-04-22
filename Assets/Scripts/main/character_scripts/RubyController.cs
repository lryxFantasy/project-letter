using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    public int maxHealth = 100; // 最大血量设为100
    public float timeInvincible = 2.0f;
    public Slider healthBar; // 血量条UI组件
    public TextMeshProUGUI healthText;
    public bool pauseHealthUpdate = false; // 新增变量，用于暂停血量更新

    public GameObject deathPanel; // 在 Inspector 中拖入死亡面板
    public Button continueButton; // 在 Inspector 中拖入继续按钮

    public int health { get { return currentHealth; } }
    int currentHealth;

    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    public GameObject projectilePrefab;
    private PlayerController playerController;
    public CameraController cameraController; // 用于判断室内外状态
    private Vector3 lastHousePosition; // 记录最后进入的房子位置
    private Vector3 teleportPosition = new Vector3(-7.3f, -2.5f, -6.1f);
    private bool isShieldActive = false; // 护盾状态，阻止所有伤害
    private float shieldTimer = 0f; // 剩余护盾时间
    [SerializeField] private SpriteRenderer shieldSprite; // 护盾精灵，显示护盾效果
    private WeatherManager weatherManager; // WeatherManager 引用

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        playerController = GetComponent<PlayerController>();
        cameraController = FindObjectOfType<CameraController>();
        weatherManager = FindObjectOfType<WeatherManager>(); // 获取 WeatherManager 实例

        // 初始化血量UI
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
        UpdateHealthText();

        // 初始化死亡面板
        if (deathPanel != null)
        {
            deathPanel.SetActive(false); // 游戏开始时隐藏死亡面板
            if (continueButton != null)
            {
                continueButton.onClick.AddListener(OnContinueClicked); // 添加按钮点击事件
            }
        }

        // 初始化护盾精灵
        if (shieldSprite != null)
        {
            shieldSprite.enabled = false; // 初始隐藏护盾
        }

        StartCoroutine(HealthUpdateRoutine());
    }

    void Update()
    {
        // 对话中禁用移动
        if (playerController != null && playerController.IsInDialogue())
        {
            horizontal = 0f;
            vertical = 0f;
            animator.SetBool("IsRunning", false);
            if (SFXManager.Instance != null)
            {
                SFXManager.Instance.SetWalkSound(false); // 停止走路音效
            }
            return;
        }

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        // 控制角色动画和朝向
        if (move.magnitude > 0.1f)
        {
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }

        // 控制走路音效
        bool isMoving = move.magnitude > 0.1f;
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.SetWalkSound(isMoving);
        }

        if (horizontal > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontal < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }
    }

    void FixedUpdate()
    {
        if (playerController != null && playerController.IsInDialogue())
        {
            return;
        }

        // 处理物理移动
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;
        rigidbody2d.MovePosition(position);
    }

    // 血量更新核心逻辑
    private IEnumerator HealthUpdateRoutine()
    {
        while (true)
        {
            if (!pauseHealthUpdate && !isShieldActive) // 护盾期间不扣血
            {
                if (cameraController != null)
                {
                    if (!cameraController.IsIndoors() && currentHealth > 0) // 室外扣血
                    {
                        // 根据天气调整扣血量
                        int healthLoss;
                        if (weatherManager != null)
                        {
                            switch (weatherManager.currentWeather)
                            {
                                case "radiation":
                                    healthLoss = -6; // 辐射天气扣6血
                                    break;
                                case "rain":
                                case "snow":
                                    healthLoss = -4; // 雨天和雪天扣4血
                                    break;
                                case "sunny":
                                default:
                                    healthLoss = -2; // 晴天扣2血
                                    break;
                            }
                        }
                        else
                        {
                            healthLoss = -2; // 默认扣2血（如果 WeatherManager 未找到）
                        }
                        ChangeHealth(healthLoss, true); // 标记为辐射伤害
                    }
                    else if (cameraController.IsIndoors() && currentHealth < maxHealth) // 室内每秒+10血
                    {
                        ChangeHealth(10);
                    }
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    // 修改血量方法
    public void ChangeHealth(int amount, bool isRadiationDamage = false)
    {
        Debug.Log($"ChangeHealth called with amount: {amount}, isRadiationDamage: {isRadiationDamage}");
        if (amount < 0)
        {
            // 护盾屏蔽所有伤害，包括辐射伤害
            if (isShieldActive)
            {
                Debug.Log("Damage blocked due to shield");
                return;
            }
            // 非辐射伤害触发无敌状态
            if (!isRadiationDamage)
            {
                if (isInvincible)
                {
                    Debug.Log("Damage blocked due to invincibility");
                    return;
                }
                isInvincible = true;
                invincibleTimer = timeInvincible;
                //animator.SetTrigger("Hit");
            }
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log($"New health: {currentHealth}/{maxHealth}");

        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }
        UpdateHealthText();

        if (currentHealth <= 0)
        {
            StartCoroutine(DieAndRespawn());
        }
    }

    // 激活护盾
    public void ActivateShield(float duration)
    {
        shieldTimer += duration; // 累加护盾时间
        if (!isShieldActive) // 仅在未激活时启动协程
        {
            StartCoroutine(ShieldCoroutine());
        }
    }

    // 护盾协程，控制护盾状态和精灵显示
    private IEnumerator ShieldCoroutine()
    {
        isShieldActive = true;
        if (shieldSprite != null)
        {
            shieldSprite.enabled = true; // 显示护盾精灵
        }

        while (shieldTimer > 0)
        {
            shieldTimer -= Time.deltaTime;
            yield return null;
        }

        isShieldActive = false;
        shieldTimer = 0f; // 确保计时器归零
        if (shieldSprite != null)
        {
            shieldSprite.enabled = false; // 隐藏护盾精灵
        }
    }

    // 更新血量文字的方法
    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{maxHealth}"; // 显示 "当前血量/最大血量"
        }
    }

    // 死亡和重生逻辑
    private IEnumerator DieAndRespawn()
    {
        // 淡入黑色并显示死亡面板
        yield return StartCoroutine(FadeManager.Instance.FadeToBlack(() =>
        {
            if (deathPanel != null)
            {
                Time.timeScale = 0f; // 暂停游戏时间
                deathPanel.SetActive(true);
                pauseHealthUpdate = true; // 暂停血量更新
            }
        }));
    }

    // 继续游戏按钮的点击事件
    private void OnContinueClicked()
    {
        Time.timeScale = 1f; // 恢复游戏时间

        // 重生逻辑
        if (cameraController != null && cameraController.housePlayerPositions.Length > 1)
        {
            transform.position = cameraController.housePlayerPositions[1];
            cameraController.transform.position = cameraController.housePositions[1];
            cameraController.isIndoors = true;
            cameraController.currentHouseIndex = 1;
        }
        else
        {
            transform.position = Vector3.zero;
        }

        // 重置血量
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }
        UpdateHealthText();

        // 隐藏死亡面板
        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }
        pauseHealthUpdate = false; // 恢复血量更新

        // 重置护盾状态
        isShieldActive = false;
        shieldTimer = 0f; // 重置护盾计时器
        if (shieldSprite != null)
        {
            shieldSprite.enabled = false; // 确保重生时隐藏护盾
        }
    }

    public void UpdateLastHousePosition(Vector3 position)
    {
        lastHousePosition = position;
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        animator.SetTrigger("Launch");
    }
}