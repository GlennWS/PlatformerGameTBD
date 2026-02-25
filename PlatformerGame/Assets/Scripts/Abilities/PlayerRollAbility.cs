using System.Collections;
using UnityEngine;

public class PlayerRollAbility : PlayerAbility
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

    private BoxCollider2D coll;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    private bool canRoll = true;
    private bool shouldBoostNextJump = false;

    private Coroutine boostWindowCoroutine;

    public override void Initialize(PlayerController playerController)
    {
        base.Initialize(playerController);
        coll = playerController.GetComponent<BoxCollider2D>();
        originalColliderSize = coll.size;
        originalColliderOffset = coll.offset;
    }

    public void Roll()
    {
        if (!canRoll || IsActive || !IsUnlocked) return;

        CancelBoostWindow();
        StartCoroutine(RollCoroutine());
    }

    private IEnumerator RollCoroutine()
    {
        canRoll = false;
        IsActive = true;

        float rollDirection = pc.facingDirection;

        rb.linearVelocity = new Vector2(rollDirection * rollSpeed, rb.linearVelocity.y);
        rb.gravityScale = 0f;

        coll.size = new Vector2(originalColliderSize.x, rolledHeight);
        coll.offset = new Vector2(originalColliderOffset.x, rolledOffset);

        yield return new WaitForSeconds(rollDuration);

        IsActive = false;
        RestoreGravity();

        float clearanceNeeded = originalColliderSize.y - rolledHeight;

        while (!pc.IsClearAbove(clearanceNeeded))
        {
            yield return new WaitForFixedUpdate();
        }

        coll.size = originalColliderSize;
        coll.offset = originalColliderOffset;

        boostWindowCoroutine = StartCoroutine(BoostWindowCoroutine());

        yield return new WaitForSeconds(rollCooldown);
        canRoll = true;
    }

    private IEnumerator BoostWindowCoroutine()
    {
        shouldBoostNextJump = true;

        yield return new WaitForSeconds(jumpBoostWindow);

        shouldBoostNextJump = false;
        boostWindowCoroutine = null;
    }

    public (float verticalMultiplier, float horizontalForce) ConsumeJumpBoost()
    {
        if (shouldBoostNextJump)
        {
            shouldBoostNextJump = false;
            CancelBoostWindow();

            return (boostedJumpMultiplier, boostedJumpHorizontalForce);
        }
        return (1.0f, 0.0f);
    }

    private void CancelBoostWindow()
    {
        if (boostWindowCoroutine != null)
        {
            StopCoroutine(boostWindowCoroutine);
            boostWindowCoroutine = null;
        }
        shouldBoostNextJump = false;
    }
}