using System.Collections;
using UnityEngine;

public class PlayerBurstAbility : MonoBehaviour
{
    [Header("Burst Parameters")]
    [SerializeField] private float burstForce = 20f;
    [SerializeField] private float burstRadius = 1.5f;
    [SerializeField] private float burstDamage = 10f;
    [SerializeField] private float burstCooldown = 0.5f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Combo Multipliers")]
    [SerializeField] private float dashBurstMultiplier = 1.5f;
    [SerializeField] private float maxAbilitySpeed = 25f;

    private Rigidbody2D rb;
    private PlayerDashAbility dashAbility;
    private bool canBurst = true;
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
        dashAbility = pc.GetComponent<PlayerDashAbility>();
    }

    public void Burst(Vector2 aimDirection, float storedFacingDirection)
    {
        if (!canBurst || !IsUnlocked) return;

        Vector2 burstDirection;

        if (aimDirection.magnitude > 0.1f)
        {
            burstDirection = aimDirection.normalized;
            burstDirection = new Vector2(Mathf.Round(burstDirection.x), Mathf.Round(burstDirection.y));
        }
        else
        {
            burstDirection = new Vector2(storedFacingDirection, 0f);
        }

        StartCoroutine(BurstCoroutine(burstDirection));
    }

    private IEnumerator BurstCoroutine(Vector2 direction)
    {
        canBurst = false;
        IsActive = true;

        Vector2 baseForce = -direction * burstForce;
        Vector2 finalVelocity;

        float boostAmount = 1f;

        if (dashAbility != null && dashAbility.IsActive)
        {
            boostAmount = dashBurstMultiplier;
            finalVelocity = rb.linearVelocity + baseForce * boostAmount;
        }
        else
        {
            finalVelocity = baseForce;
        }

        finalVelocity.x = Mathf.Clamp(finalVelocity.x, -maxAbilitySpeed, maxAbilitySpeed);

        rb.linearVelocity = finalVelocity;

        ApplyBurstEffect(direction);

        yield return new WaitForFixedUpdate();

        IsActive = false;
        yield return new WaitForSeconds(burstCooldown);

        canBurst = true;
    }

    private void ApplyBurstEffect(Vector2 direction)
    {
        Vector3 explosionCenter = transform.position + (Vector3)direction * burstRadius * 0.5f;

        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(explosionCenter, burstRadius, enemyLayer);
        foreach (Collider2D target in hitTargets)
        {
            Debug.Log("Player Burst hit: " + target.name);
        }
    }
}