using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages (most of) the audio in the game.
/// 
/// For further details on implementing the singleton pattern 
/// in C#, see :http://csharpindepth.com/Articles/General/Singleton.aspx
/// </summary>
public sealed class AudioManager
{
    private static AudioManager instance = null;
    private static readonly object padlock = new object();

    private bool initialized = false;
    private AudioSource audioSource;
    private Dictionary<AudioClipName, AudioClip> audioClips =
        new Dictionary<AudioClipName, AudioClip>();

    private string currentClip;

    private GameState gameState;

    AudioManager()
    {
    }

    public static AudioManager Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new AudioManager();
                }
                return instance;
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:AudioManager"/> is initialized.
    /// </summary>
    /// <value><c>true</c> if initialized; otherwise, <c>false</c>.</value>
    public bool Initialized
    {
        get { return initialized; }
    }

    /// <summary>
    /// Initialize the <see cref="T:AudioManager"/> using the specified source.
    /// </summary>
    /// <param name="source">Source.</param>
    public void Initialize(AudioSource source)
    {
        initialized = true;
        audioSource = source;
        audioClips.Add(AudioClipName.Loading, Resources.Load<AudioClip>("Sounds/SpectrumLoadShort"));
        audioClips.Add(AudioClipName.MenuMusic, Resources.Load<AudioClip>("Sounds/8BitIntroduction")); // ideally moonlight sonata, 3rd movement
        audioClips.Add(AudioClipName.GameMusic, Resources.Load<AudioClip>("Sounds/Happy8BitLoop")); // default; overridden in room config
        audioClips.Add(AudioClipName.Boing, Resources.Load<AudioClip>("Sounds/boing"));
        audioClips.Add(AudioClipName.Jump, Resources.Load<AudioClip>("Sounds/jumppp11"));
        audioClips.Add(AudioClipName.Died, Resources.Load<AudioClip>("Sounds/Hit_02"));
        audioClips.Add(AudioClipName.Chorus, Resources.Load<AudioClip>("Sounds/angelchorus"));
        audioClips.Add(AudioClipName.Fanfare, Resources.Load<AudioClip>("Sounds/fanfare"));
        audioClips.Add(AudioClipName.Alert, Resources.Load<AudioClip>("Sounds/alert"));
        audioClips.Add(AudioClipName.Door, Resources.Load<AudioClip>("Sounds/dooropen"));
        audioClips.Add(AudioClipName.Portal, Resources.Load<AudioClip>("Sounds/portal"));
        audioClips.Add(AudioClipName.GameWon, Resources.Load<AudioClip>("Sounds/lullaby"));
        audioClips.Add(AudioClipName.GameLost, Resources.Load<AudioClip>("Sounds/FuneralMarch"));
        audioClips.Add(AudioClipName.PickUp, Resources.Load<AudioClip>("Sounds/Pickup_Coin"));
        audioClips.Add(AudioClipName.PowerUp, Resources.Load<AudioClip>("Sounds/Pickup_Coin"));
        audioClips.Add(AudioClipName.Click, Resources.Load<AudioClip>("Sounds/click"));
        audioClips.Add(AudioClipName.Laser, Resources.Load<AudioClip>("Sounds/laser"));
        audioClips.Add(AudioClipName.Explosion, Resources.Load<AudioClip>("Sounds/Explosion"));
        audioClips.Add(AudioClipName.Oof, Resources.Load<AudioClip>("Sounds/oof"));

        GameState.OnDestruction += GameState_OnDestruction;
    }

    private void GameState_OnDestruction()
    {
        // make sure we don't hold onto a reference to a destroyed GameState
        gameState = null;
    }

    private GameState GetGameState()
    {
        if (gameState == null)
            gameState = Camera.main.GetComponent<GameState>();

        return gameState;
    }

    /// <summary>
    /// Plays the audio clip with the given name
    /// </summary>
    /// <param name="name">Name.</param>
    public void PlayOneShot(AudioClipName name, float volumeScale = 1.0f)
    {
        GameState currentGameState = GetGameState();
        if (currentGameState != null && !currentGameState.DisableSoundEffects)
            audioSource.PlayOneShot(audioClips[name], volumeScale);
    }

    public void Play(AudioClipName name, bool loop = true, float volumeScale = 1.0f)
    {
        PlayClip(audioClips[name], loop, volumeScale);
    }

    public void Play(string name, bool loop = true, float volumeScale = 1.0f)
    {
        PlayClip(Resources.Load<AudioClip>("Sounds/" + name), loop, volumeScale);
    }

    private void PlayClip(AudioClip audioClip, bool loop, float volumeScale)
    {
        if (currentClip != audioClip.name)
        {
            currentClip = audioClip.name;

            GameState currentGameState = GetGameState();
            if (currentGameState != null && !currentGameState.DisableMusic)
            {
                audioSource.clip = audioClip;
                audioSource.loop = loop;
                audioSource.volume = volumeScale;
                audioSource.Play();
            }
            else
            {
            }
        }
        else
        {
        }
    }

    public void Stop()
    {
        if (audioSource != null)
            audioSource.Stop();

        currentClip = string.Empty;
    }
}
