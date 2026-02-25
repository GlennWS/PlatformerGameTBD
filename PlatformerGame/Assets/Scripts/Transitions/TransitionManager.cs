using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    [SerializeField] private SceneFade sceneFade;

    private string nextTransitionID = "";
    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public void LoadScene(string sceneName, string targetID)
    {
        if (isTransitioning) return;

        if (sceneFade == null)
        {
            Debug.LogError("SceneFade reference is missing in TransitionManager.");
            return;
        }

        StartCoroutine(TransitionCoroutine(sceneName, targetID));
    }

    private IEnumerator TransitionCoroutine(string sceneName, string targetID)
    {
        isTransitioning = true;

        yield return sceneFade.FadeToBlack();

        nextTransitionID = targetID;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;

        yield return null;

        yield return sceneFade.FadeToClear();

        isTransitioning = false;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (string.IsNullOrEmpty(nextTransitionID)) return;

        TransitionPoint spawnPoint = FindTransitionPoint(nextTransitionID);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        CameraController cameraController = FindAnyObjectByType<CameraController>();

        if (spawnPoint == null)
        {
            Debug.LogError($"Spawn point with ID '{nextTransitionID}' not found.");
            nextTransitionID = "";
            return;
        }

        if (player == null)
        {
            Debug.LogError("Player object not found in the scene.");
            nextTransitionID = "";
            return;
        }

        player.transform.position = (Vector2)spawnPoint.transform.position + spawnPoint.SpawnOffset;

        if (cameraController != null)
        {
            cameraController.SetTarget(player.transform);
        }
        else
        {
            Debug.LogError("CameraController not found on scene load.");
        }

        nextTransitionID = "";
    }

    private TransitionPoint FindTransitionPoint(string id)
    {
        TransitionPoint[] points = FindObjectsByType<TransitionPoint>(FindObjectsSortMode.None);

        foreach (TransitionPoint point in points)
        {
            if (point.TransitionID == id)
            {
                return point;
            }
        }

        return null;
    }
}