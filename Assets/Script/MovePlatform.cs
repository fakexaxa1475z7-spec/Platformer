using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    public float waitTime = 0f; // เวลาหยุดรอก่อนกลับ

    private Rigidbody2D rb;
    private Vector2 target;
    private bool waiting = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = pointB.position;
    }

    void FixedUpdate()
    {
        if (waiting) return;

        rb.MovePosition(Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime));

        if (Vector2.Distance(rb.position, target) < 0.05f)
        {
            StartCoroutine(SwitchTarget());
        }
    }

    System.Collections.IEnumerator SwitchTarget()
    {
        waiting = true;
        yield return new WaitForSeconds(waitTime);

        target = (Vector2)target == (Vector2)pointA.position ?
                 pointB.position : pointA.position;

        waiting = false;
    }

    // ทำให้ Player ขยับไปพร้อมแพลตฟอร์ม
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
