using System.Collections;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float damageAmount = 25f;
    [SerializeField] private float damageInterval = 1.0f;
    private bool isPlayerInContact = false;
    private Coroutine damageCoroutine;

    [Header("Collision Settings")]
    [SerializeField] private bool destroyOnHit = false;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isPlayerInContact) return;

        isPlayerInContact = true;

        if (other.TryGetComponent(out IDamageable target))
        {
            if (destroyOnHit)
            {
                target.TakeDamage(damageAmount);
                Destroy(gameObject);
                return;
            }

            damageCoroutine = StartCoroutine(RepeatingDamage(target));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isPlayerInContact)
        {
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
            isPlayerInContact = false;
        }
    }

    private IEnumerator RepeatingDamage(IDamageable target)
    {
        while (isPlayerInContact)
        {
            if (target == null || target.Equals(null)) yield break;

            target.TakeDamage(damageAmount);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}