using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WallJumpAbility : MonoBehaviour
{
    private PlayerController controller;
    private Rigidbody2D rb;

    [Header("Wall Detection")]
    [SerializeField] private float wallCheckDistance = 0.5f;
    [SerializeField] private LayerMask wallLayer;
    public bool isTouchingWall { get; private set; } = false;

    [Header("Wall Jump Parameters")]
    [SerializeField] private float wallJumpForceX = 8f;
    [SerializeField] private float wallJumpForceY = 12f;
    [SerializeField] private float wallSlideSpeed = 2f;

    public bool IsUnlocked { get; set; } = true;

    public void Initialize(PlayerController pc)
    {
        controller = pc;
        rb = pc.rb;
    }

    public void HandleWallLogic()
    {
        if (!IsUnlocked || controller.IsDead)
        {
            return;
        }

        CheckWall();

        bool isPressingIntoWall = (controller.facingDirection == Mathf.Sign(controller.horizontalMovement));

        if (isTouchingWall && !controller.IsGrounded() && rb.linearVelocity.y < 0 && isPressingIntoWall)
        {
            float newYVelocity = Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, newYVelocity);

            rb.gravityScale = controller.baseGravity;
        }
    }

    public void WallJump()
    {
        bool isPressingIntoWall = (controller.facingDirection == Mathf.Sign(controller.horizontalMovement));

        if (!IsUnlocked || !isPressingIntoWall) return;

        controller.FlipFacingDirection();

        float horizontalForce = controller.facingDirection * wallJumpForceX;
        rb.linearVelocity = new Vector2(horizontalForce, wallJumpForceY);
        rb.gravityScale = controller.baseGravity;
    }

    private void CheckWall()
    {
        Vector2 playerSize = controller.GetComponent<Collider2D>().bounds.size;
        Vector2 castDirection = new Vector2(controller.facingDirection, 0);
        Vector2 castOrigin = (Vector2)rb.position + castDirection * (playerSize.x / 2f);
        Vector2 castBoxSize = new Vector2(0.05f, playerSize.y * 0.9f);

        RaycastHit2D hit = Physics2D.BoxCast(castOrigin, castBoxSize,
            0f, castDirection, wallCheckDistance, wallLayer);
        
        isTouchingWall = hit.collider != null;
    }
}
