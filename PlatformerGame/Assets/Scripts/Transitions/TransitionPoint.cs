using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    [SerializeField] private string transitionID;
    public string TransitionID => transitionID;

    [Header("Transition Settings")]
    [SerializeField] private string targetSceneName;
    [SerializeField] private string targetTransitionID;

    [Header("Music Progression")]
    [SerializeField] private MusicProgression musicProgressionAsset;
    [SerializeField] private bool advanceMusic = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && TransitionManager.Instance != null)
        {
            if (!string.IsNullOrEmpty(targetSceneName))
            {
                if (advanceMusic && AudioManager.Instance != null && musicProgressionAsset != null)
                {
                    AudioManager.Instance.StartMusicProgression(musicProgressionAsset);
                }

                TransitionManager.Instance.LoadScene(targetSceneName, targetTransitionID);
            }
        }
    }
}