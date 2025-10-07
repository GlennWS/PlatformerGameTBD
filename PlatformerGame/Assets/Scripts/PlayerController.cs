using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb = null;

    [Header("Player Basic Movement")]
    [SerializeField] private float moveSpeed = 5.0f;
    private float horizontalMovement;
    private float facingDirection = 1f;

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

    [Header("Dashing")]
    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 24.0f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1.0f;
    [SerializeField] private TrailRenderer tr;

    public void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
        Gravity();
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
    }

    public void Update()
    {
        IsGrounded();
    }

    private void Gravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        } else
        {
            rb.gravityScale = baseGravity;
        }
    }

    public void Move(InputAction.CallbackContext value)
    {
        horizontalMovement = value.ReadValue<Vector2>().x;

        if (horizontalMovement != 0)
        {
            facingDirection = Mathf.Sign(horizontalMovement);
        }
    }

    public void Jump(InputAction.CallbackContext value)
    {
        if (IsGrounded())
        {
            if (value.performed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);
            }
            else if (value.canceled)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            }
        }
    }

    public void Dash(InputAction.CallbackContext value)
    {
        if (value.performed && canDash)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        float dashDirection = (horizontalMovement != 0) ? Mathf.Sign(horizontalMovement) : facingDirection;

        rb.linearVelocity = new Vector2(dashDirection * dashingPower, 0f);
        tr.emitting = true;

        yield return new WaitForSeconds(dashingTime);

        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashingCooldown);

        canDash = true;
    }

    private bool IsGrounded()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
        {
            return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }
}
