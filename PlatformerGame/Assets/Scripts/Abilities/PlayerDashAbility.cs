using System.Collections;
using UnityEngine;

public class PlayerDashAbility : PlayerAbility
{
    [Header("Dash Parameters")]
    [SerializeField] private float dashingPower = 24.0f;
    [SerializeField] private float dashingTime = 0.2f;
    [SerializeField] private float dashingCooldown = 1.0f;

    private TrailRenderer tr;
    private bool canDash = true;
    private bool usedInAir = false;

    public override void Initialize(PlayerController playerController)
    {
        base.Initialize(playerController);
        tr = playerController.tr;
    }

    public void Dash(float horizontalInput, float storedFacingDirection)
    {
        if (!canDash || !IsUnlocked || usedInAir) return;
        usedInAir = true;
        StartCoroutine(DashCoroutine(horizontalInput, storedFacingDirection));
    }

    private IEnumerator DashCoroutine(float horizontalInput, float storedFacingDirection)
    {
        canDash = false;
        IsActive = true;

        float dashDirection = (horizontalInput != 0) ? Mathf.Sign(horizontalInput) : storedFacingDirection;

        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(dashDirection * dashingPower, 0f);
        tr.emitting = true;

        yield return new WaitForSeconds(dashingTime);

        tr.emitting = false;
        RestoreGravity();
        IsActive = false;

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    public void ResetAirUse()
    {
        usedInAir = false;
        canDash = true;
    }
}