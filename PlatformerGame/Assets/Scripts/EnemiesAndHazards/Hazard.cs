using UnityEngine;

public class Hazard : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float damageAmount = 25f;

    [Header("Collision Settings")]
    [SerializeField] private bool destroyOnHit = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damageAmount);

            if (destroyOnHit)
            {
                Destroy(gameObject);
            }
        }
    }
}