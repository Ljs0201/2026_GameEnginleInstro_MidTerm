using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator pAni;
    private bool isGrounded;
    private float moveInput;

    private bool isGiant = false;

    float score;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pAni = GetComponent<Animator>();
        score = 0f;
    }

    private void Update()
    {
        // 이동 로직
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // 거대화 여부에 따른 크기 및 방향 설정
        if (isGiant)
        {
            if (moveInput < 0)
                transform.localScale = new Vector3(2, 2, 2);
            else if (moveInput > 0)
                transform.localScale = new Vector3(-2, 2, 2);
        }
        else
        {
            if (moveInput < 0)
                transform.localScale = new Vector3(1, 1, 1);
            else if (moveInput > 0)
                transform.localScale = new Vector3(-1, 1, 1);
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        moveInput = input.x;
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            pAni.SetTrigger("Jump");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. 리스폰 (낙사 등)
        if (collision.CompareTag("Respawn"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // 2. 피니시 지점 도달
        if (collision.CompareTag("Finish"))
        {
            HighScore.TrySet(SceneManager.GetActiveScene().buildIndex, (int)score);
            collision.GetComponent<LevelObject>().MoveToNextLevel();
        }

        // 3. 적(Enemy)과 충돌 시 로직 수정
        if (collision.CompareTag("Enemy"))
        {
            if (isGiant)
            {
                // 거대 상태라면 적을 파괴
                Destroy(collision.gameObject);
            }
            else
            {
                // 일반 상태라면 플레이어 사망 (씬 재시작)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        // 4. 아이템(Item) 획득 시 로직 수정
        if (collision.CompareTag("Item"))
        {
            score += 10f;
            isGiant = true; // 거대화 상태 활성화
            Destroy(collision.gameObject); // 먹은 아이템 제거
        }
    }
}