using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentRubyController : MonoBehaviour
{
    public float speed = 3.0f;

    public int maxHealth = 5;
    public float timeInvincible = 2.0f;

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

    private TransparentPlayerController transparentPlayerController; // ���� TransparentPlayerController

    // �ڵ�һ��֡����֮ǰ���� Start
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;

        // ��ȡ TransparentPlayerController ���
        transparentPlayerController = GetComponent<TransparentPlayerController>();
        if (transparentPlayerController == null)
        {
            Debug.LogError("TransparentPlayerController ���δ�ҵ�����ȷ��͸����ɫ�������� TransparentPlayerController �ű���");
        }
    }

    // ÿ֡����һ�� Update
    void Update()
    {
        // ����ڶԻ��У��������봦��
        if (transparentPlayerController != null && transparentPlayerController.IsInDialogue())
        {
            horizontal = 0f;
            vertical = 0f;
            animator.SetBool("IsRunning", false); // ֹͣ���ж���
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

        // �����Ƿ��������л�����
        if (move.magnitude > 0.1f) // ��ֵ 0.1f ����΢С��������
        {
            animator.SetBool("IsRunning", true); // ����״̬
        }
        else
        {
            animator.SetBool("IsRunning", false); // վ��״̬
        }

        // ����ˮƽ���뷭ת����
        if (horizontal > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // ��������
        }
        else if (horizontal < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // ��ת����
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
        // ����ڶԻ��У������ƶ�
        if (transparentPlayerController != null && transparentPlayerController.IsInDialogue())
        {
            return;
        }

        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            animator.SetTrigger("Hit");
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log(currentHealth + "/" + maxHealth);
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        animator.SetTrigger("Launch");
    }
}
