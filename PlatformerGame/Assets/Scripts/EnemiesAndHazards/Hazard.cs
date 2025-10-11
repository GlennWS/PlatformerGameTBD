using System.Collections;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float damageAmount = 25f;
    [SerializeField] private float damageInterval;
    private bool isPlayerInContact = false;

    [Header("Collision Settings")]
    [SerializeField] private bool destroyOnHit = false;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isPlayerInContact)
        {
            isPlayerInContact = true;
            IDamageable damageableTarget = other.GetComponent<IDamageable>();

            if (damageableTarget != null)
            {
                StartCoroutine(RepeatingDamage(damageableTarget));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isPlayerInContact)
        {
            StopAllCoroutines();
            isPlayerInContact = false;
        }
    }

    private IEnumerator RepeatingDamage(IDamageable target)
    {
        while (isPlayerInContact)
        {
            target.TakeDamage(damageAmount);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}