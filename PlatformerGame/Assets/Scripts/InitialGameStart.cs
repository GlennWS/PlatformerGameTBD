using UnityEngine;

public class InitialGameStarter : MonoBehaviour
{
    [SerializeField] private MusicProgression initialProgression;
    void Start()
    {
        CameraController cameraController = FindAnyObjectByType<CameraController>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (cameraController != null && player != null)
        {
            cameraController.SetTarget(player.transform);
        }
        else
        {
            Debug.LogError("Failed to initialize camera target. Check for missing Camera or Player.");
        }

        if (AudioManager.Instance != null && initialProgression != null)
        {
            AudioManager.Instance.StartMusicProgression(initialProgression);
        }
        else
        {
            Debug.LogError("Failed to start music. AudioManager or Initial Progression asset is missing.");
        }
    }
}