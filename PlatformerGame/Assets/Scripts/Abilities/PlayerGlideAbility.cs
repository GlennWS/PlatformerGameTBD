using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerGlideAbility : MonoBehaviour
{
    [SerializeField] private float glideGravityScale = 2.0f;
    [SerializeField] private bool isUnlockedField = true;
    [SerializeField] private float glideFallSpeedMultiplier = 0.5f;
    [SerializeField] private float maxGlideFallSpeed = -2.0f;
    private Rigidbody2D rb;
    private float originalGravityScale;
    public bool IsActive { get; private set; } = false;

    public bool IsUnlocked
    {
        get { return isUnlockedField; }
        set { isUnlockedField = value; }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null)
        {
            originalGravityScale = pc.baseGravity;
        }
        else
        {
            originalGravityScale = rb.gravityScale;
        }
    }

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
        if (!IsUnlocked) return;
        if (IsActive) return;
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
        rb.gravityScale = originalGravityScale;
    }
}