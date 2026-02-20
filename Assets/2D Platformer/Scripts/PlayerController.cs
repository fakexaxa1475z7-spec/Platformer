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

        private Rigidbody2D rb;
        private Animator animator;
        private GameManager gameManager;

        // 🔊 ===== SOUND =====
        [Header("Audio")]
        public AudioSource footstepAudio;
        public AudioSource sfxAudio;

        public AudioClip jumpClip;
        public AudioClip deathClip;
        public AudioClip coinClip;
        public AudioClip winClip;

        public float footstepDelay = 0.4f;
        private float footstepTimer;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
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
            if (deathState)
            {
                HandleAnimation();
                return;
            }

            if (isWallJumping)
            {
                wallJumpLockCounter -= Time.deltaTime;
                if (wallJumpLockCounter <= 0)
                    isWallJumping = false;
            }

            if (Input.GetKeyDown(KeyCode.S) && isGrounded && !isDropping)
            {
                StartCoroutine(DropDown());
            }

            Move();
            HandleFootstepSound();
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

                if (isWallSliding)
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2(moveInput * movingSpeed, rb.velocity.y);
                }
            }
        }

        // 🔊 FOOTSTEP
        void HandleFootstepSound()
        {
            if (isGrounded && Mathf.Abs(moveInput) > 0.1f)
            {
                footstepTimer -= Time.deltaTime;

                if (footstepTimer <= 0)
                {
                    if (footstepAudio != null)
                    {
                        footstepAudio.pitch = Random.Range(0.9f, 1.1f);
                        footstepAudio.Play();
                    }
                    footstepTimer = footstepDelay;
                }
            }
            else
            {
                if (footstepAudio != null && footstepAudio.isPlaying)
                {
                    footstepAudio.Stop();
                }
                footstepTimer = 0;
            }
        }

        // 🔊 JUMP
        void HandleJump()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isGrounded)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);

                    if (jumpClip != null && sfxAudio != null)
                    {
                        sfxAudio.PlayOneShot(jumpClip);
                    }
                }
                else if (isWallSliding)
                {
                    float direction = facingRight ? -1 : 1;

                    rb.velocity = new Vector2(direction * wallJumpForceX, wallJumpForceY);

                    isWallJumping = true;
                    wallJumpLockCounter = wallJumpLockTime;

                    Flip();

                    if (jumpClip != null && sfxAudio != null)
                    {
                        sfxAudio.PlayOneShot(jumpClip);
                    }
                }
            }
        }

        void HandleWallSlide()
        {
            if (isTouchingWall && !isGrounded && rb.velocity.y < 0)
            {
                isWallSliding = true;
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
            else
            {
                isWallSliding = false;
            }
        }

        void HandleFlip()
        {
            if (!facingRight && moveInput > 0)
                Flip();
            else if (facingRight && moveInput < 0)
                Flip();
        }

        void HandleAnimation()
        {
            if (deathState)
            {
                animator.SetInteger("playerState", 3);
                return;
            }

            if (!isGrounded)
                animator.SetInteger("playerState", 2);
            else if (Mathf.Abs(moveInput) > 0.1f)
                animator.SetInteger("playerState", 1);
            else
                animator.SetInteger("playerState", 0);
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

        // 🔊 DIE (แก้ตรงนี้)
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                if (deathState) return;

                deathState = true;

                // 🔊 เล่นเสียงทันที
                if (deathClip != null)
                {
                    AudioSource.PlayClipAtPoint(deathClip, transform.position);
                }
            }
        }

        // 🔊 COIN + WIN
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Coin"))
            {
                Coin coin = other.GetComponent<Coin>();

                if (coin != null)
                {
                    PlayerPrefs.SetInt("Coin_" + coin.coinID, 1);
                }

                CoinManager.Instance.AddCoin(1);

                if (coinClip != null && sfxAudio != null)
                {
                    sfxAudio.PlayOneShot(coinClip);
                }

                Destroy(other.gameObject);
            }

            if (other.gameObject.CompareTag("Goal"))
            {
                winState = true;

                if (winClip != null && sfxAudio != null)
                {
                    sfxAudio.PlayOneShot(winClip);
                }
            }
        }
    }
}