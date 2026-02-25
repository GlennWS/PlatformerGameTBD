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
    private bool isFading = false;
    private bool advancing = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        AudioSource activeSource = (primarySource.volume > secondarySource.volume) ? primarySource : secondarySource;

        if (activeSource.clip == null || !activeSource.isPlaying) return;

        if (activeSource.time >= currentStemEndTime)
        {
            if (advancing)
            {
                AdvanceStem();
            }
            activeSource.time = currentStemStartTime;
        }
    }
    public void StartMusicProgression(MusicProgression progression)
    {
        if (progression == null) return;

        if (progression != currentProgression)
        {
            currentProgression = progression;
            if (progression.stems.Count == 0) return;

            currentStemIndex = 0;
            currentStemStartTime = progression.stems[0].startTime;
            currentStemEndTime = progression.stems[0].endTime;

            primarySource.clip = progression.fullTrack;
            secondarySource.clip = progression.fullTrack;

            primarySource.time = currentStemStartTime;
            primarySource.volume = targetVolume;
            secondarySource.volume = 0f;

            primarySource.Play();
            secondarySource.Play();
        }
        else
        {
            AdvanceStem();
        }
    }

    private void AdvanceStem()
    {
        if (isFading) return;

        if (!advancing)
        {
            advancing = true;
            return;
        }

        int nextStemIndex = currentStemIndex + 1;
        if (nextStemIndex >= currentProgression.stems.Count)
        {
            return;
        }

        currentStemIndex = nextStemIndex;
        currentStemStartTime = currentProgression.stems[currentStemIndex].startTime;
        currentStemEndTime = currentProgression.stems[currentStemIndex].endTime;

        AudioSource oldSource = (primarySource.volume > secondarySource.volume) ? primarySource : secondarySource;
        AudioSource newSource = (oldSource == primarySource) ? secondarySource : primarySource;

        newSource.time = currentStemStartTime;
        newSource.volume = targetVolume;
        oldSource.volume = 0f;
        advancing = false;
    }
}