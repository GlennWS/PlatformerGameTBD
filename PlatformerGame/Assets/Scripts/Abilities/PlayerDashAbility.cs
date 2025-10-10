using System.Collections;
using UnityEngine;

public class PlayerDashAbility : MonoBehaviour
{
    [Header("Dash Parameters")]
    [SerializeField] private float dashingPower = 24.0f;
    [SerializeField] private float dashingTime = 0.2f;
    [SerializeField] private float dashingCooldown = 1.0f;

    private Rigidbody2D rb;
    private TrailRenderer tr;
    private bool canDash = true;
    public bool IsActive { get; private set; } = false;
    [SerializeField] private bool isUnlockedField = true;
    public bool IsUnlocked
    {
        get { return isUnlockedField; }
        set { isUnlockedField = value; }
    }

    public void Initialize(PlayerController pc)
    {
        rb = pc.rb;
        tr = pc.tr;
    }

    public void Dash(float horizontalInput, float storedFacingDirection)
    {
        if (!canDash || !IsUnlocked) return;
        StartCoroutine(DashCoroutine(horizontalInput, storedFacingDirection));
    }

    private IEnumerator DashCoroutine(float horizontalInput, float storedFacingDirection)
    {
        canDash = false;
        IsActive = true;

        float dashDirection = (horizontalInput != 0) ? Mathf.Sign(horizontalInput) : storedFacingDirection;

        float originalGravity = rb.gravityScale;

        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(dashDirection * dashingPower, 0f);
        tr.emitting = true;

        yield return new WaitForSeconds(dashingTime);

        tr.emitting = false;
        rb.gravityScale = originalGravity;
        IsActive = false;

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}