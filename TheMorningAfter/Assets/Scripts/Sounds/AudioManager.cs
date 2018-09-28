using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioManager
{
    static bool initialized = false;
    static AudioSource audioSource;
    static Dictionary<AudioClipName, AudioClip> audioClips =
        new Dictionary<AudioClipName, AudioClip>();

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:AudioManager"/> is initialized.
    /// </summary>
    /// <value><c>true</c> if initialized; otherwise, <c>false</c>.</value>
    public static bool Initialized
    {
        get { return initialized; }
    }

    /// <summary>
    /// Initialize the <see cref="T:AudioManager"/> using the specified source.
    /// </summary>
    /// <param name="source">Source.</param>
    public static void Initialize(AudioSource source)
    {
        initialized = true;
        audioSource = source;
        audioClips.Add(AudioClipName.Loading, Resources.Load<AudioClip>("Sounds/SpectrumLoadShort"));
        audioClips.Add(AudioClipName.MenuMusic, Resources.Load<AudioClip>("Sounds/MoonlightSonata"));
        audioClips.Add(AudioClipName.GameMusic, Resources.Load<AudioClip>("Sounds/IfIWereARichManEdit"));
        audioClips.Add(AudioClipName.Boing, Resources.Load<AudioClip>("Sounds/boing"));
        audioClips.Add(AudioClipName.Jump, Resources.Load<AudioClip>("Sounds/jump"));
        audioClips.Add(AudioClipName.Died, Resources.Load<AudioClip>("Sounds/exitroom"));
        audioClips.Add(AudioClipName.GameWon, Resources.Load<AudioClip>("Sounds/lullaby"));
        audioClips.Add(AudioClipName.GameLost, Resources.Load<AudioClip>("Sounds/gameover"));
        audioClips.Add(AudioClipName.Click, Resources.Load<AudioClip>("Sounds/click"));
        audioClips.Add(AudioClipName.ExitRoom, Resources.Load<AudioClip>("Sounds/exitroom"));
    }

    /// <summary>
    /// Plays the audio clip with the given name
    /// </summary>
    /// <param name="name">Name.</param>
    public static void PlayOneShot(AudioClipName name, float volumeScale = 1.0f)
    {
        audioSource.PlayOneShot(audioClips[name], volumeScale);
    }

    public static void Play(AudioClipName name, bool loop = true, float volumeScale = 1.0f)
    {
        play(audioClips[name], loop, volumeScale);
    }

    public static void Play(string name, bool loop = true, float volumeScale = 1.0f)
    {
        play(Resources.Load<AudioClip>("Sounds/" + name), loop, volumeScale);
    }

    private static void play(AudioClip audioClip, bool loop, float volumeScale)
    {
        audioSource.clip = audioClip;
        audioSource.loop = loop;
        audioSource.volume = volumeScale;
        audioSource.Play();
    }

    public static void Stop()
    {
        if (audioSource != null)
            audioSource.Stop();
    }
}
