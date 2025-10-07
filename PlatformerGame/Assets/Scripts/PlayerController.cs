using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rb = null;

    [Header("Player Basic Movement")]
    [SerializeField]
    private float moveSpeed = 5.0f;
    private float horizontalMovement;

    [Header("Jumping")]
    [SerializeField]
    private float jumpSpeed = 10.0f;

    [Header("Ground Check")]
    [SerializeField]
    private Transform groundCheckPos;
    [SerializeField]
    private Vector2 groundCheckSize = new Vector2(0.5f, 0.5f);
    [SerializeField]
    private LayerMask groundLayer;

    [Header("Gravity")]
    [SerializeField]
    private float baseGravity = 2.0f;
    [SerializeField]
    private float maxFallSpeed = 18.0f;
    [SerializeField]
    private float fallSpeedMultiplier = 2.0f;

    public void Update()
    {
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
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
