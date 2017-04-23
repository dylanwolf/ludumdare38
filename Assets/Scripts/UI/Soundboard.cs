using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("LudumDareResources/Sound/Sound Board")]
[RequireComponent(typeof(AudioSource))]
public class Soundboard : MonoBehaviour
{

    public static Soundboard Current;

    public static void Play(string clipName, float volume = 1.0f)
    {
        if (Current != null && Current.clips.ContainsKey(clipName) && Current.clips[clipName].Length > 0)
        {
            Current.PlayRandomSound(Current.clips[clipName], volume);
        }
    }

    public float ThrustersTime = 0.5f;
    static float thrustersTimer = 0;

    public static void ResetThrusters()
    {
        thrustersTimer = 0;
    }

    public static void PlayThrusters(float time, float volume)
    {
        thrustersTimer -= time;
        if (thrustersTimer < 0)
        {
            Play("Thrusters", volume);
            thrustersTimer = Current.ThrustersTime;
        }
    }

    public AudioClipLibrary[] Clips;

    [Serializable]
    public class AudioClipLibrary
    {
        public string Name;
        public AudioClip[] Clips;
    }

    Dictionary<string, AudioClip[]> clips = new Dictionary<string, AudioClip[]>();

    #region Unity Lifecycle
    AudioSource source;

    void Awake()
    {
        Current = this;
        source = GetComponent<AudioSource>();
    }

    void Start()
    {
        SetupClips();
    }
    #endregion

    void SetupClips()
    {
        clips.Clear();
        for (int i = 0; i < Clips.Length; i++)
            clips[Clips[i].Name] = Clips[i].Clips;
    }

    void PlayRandomSound(AudioClip[] clips, float volume)
    {
        PlaySound(clips[Mathf.Clamp(UnityEngine.Random.Range(0, clips.Length), 0, clips.Length - 1)], volume);
    }

    void PlaySound(AudioClip clip, float volume)
    {
        source.PlayOneShot(clip, volume);
    }
}
