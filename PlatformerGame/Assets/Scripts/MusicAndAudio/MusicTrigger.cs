using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    [SerializeField] private MusicProgression progressionAsset;
    [SerializeField] private bool advanceStem = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (AudioManager.Instance != null && progressionAsset != null)
            {
                AudioManager.Instance.StartMusicProgression(progressionAsset);
            }
        }
    }
}