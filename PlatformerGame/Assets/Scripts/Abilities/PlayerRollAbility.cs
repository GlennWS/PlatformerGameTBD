using System.Collections;
using UnityEngine;

public class PlayerRollAbility : MonoBehaviour
{
    [Header("Roll Parameters")]
    [SerializeField] private float rollDuration = 0.4f;
    [SerializeField] private float rollSpeed = 10.0f;
    [SerializeField] private float rollCooldown = 0.5f;

    [Header("Collider Adjustment")]
    [SerializeField] private float rolledHeight = 0.5f;
    [SerializeField] private float rolledOffset = -0.25f;

    [Header("Boosted Jump")]
    [SerializeField] private float boostedJumpMultiplier = 1.5f;
    [SerializeField] private float boostedJumpHorizontalForce = 5.0f;
    [SerializeField] private float jumpBoostWindow = 0.15f;

    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private PlayerController pc;

    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    private bool canRoll = true;
    private bool shouldBoostNextJump = false;

    public bool IsActive { get; private set; } = false;

    [SerializeField] private bool isUnlockedField = true;
    public bool IsUnlocked
    {
        get { return isUnlockedField; }
        set { isUnlockedField = value; }
    }

    public void Initialize(PlayerController controller, BoxCollider2D playerCollider)
    {
        pc = controller;
        rb = controller.rb;
        coll = playerCollider;

        originalColliderSize = coll.size;
        originalColliderOffset = coll.offset;
    }

    public void Roll()
    {
        if (!canRoll || IsActive || !IsUnlocked) return;

        StopAllCoroutines();

        StartCoroutine(RollCoroutine());
    }

    private IEnumerator RollCoroutine()
    {
        canRoll = false;
        IsActive = true;

        float rollDirection = pc.facingDirection;
        float originalGravity = rb.gravityScale;

        rb.linearVelocity = new Vector2(rollDirection * rollSpeed, rb.linearVelocity.y);
        rb.gravityScale = 0f;

        coll.size = new Vector2(originalColliderSize.x, rolledHeight);
        coll.offset = new Vector2(originalColliderOffset.x, rolledOffset);

        yield return new WaitForSeconds(rollDuration);

        IsActive = false;
        rb.gravityScale = originalGravity;

        float clearanceNeeded = originalColliderSize.y - rolledHeight;

        while (!pc.IsClearAbove(clearanceNeeded))
        {
            yield return new WaitForFixedUpdate();
        }

        coll.size = originalColliderSize;
        coll.offset = originalColliderOffset;

        StartCoroutine(BoostWindowCoroutine());

        yield return new WaitForSeconds(rollCooldown);
        canRoll = true;
    }

    private IEnumerator BoostWindowCoroutine()
    {
        shouldBoostNextJump = true;

        yield return new WaitForSeconds(jumpBoostWindow);

        shouldBoostNextJump = false;
    }

    public (float verticalMultiplier, float horizontalForce) ConsumeJumpBoost()
    {
        if (shouldBoostNextJump)
        {
            shouldBoostNextJump = false;
            StopCoroutine(BoostWindowCoroutine());

            return (boostedJumpMultiplier, boostedJumpHorizontalForce);
        }
        return (1.0f, 0.0f);
    }
}