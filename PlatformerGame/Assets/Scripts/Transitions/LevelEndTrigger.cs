using UnityEngine;

public class LevelEndTrigger : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private MusicProgression musicProgressionAsset;
    [SerializeField] private bool advanceMusic = false;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;

        if (advanceMusic && AudioManager.Instance != null && musicProgressionAsset != null)
        {
            AudioManager.Instance.StartMusicProgression(musicProgressionAsset);
        }

        if (LevelTracker.Instance != null)
        {
            LevelTracker.Instance.CompleteLevel();
        }
    }
}