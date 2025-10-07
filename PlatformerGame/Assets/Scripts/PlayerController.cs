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

    [Header("Jumping")]
    [SerializeField] private float jumpSpeed = 10.0f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPos;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.5f);
    [SerializeField] private LayerMask groundLayer;

    [Header("Gravity")]
    [SerializeField] private float baseGravity = 2.0f;
    [SerializeField] private float maxFallSpeed = 18.0f;
    [SerializeField] private float fallSpeedMultiplier = 2.0f;

    void Start()
    {
        currentHealth = maxHealth;
        dashAbility.Initialize(this);
        burstAbility.Initialize(this);
    }

    public void FixedUpdate()
    {
        if (dashAbility.IsActive || burstAbility.IsActive)
        {
            return;
        }

        bool isGrounded = IsGrounded();

        if (Mathf.Abs(rb.linearVelocity.x) > naturalDecayThreshold && !isGrounded)
        {
            Gravity();
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));

            return;
        }

        Gravity();

        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
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
        return Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer);
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
        if (IsGrounded() && !IsDead)
        {
            if (value.performed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);
            }
            else if (value.canceled && rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            }
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }
}