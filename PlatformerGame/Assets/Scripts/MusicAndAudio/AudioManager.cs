using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource primarySource;
    [SerializeField] private AudioSource secondarySource;

    [Header("Settings")]
    [SerializeField] private float targetVolume = 1f;

    private MusicProgression currentProgression;
    private int currentStemIndex = 0;
    private float currentStemStartTime = 0f;
    private float currentStemEndTime = 0f;
    private bool advancing = false;

    private AudioSource activeSource;
    private AudioSource inactiveSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        activeSource = primarySource;
        inactiveSource = secondarySource;
    }

    private void Update()
    {
        if (activeSource.clip == null || !activeSource.isPlaying) return;

        float lookahead = Time.deltaTime;

        if (activeSource.time + lookahead >= currentStemEndTime)
        {
            if (advancing)
            {
                TransitionToNextStem();
                return;
            }

            activeSource.time = currentStemStartTime;
        }
    }

    public void StartMusicProgression(MusicProgression progression)
    {
        if (progression == null) return;

        if (progression != currentProgression)
        {
            StopAllCoroutines();

            currentProgression = progression;
            if (progression.stems.Count == 0) return;

            if (!progression.stems[0].IsValid)
            {
                Debug.LogError($"Invalid stem at index 0: start must be less than end.");
                return;
            }

            currentStemIndex = 0;
            currentStemStartTime = progression.stems[0].startTime;
            currentStemEndTime = progression.stems[0].endTime;
            advancing = false;

            activeSource = primarySource;
            inactiveSource = secondarySource;

            activeSource.clip = progression.fullTrack;
            inactiveSource.clip = progression.fullTrack;

            activeSource.time = currentStemStartTime;
            activeSource.volume = targetVolume;
            inactiveSource.volume = 0f;

            activeSource.Play();
            inactiveSource.Play();
        }
        else
        {
            RequestAdvance();
        }
    }

    private void RequestAdvance()
    {
        if (advancing) return;

        int nextStemIndex = currentStemIndex + 1;
        if (nextStemIndex >= currentProgression.stems.Count) return;

        advancing = true;
    }

    private void TransitionToNextStem()
    {
        int nextStemIndex = currentStemIndex + 1;
        if (nextStemIndex >= currentProgression.stems.Count)
        {
            advancing = false;
            activeSource.time = currentStemStartTime;
            return;
        }

        MusicStem currentStem = currentProgression.stems[currentStemIndex];
        MusicStem nextStem = currentProgression.stems[nextStemIndex];

        if (!nextStem.IsValid)
        {
            Debug.LogError($"Invalid stem at index {nextStemIndex}: start must be less than end.");
            advancing = false;
            activeSource.time = currentStemStartTime;
            return;
        }

        currentStemIndex = nextStemIndex;
        currentStemStartTime = nextStem.startTime;
        currentStemEndTime = nextStem.endTime;
        advancing = false;

        bool isContiguous = Mathf.Abs(nextStem.startTime - currentStem.endTime) < 0.01f;

        if (isContiguous)
        {
            return;
        }

        StartCoroutine(CrossfadeToPosition(nextStem.startTime));
    }

    private IEnumerator CrossfadeToPosition(float targetTime)
    {
        float duration = currentProgression.fadeDuration;

        inactiveSource.time = targetTime;
        inactiveSource.volume = 0f;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            activeSource.volume = Mathf.Lerp(targetVolume, 0f, t);
            inactiveSource.volume = Mathf.Lerp(0f, targetVolume, t);

            yield return null;
        }

        activeSource.volume = 0f;
        inactiveSource.volume = targetVolume;

        (activeSource, inactiveSource) = (inactiveSource, activeSource);
    }
}