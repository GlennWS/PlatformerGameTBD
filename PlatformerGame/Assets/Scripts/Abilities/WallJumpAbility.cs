using UnityEngine;

public class WallJumpAbility : PlayerAbility
{
    [Header("Wall Detection")]
    [SerializeField] private float wallCheckDistance = 0.5f;
    [SerializeField] private LayerMask wallLayer;
    public bool isTouchingWall { get; private set; } = false;

    [Header("Wall Jump Parameters")]
    [SerializeField] private float wallJumpForceX = 8f;
    [SerializeField] private float wallJumpForceY = 12f;
    [SerializeField] private float wallSlideSpeed = 2f;

    private Collider2D playerCollider;

    public override void Initialize(PlayerController playerController)
    {
        base.Initialize(playerController);
        playerCollider = playerController.GetComponent<Collider2D>();
    }

    public void HandleWallLogic()
    {
        if (!IsUnlocked || pc.IsDead)
        {
            return;
        }

        CheckWall();

        bool isPressingIntoWall = (pc.facingDirection == Mathf.Sign(pc.horizontalMovement));

        if (isTouchingWall && !pc.IsGrounded() && rb.linearVelocity.y < 0 && isPressingIntoWall)
        {
            float newYVelocity = Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, newYVelocity);

            RestoreGravity();
        }
    }

    public void WallJump()
    {
        bool isPressingIntoWall = (pc.facingDirection == Mathf.Sign(pc.horizontalMovement));

        if (!IsUnlocked || !isPressingIntoWall) return;

        pc.FlipFacingDirection();

        float horizontalForce = pc.facingDirection * wallJumpForceX;
        rb.linearVelocity = new Vector2(horizontalForce, wallJumpForceY);
        RestoreGravity();
    }

    private void CheckWall()
    {
        Vector2 playerSize = playerCollider.bounds.size;
        Vector2 castDirection = new Vector2(pc.facingDirection, 0);
        Vector2 castOrigin = (Vector2)rb.position + castDirection * (playerSize.x / 2f);
        Vector2 castBoxSize = new Vector2(0.05f, playerSize.y * 0.9f);

        RaycastHit2D hit = Physics2D.BoxCast(castOrigin, castBoxSize,
            0f, castDirection, wallCheckDistance, wallLayer);
        
        isTouchingWall = hit.collider != null;
    }
}
