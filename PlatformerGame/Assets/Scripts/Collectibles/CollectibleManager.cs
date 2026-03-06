using UnityEngine;
using TMPro;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;

    [Header("Display")]
    [SerializeField] private string prefix = "Coins: ";

    private int totalCollected = 0;

    public int TotalCollected => totalCollected;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        UpdateDisplay();
    }

    public void Collect(int value)
    {
        totalCollected += value;
        UpdateDisplay();
    }

    public void ResetCount()
    {
        totalCollected = 0;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = prefix + totalCollected;
        }
    }
}