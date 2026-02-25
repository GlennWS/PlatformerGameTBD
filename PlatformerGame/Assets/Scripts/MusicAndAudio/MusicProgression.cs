using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public struct MusicStem
{
    public float startTime;
    public float endTime;

    public bool IsValid => endTime > startTime;
}

[CreateAssetMenu(fileName = "NewMusicProgression", menuName = "Audio/Variable-Time Music Progression")]
public class MusicProgression : ScriptableObject
{
    public AudioClip fullTrack;
    public List<MusicStem> stems = new List<MusicStem>();
    public float fadeDuration = 1.5f;
}