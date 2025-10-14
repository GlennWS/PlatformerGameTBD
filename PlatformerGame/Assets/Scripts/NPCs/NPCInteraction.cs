using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [Header("Dialogue Settings")]
    public DialogueLine[] dialogueTree;

    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 3f;

    private PlayerController pc;
    private bool isPlayerInRange = false;

    private void Update()
    {
        if (pc == null) return;

        bool currentlyInRange = Vector3.Distance(transform.position, pc.transform.position) <= interactionRange;

        if (currentlyInRange != isPlayerInRange)
        {
            isPlayerInRange = currentlyInRange;
            if (isPlayerInRange)
            {
                pc.currentInteractable = this;
                Debug.Log("Player entered interaction range.");
            }
            else
            {
                if (pc.currentInteractable == this)
                {
                    pc.currentInteractable = null;
                }
                Debug.Log("Player exited interaction range.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            pc = other.GetComponent<PlayerController>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (pc != null)
            {
                if (pc.currentInteractable == this)
                {
                    pc.currentInteractable = null;
                }
                pc = null;
            }
            isPlayerInRange = false;
        }
    }

    public void StartInteraction()
    {
        if (!isPlayerInRange) return;
        if (DialogueManager.Instance.IsDialogueActive) return;

        DialogueManager.Instance.StartDialogue(dialogueTree);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
