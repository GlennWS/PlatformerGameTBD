using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
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
        if (IsGrounded())
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
        if (value.performed)
        {
            dashAbility.Dash(horizontalMovement, facingDirection);
        }
    }

    public void BurstAction(InputAction.CallbackContext value)
    {
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