using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudioSource : MonoBehaviour 
{
    // Uses Event Queue pattern

    //private struct PlaySound
    //{
    //    public AudioClipName clipName;
    //    public float volume;
    //    public bool isOneShot;
    //}

    //// using a ring buffer
    //const int MAX_PENDING = 16;
    //PlaySound[] pending;
    //int head;
    //int tail;

    //AudioSource audioSource;
    //Dictionary<AudioClipName, AudioClip> audioClips;

    //bool initialized;

    private void Awake()
    {
        // make sure we only have one of this game object in the game
        if (!AudioManager.Initialized)
        {
            // initialize audio manager and persist audio source across all scenes
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            AudioManager.Initialize(audioSource);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //private void Awake()
    //{
    //    // initialize audio manager and persist audio source across all scenes
    //    AudioSource audioSource = gameObject.AddComponent<AudioSource>();
    //    initialize(audioSource);
    //    DontDestroyOnLoad(gameObject);
    //}

    //private void initialize(AudioSource source)
    //{
    //    head = 0;
    //    tail = 0;

    //    pending = new PlaySound[MAX_PENDING];
    //    for (int i = 0; i < MAX_PENDING; i++)
    //        pending[i] = new PlaySound();

    //    audioSource = source;

    //    audioClips = new Dictionary<AudioClipName, AudioClip>();
    //    audioClips.Add(AudioClipName.Loading, Resources.Load<AudioClip>("Sounds/SpectrumLoadShort"));
    //    audioClips.Add(AudioClipName.MenuMusic, Resources.Load<AudioClip>("Sounds/MoonlightSonata"));
    //    audioClips.Add(AudioClipName.GameMusic, Resources.Load<AudioClip>("Sounds/IfIWereARichManEdit"));
    //    audioClips.Add(AudioClipName.Boing, Resources.Load<AudioClip>("Sounds/boing"));
    //    audioClips.Add(AudioClipName.Jump, Resources.Load<AudioClip>("Sounds/jump"));
    //    audioClips.Add(AudioClipName.Died, Resources.Load<AudioClip>("Sounds/exitroom"));
    //    audioClips.Add(AudioClipName.GameWon, Resources.Load<AudioClip>("Sounds/lullaby"));
    //    audioClips.Add(AudioClipName.GameLost, Resources.Load<AudioClip>("Sounds/gameover"));
    //    audioClips.Add(AudioClipName.Click, Resources.Load<AudioClip>("Sounds/click"));
    //    audioClips.Add(AudioClipName.ExitRoom, Resources.Load<AudioClip>("Sounds/exitroom"));

    //    initialized = true;
    //}

    //public void QueueSound(AudioClipName clipName, int volume, bool isOneShot)
    //{
    //    // If have an existing requests for the same sound, walk the 
    //    // pending requests and skip this one (but use the loudest volume)
    //    for (int i = head; i != tail; i = (i + 1) % MAX_PENDING)
    //    {
    //        if (pending[i].clipName == clipName)
    //        {
    //            // Use the larger of the two volumes
    //            pending[i].volume = Mathf.Max(volume, pending[i].volume);

    //            // Don't need to enqueue
    //            return;
    //        }
    //    }

    //    if ((tail + 1) % MAX_PENDING != head)
    //        return;

    //    pending[tail].clipName = clipName;
    //    pending[tail].volume = volume;
    //    pending[tail].isOneShot = isOneShot;
    //    tail = (tail + 1) % MAX_PENDING;
    //}

    //public void Update()
    //{
    //    if (head == tail)
    //        return;

    //    PlaySound playSound = pending[head];
    //    AudioClipName audioClipName = playSound.clipName;
    //    float volume = playSound.volume;
    //    bool isOneShot = playSound.isOneShot;

    //    if (isOneShot)
    //        audioSource.PlayOneShot(audioClips[audioClipName], volume);
    //    else


    //    head = (head + 1) % MAX_PENDING;
    //}


    //public void Play(AudioClipName name, bool loop = true, float volumeScale = 1.0f)
    //{
    //    play(audioClips[name], loop, volumeScale);
    //}

    //public void Play(string name, bool loop = true, float volumeScale = 1.0f)
    //{
    //    play(Resources.Load<AudioClip>("Sounds/" + name), loop, volumeScale);
    //}

    //private void play(AudioClip audioClip, bool loop, float volumeScale)
    //{
    //    audioSource.clip = audioClip;
    //    audioSource.loop = loop;
    //    audioSource.volume = volumeScale;
    //    audioSource.Play();
    //}

}
