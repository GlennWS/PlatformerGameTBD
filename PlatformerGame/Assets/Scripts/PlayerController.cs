using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IDamageable
{
    [SerializeField] public Rigidbody2D rb;
    [SerializeField] public TrailRenderer tr;

    [Header("Ability References")]
    [SerializeField] private PlayerDashAbility dashAbility;
    [SerializeField] private PlayerBurstAbility burstAbility;

    [Header("Player State & Movement")]
    [SerializeField] private float moveSpeed = 5.0f;
    public float horizontalMovement { get; private set; }
    public float facingDirection { get; private set; } = 1f;

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    public bool IsDead { get; private set; } = false;

    [Header("Burst Ability")]
    public Vector2 burstAimInput { get; private set; }
    private bool burstKeyHeld = false;
    [SerializeField] private float burstTriggerThreshold = 0.9f;
    [SerializeField] private float naturalDecayThreshold = 0.5f;

    [Header("Wall Jump Ability")]
    [SerializeField] private WallJumpAbility wallJumpAbility;

    [Header("Jumping")]
    [SerializeField] private float jumpSpeed = 10.0f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPos;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.5f);
    [SerializeField] private LayerMask groundLayer;

    [Header("Gravity")]
    [SerializeField] public float baseGravity = 2.0f;
    [SerializeField] private float maxFallSpeed = 18.0f;
    [SerializeField] private float fallSpeedMultiplier = 2.0f;

    private void Awake()
    {
        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);

        if (players.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        currentHealth = maxHealth;
        dashAbility.Initialize(this);
        burstAbility.Initialize(this);
        wallJumpAbility.Initialize(this);
    }

    public void FixedUpdate()
    {
        if (dashAbility.IsActive || burstAbility.IsActive)
        {
            return;
        }

        wallJumpAbility.HandleWallLogic();

        bool isGrounded = IsGrounded();

        Gravity();

        if (isGrounded && horizontalMovement == 0)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        } else
        {
            rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
        }
    }
    public void TakeDamage(float damageAmount)
    {
        if (IsDead) return;

        currentHealth -= damageAmount;

        Debug.Log(gameObject.name + " took " + damageAmount + " damage. Remaining health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (IsDead) return;

        IsDead = true;
        currentHealth = 0;

        this.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        Debug.Log(gameObject.name + " has died.");
    }

    private void Gravity()
    {
        if (wallJumpAbility.IsUnlocked && wallJumpAbility.isTouchingWall)
        {
            return;
        }
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier;
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    public bool IsGrounded()
    {
        RaycastHit2D collisionInfo = Physics2D.BoxCast(groundCheckPos.position, groundCheckSize, 
            0f, Vector2.down, 0.2f, groundLayer);

        if (collisionInfo.collider != null)
        {
            float angleDot = Vector2.Dot(collisionInfo.normal, Vector2.up);
            if (angleDot > 0.9f)
            {
                return true;
            }
        }

        return false;
    }

    public void Move(InputAction.CallbackContext value)
    {
        horizontalMovement = value.ReadValue<Vector2>().x;
        if (horizontalMovement != 0)
        {
            facingDirection = Mathf.Sign(horizontalMovement);
        }
    }

    public void JumpInput(InputAction.CallbackContext value)
    {
        if (IsDead) return;

        if (value.performed)
        {
            if (IsGrounded())
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);
            }
            else if (wallJumpAbility.IsUnlocked && wallJumpAbility.isTouchingWall && (facingDirection == Mathf.Sign(horizontalMovement)))
            {
                wallJumpAbility.WallJump();
            }
        }
        else if (value.canceled && IsGrounded() && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    public void DashInput(InputAction.CallbackContext value)
    {
        if (value.performed && !IsDead)
        {
            dashAbility.Dash(horizontalMovement, facingDirection);
        }
    }

    public void BurstAction(InputAction.CallbackContext value)
    {
        if (IsDead) return;

        burstAimInput = value.ReadValue<Vector2>();

        if (burstAimInput.magnitude >= burstTriggerThreshold)
        {
            if (!burstKeyHeld)
            {
                burstKeyHeld = true;
                burstAbility.Burst(burstAimInput, facingDirection);
            }
        }
        else
        {
            burstKeyHeld = false;
        }
    }

    public void FlipFacingDirection()
    {
        facingDirection *= -1;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }
}