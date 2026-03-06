using UnityEngine;

public class LevelTracker : MonoBehaviour
{
    public static LevelTracker Instance { get; private set; }

    [Header("Level Info")]
    [SerializeField] private string levelName = "Zone 1 - Factory";

    private float startTime;
    private float completionTime;
    private int collected = 0;
    private int totalAvailable = 0;
    private bool isComplete = false;
    private bool isTracking = false;

    public string LevelName => levelName;
    public float CompletionTime => completionTime;
    public int Collected => collected;
    public int TotalAvailable => totalAvailable;
    public bool IsComplete => isComplete;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        StartTracking();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void RegisterCollectible(int value)
    {
        totalAvailable += value;
    }

    public void CollectItem(int value)
    {
        if (isComplete) return;
        collected += value;
    }

    public void StartTracking()
    {
        startTime = Time.time;
        isTracking = true;
        isComplete = false;
    }

    public void CompleteLevel()
    {
        if (isComplete || !isTracking) return;

        isComplete = true;
        isTracking = false;
        completionTime = Time.time - startTime;

        if (LevelReviewManager.Instance != null)
        {
            LevelReviewManager.Instance.ShowReview(this);
        }
    }
}