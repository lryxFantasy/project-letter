using UnityEngine;

public class WalkAnimation : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float timer;
    private int state;
    private bool facingRight;
    private bool canWalk = true; // �����������Ƿ������ƶ�

    // �����õĲ���
    [Header("Animation Settings")]
    [SerializeField] private string walkParameter = "isWalking"; // Animator �����߲���
    [SerializeField] private float standDuration = 2f; // վ������ʱ��
    [SerializeField] private float walkDuration = 3f;  // ���߳���ʱ��

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 1.5f;     // �ƶ��ٶ�
    [SerializeField] private float moveDelay = 0.2f;   // λ���ӳ�ʱ��

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        state = 0;
        timer = 0f;
        facingRight = false; // Ĭ�ϳ���
    }

    void Update()
    {
        // ����������ƶ���ǿ�ƽ���վ��״̬
        if (!canWalk)
        {
            animator.SetBool(walkParameter, false);
            spriteRenderer.flipX = facingRight; // ���ֵ�ǰ����
            return;
        }

        timer += Time.deltaTime;

        switch (state)
        {
            case 0: // ��ʼվ��
                animator.SetBool(walkParameter, false);
                spriteRenderer.flipX = facingRight; // ������һ�εĳ���
                if (timer >= standDuration)
                {
                    state = 1;
                    timer = 0f;
                }
                break;

            case 1: // ����
                animator.SetBool(walkParameter, true);
                spriteRenderer.flipX = false; // ����
                facingRight = false;
                if (timer >= moveDelay)
                {
                    transform.position += Vector3.left * moveSpeed * Time.deltaTime;
                }
                if (timer >= walkDuration)
                {
                    state = 2;
                    timer = 0f;
                }
                break;

            case 2: // �м�վ��
                animator.SetBool(walkParameter, false);
                spriteRenderer.flipX = facingRight; // ������һ�εĳ���
                if (timer >= standDuration)
                {
                    state = 3;
                    timer = 0f;
                }
                break;

            case 3: // ����
                animator.SetBool(walkParameter, true);
                spriteRenderer.flipX = true; // ����
                facingRight = true;
                if (timer >= moveDelay)
                {
                    transform.position += Vector3.right * moveSpeed * Time.deltaTime;
                }
                if (timer >= walkDuration)
                {
                    state = 0;
                    timer = 0f;
                }
                break;
        }
    }

    // ���������������Ƿ������ƶ������߶���
    public void SetWalkingState(bool allowWalking)
    {
        canWalk = allowWalking;
        if (!canWalk)
        {
            timer = 0f; // ���ü�ʱ��������״̬�л�
            state = 0;  // ǿ�ƽ���վ��״̬
        }
    }
}