using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    [SerializeField] private MusicProgression progressionAsset;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;
        if (AudioManager.Instance == null || progressionAsset == null) return;

        hasTriggered = true;
        AudioManager.Instance.StartMusicProgression(progressionAsset);
    }
}