using UnityEngine;

public class PlayerGlideAbility : PlayerAbility
{
    [SerializeField] private float glideGravityScale = 2.0f;
    [SerializeField] private float maxGlideFallSpeed = -2.0f;

    void FixedUpdate()
    {
        if (IsActive)
        {
            if (rb.linearVelocity.y < maxGlideFallSpeed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxGlideFallSpeed);
            }
        }
    }

    public void StartGlide()
    {
        if (!IsUnlocked || IsActive) return;
        IsActive = true;
        if (rb.linearVelocity.y < maxGlideFallSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxGlideFallSpeed);
        }
        rb.gravityScale = glideGravityScale;
    }

    public void StopGlide()
    {
        if (!IsActive) return;
        IsActive = false;
        RestoreGravity();
    }
}