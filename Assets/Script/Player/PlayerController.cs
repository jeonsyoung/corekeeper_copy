using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 5f;

    [Header("채집 & 공격 설정")]
    public float interactionRange = 1.5f;
    public int attackDamage = 1;
    public LayerMask resourceLayer;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Vector2 movement;

    // 마지막으로 바라보던 방향을 기억 (기본값: 아래쪽)
    private Vector2 lastLookDirection = Vector2.down;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement * moveSpeed;
    }

    public void OnMove(InputValue value)
    {
        movement = value.Get<Vector2>();

        if (animator != null)
        {
            // 1. 움직이고 있을 때: 방향 업데이트
            if (movement.magnitude > 0.01f)
            {
                lastLookDirection = movement.normalized;
            }

            // 2. 바라보는 방향(InputX, InputY)은 움직이든 멈추든 항상 유지!
            animator.SetFloat("InputX", lastLookDirection.x);
            animator.SetFloat("InputY", lastLookDirection.y);

            // 3. 현재 속도 크기(Speed)를 애니메이터에 전달! (★ 핵심 수정사항)
            animator.SetFloat("Speed", movement.magnitude);
        }
    }
}