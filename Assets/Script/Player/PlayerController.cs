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
            // 1. 움직이고 있을 때
            if (movement.magnitude > 0.01f)
            {
                lastLookDirection = movement.normalized;

                // 걷는 방향 벡터를 애니메이터에 전달 (Walk 애니메이션 출력)
                animator.SetFloat("InputX", lastLookDirection.x);
                animator.SetFloat("InputY", lastLookDirection.y);
            }
            // 2. 완전히 멈췄을 때
            else
            {
                // 완전히 멈추면 좌표를 (0, 0) 근처로 보내서 Idle 애니메이션이 나오게 하되,
                // 마지막으로 바라보던 방향의 Idle을 찾을 수 있도록 아주 미세한(0.001) 값을 더해줍니다.
                animator.SetFloat("InputX", lastLookDirection.x * 0.001f);
                animator.SetFloat("InputY", lastLookDirection.y * 0.001f);
            }
        }
    }
}