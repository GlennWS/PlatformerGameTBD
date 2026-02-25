using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    [SerializeField] private string transitionID;
    public string TransitionID => transitionID;

    [Header("Transition Settings")]
    [SerializeField] private string targetSceneName;
    [SerializeField] private string targetTransitionID;

    [Header("Spawn Settings")]
    [SerializeField] private Vector2 spawnOffset = Vector2.zero;
    public Vector2 SpawnOffset => spawnOffset;

    [Header("Music Progression")]
    [SerializeField] private MusicProgression musicProgressionAsset;
    [SerializeField] private bool advanceMusic = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (TransitionManager.Instance == null) return;
        if (string.IsNullOrEmpty(targetSceneName)) return;

        if (advanceMusic && AudioManager.Instance != null && musicProgressionAsset != null)
        {
            AudioManager.Instance.StartMusicProgression(musicProgressionAsset);
        }

        TransitionManager.Instance.LoadScene(targetSceneName, targetTransitionID);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 spawnPosition = transform.position + (Vector3)spawnOffset;
        Gizmos.DrawWireSphere(spawnPosition, 0.5f);
        Gizmos.DrawLine(transform.position, spawnPosition);
    }
}