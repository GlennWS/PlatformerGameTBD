using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }
    private string nextTransitionID = "";
    [SerializeField] private SceneFade sceneFade;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadScene(string sceneName, string targetID)
    {
        if (sceneFade == null)
        {
            Debug.LogError("SceneFade reference is missing in TransitionManager.");
            return;
        }
        
        StartCoroutine(TransitionCoroutine(sceneName, targetID));
    }

    private IEnumerator TransitionCoroutine(string sceneName, string targetID)
    {
        yield return sceneFade.FadeInCoroutine();
        nextTransitionID = targetID;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;

        yield return null;
        yield return sceneFade.FadeOutCoroutine();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (string.IsNullOrEmpty(nextTransitionID)) return;
        
        TransitionPoint spawnPoint = FindTransitionPoint(nextTransitionID);
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        CameraController cameraController = FindAnyObjectByType<CameraController>();

        if (spawnPoint != null && player != null)
        {
            player.transform.position = new Vector3(spawnPoint.transform.position.x + 15.0f, 
                spawnPoint.transform.position.y, spawnPoint.transform.position.z);

            if (cameraController != null)
            {
                cameraController.SetTarget(player.transform);
            }
            else
            {
                Debug.LogError("CameraController not found on scene load.");
            }
        }
        else
        {
            if (spawnPoint == null)
            {
                Debug.LogError($"Spawn point with ID '{nextTransitionID}' not found.");
            }
            if (player == null)
            {
                Debug.LogError("Player object not found in the scene.");
            }
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
        Debug.LogError($"TransitionPoint with ID '{id}' not found in the new scene!");
        return null;
    }
}