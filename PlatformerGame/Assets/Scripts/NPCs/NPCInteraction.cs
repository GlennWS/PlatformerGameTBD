using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [SerializeField] private DialogueLine[] dialogueTree;

    private PlayerController pc;
    private bool isPlayerInRange = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent(out PlayerController player))
        {
            pc = player;
            isPlayerInRange = true;
            pc.currentInteractable = this;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (pc == null) return;

        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
        {
            return;
        }

        if (pc.currentInteractable == this)
        {
            pc.currentInteractable = null;
        }

        pc = null;
        isPlayerInRange = false;
    }

    public void StartInteraction()
    {
        if (!isPlayerInRange) return;
        if (DialogueManager.Instance == null || DialogueManager.Instance.IsDialogueActive) return;

        DialogueManager.Instance.StartDialogue(dialogueTree);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        CircleCollider2D trigger = GetComponent<CircleCollider2D>();
        if (trigger != null)
        {
            Gizmos.DrawWireSphere(transform.position, trigger.radius * transform.localScale.x);
        }
    }
}