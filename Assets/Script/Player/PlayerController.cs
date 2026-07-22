using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 5f;

    [Header("채집 & 타격 설정 (3x3 사각형 영역)")]
    [Tooltip("3x3 사각형 가로/세로 크기 (1타일이 1이면 3.0 정도가 3x3 영역)")]
    public Vector2 areaSize = new Vector2(3f, 3f);
    public int attackDamage = 1;
    public LayerMask resourceLayer;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    private Vector2 lastLookDirection = Vector2.down;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement * moveSpeed;
    }

    // 키보드/패드 이동 입력
    public void OnMove(InputValue value)
    {
        movement = value.Get<Vector2>();

        if (movement.magnitude > 0.01f)
        {
            lastLookDirection = Get4WayDirection(movement);
            UpdateAnimator(lastLookDirection, movement.magnitude);
        }
        else
        {
            UpdateAnimator(lastLookDirection, 0f);
        }
    }

    void Update()
    {
        // 마우스 좌클릭 또는 화면 터치 시
        if (Input.GetMouseButtonDown(0))
        {
            HandleTouchOrClick();
        }
    }

    // 마우스/터치 선택 처리
    private void HandleTouchOrClick()
    {
        // 1. 화면 클릭 위치를 World 좌표로 변환
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 targetPos = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
        Vector2 playerPos = (Vector2)transform.position;

        // 2. 클릭한 위치가 플레이어 중심 3x3 사각형 범위 내인지 확인
        Bounds attackBounds = new Bounds(playerPos, areaSize);

        if (attackBounds.Contains(targetPos))
        {
            // 3. 클릭한 방향으로 플레이어 시선 돌리기
            Vector2 direction = targetPos - playerPos;
            if (direction.magnitude > 0.1f) // 플레이어 본인 위치가 아닌 경우
            {
                lastLookDirection = Get4WayDirection(direction);
                UpdateAnimator(lastLookDirection, 0f);
            }

            // 4. 클릭한 지점에 Object(자원)가 있는지 검사하여 타격
            Collider2D hit = Physics2D.OverlapPoint(targetPos, resourceLayer);
            if (hit != null)
            {
                Resource resource = hit.GetComponent<Resource>();
                if (resource != null)
                {
                    resource.TakeDamage(attackDamage);
                }
            }
        }
    }

    // 대각선 입력을 상/하/좌/우 4방향 정밀 벡터로 변환하는 함수
    private Vector2 Get4WayDirection(Vector2 dir)
    {
        dir.Normalize();
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            return dir.x > 0 ? Vector2.right : Vector2.left;
        }
        else
        {
            return dir.y > 0 ? Vector2.up : Vector2.down;
        }
    }

    // 애니메이터 파라미터 갱신
    private void UpdateAnimator(Vector2 dir, float speed)
    {
        if (animator != null)
        {
            animator.SetFloat("InputX", dir.x);
            animator.SetFloat("InputY", dir.y);
            animator.SetFloat("Speed", speed);
        }
    }

    // Scene 뷰에서 3x3 빨간색 사각형 범위 시각화
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, areaSize);
    }
}