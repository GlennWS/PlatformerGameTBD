using UnityEngine;
using TMPro;

public class LevelReviewManager : MonoBehaviour
{
    public static LevelReviewManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject reviewPanel;
    [SerializeField] private TMP_Text levelNameText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text collectiblesText;

    [Header("Next Level")]
    [SerializeField] private string nextSceneName;
    [SerializeField] private string nextTransitionID;

    public bool IsReviewActive { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Object.DontDestroyOnLoad(transform.root.gameObject);

        if (reviewPanel != null)
        {
            reviewPanel.SetActive(false);
        }
    }

    public void ShowReview(LevelTracker tracker)
    {
        if (reviewPanel == null) return;

        IsReviewActive = true;
        reviewPanel.SetActive(true);
        Time.timeScale = 0f;

        if (levelNameText != null)
        {
            levelNameText.text = tracker.LevelName;
        }

        if (timeText != null)
        {
            timeText.text = FormatTime(tracker.CompletionTime);
        }

        if (collectiblesText != null)
        {
            collectiblesText.text = tracker.Collected + " / " + tracker.TotalAvailable;
        }
    }

    public void Continue()
    {
        IsReviewActive = false;
        reviewPanel.SetActive(false);
        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(nextSceneName) && TransitionManager.Instance != null)
        {
            TransitionManager.Instance.LoadScene(nextSceneName, nextTransitionID);
        }
    }

    private string FormatTime(float totalSeconds)
    {
        if (totalSeconds < 60f)
        {
            return totalSeconds.ToString("F1") + "s";
        }

        int minutes = Mathf.FloorToInt(totalSeconds / 60f);
        float seconds = totalSeconds % 60f;
        return minutes + ":" + seconds.ToString("00.0");
    }
}