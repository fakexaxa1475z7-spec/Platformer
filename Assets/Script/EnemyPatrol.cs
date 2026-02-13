using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public float moveSpeed = 3f;
    public Transform groundCheck;
    public float groundDistance = 0.5f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private int direction = 1;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // เดิน
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        // เช็คพื้นด้านหน้า
        RaycastHit2D groundInfo = Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            groundDistance,
            groundLayer
        );

        if (!groundInfo.collider)
        {
            Flip();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // ชนกำแพงแล้วกลับ
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                Flip();
                break;
            }
        }
    }

    void Flip()
    {
        direction *= -1;
        transform.localScale = new Vector3(direction, 1, 1);
    }
}