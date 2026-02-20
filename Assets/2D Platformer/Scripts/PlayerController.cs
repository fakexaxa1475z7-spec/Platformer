using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class PlayerController : MonoBehaviour
    {
        public float movingSpeed;
        public float jumpForce;
        public Transform groundCheck;

        private Collider2D playerCollider;
        private bool isDropping = false;
        public float dropDownDuration = 0.5f;

        [Header("Wall Jump Settings")]
        public Transform wallCheck;
        public LayerMask wallLayer;
        public float wallCheckDistance = 0.5f;
        public float wallSlideSpeed = 2f;
        public float wallJumpForceX = 8f;
        public float wallJumpForceY = 10f;

        private float moveInput;
        private bool facingRight = false;
        private bool isGrounded;
        private bool isTouchingWall;
        private bool isWallSliding;

        [Header("Wall Jump Lock")]
        public float wallJumpLockTime = 0.2f;

        private bool isWallJumping;
        private float wallJumpLockCounter;

        [HideInInspector]
        public bool deathState = false;
        public bool winState = false;

        private Rigidbody2D rigidbody;
        private Animator animator;
        private GameManager gameManager;

        void Start()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            playerCollider = GetComponent<Collider2D>();

            animator = GetComponent<Animator>();
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

            CoinManager.Instance.LoadCoins();
        }

        private void FixedUpdate()
        {
            CheckGround();
            CheckWall();
        }

        void Update()
        {
            if (isWallJumping)
            {
                wallJumpLockCounter -= Time.deltaTime;

                if (wallJumpLockCounter <= 0)
                {
                    isWallJumping = false;
                }
            }
            if (Input.GetKeyDown(KeyCode.S) && isGrounded && !isDropping)
            {
                StartCoroutine(DropDown());
            }
            Move();
            HandleWallSlide();
            HandleJump();
            HandleAnimation();
            HandleFlip();
        }

        void Move()
        {
            if (!isWallJumping)
            {
                moveInput = Input.GetAxis("Horizontal");

                // ถ้ากำลังไถลกำแพง ห้ามดันเข้ากำแพง
                if (isWallSliding)
                {
                    rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
                }
                else
                {
                    rigidbody.velocity = new Vector2(
                        moveInput * movingSpeed,
                        rigidbody.velocity.y
                    );
                }
            }
        }

        IEnumerator DropDown()
        {
            isDropping = true;

            Collider2D platform = Physics2D.OverlapCircle(
                groundCheck.position,
                0.2f,
                LayerMask.GetMask("OneWayPlatform")
            );

            if (platform != null)
            {
                Physics2D.IgnoreCollision(playerCollider, platform, true);
                yield return new WaitForSeconds(dropDownDuration);
                Physics2D.IgnoreCollision(playerCollider, platform, false);
            }

            isDropping = false;
        }

        void HandleJump()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isGrounded)
                {
                    rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpForce);
                }
                else if (isWallSliding)
                {
                    float direction = facingRight ? -1 : 1;

                    rigidbody.velocity = new Vector2(direction * wallJumpForceX, wallJumpForceY);

                    isWallJumping = true;
                    wallJumpLockCounter = wallJumpLockTime;

                    Flip(); // หันหน้าทันทีตอนเด้งออก
                }
            }
        }

        void HandleWallSlide()
        {
            if (isTouchingWall && !isGrounded && rigidbody.velocity.y < 0)
            {
                isWallSliding = true;

                rigidbody.velocity = new Vector2(
                    rigidbody.velocity.x,
                    -wallSlideSpeed
                );
            }
            else
            {
                isWallSliding = false;
            }
        }

        void HandleFlip()
        {
            if (facingRight == false && moveInput > 0)
                Flip();
            else if (facingRight == true && moveInput < 0)
                Flip();
        }

        void HandleAnimation()
        {
            if (!isGrounded)
                animator.SetInteger("playerState", 2); // jump
            else if (moveInput != 0)
                animator.SetInteger("playerState", 1); // run
            else
                animator.SetInteger("playerState", 0); // idle
        }

        private void Flip()
        {
            facingRight = !facingRight;
            Vector3 scaler = transform.localScale;
            scaler.x *= -1;
            transform.localScale = scaler;
        }

        private void CheckGround()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, 0.2f);
            isGrounded = colliders.Length > 1;
        }

        private void CheckWall()
        {
            Vector2 direction = facingRight ? Vector2.right : Vector2.left;

            isTouchingWall = Physics2D.Raycast(
                wallCheck.position,
                direction,
                wallCheckDistance,
                wallLayer
            );
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.tag == "Enemy")
            {
                deathState = true;
            }
            else
            {
                deathState = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Coin"))
            {
                Coin coin = other.GetComponent<Coin>();

                if (coin != null)
                {
                    // บันทึกว่าเหรียญนี้ถูกเก็บแล้ว
                    PlayerPrefs.SetInt("Coin_" + coin.coinID, 1);
                }

                CoinManager.Instance.AddCoin(1);

                Destroy(other.gameObject);
            }
            if (other.gameObject.tag == "Goal")
            {
                winState = true;
            }
        }
    }
}
