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

    public void Update()
    {
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
    }

    public void Move(InputAction.CallbackContext value)
    {
        horizontalMovement = value.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext value)
    {
        if (isGrounded())
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

    private bool isGrounded()
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
