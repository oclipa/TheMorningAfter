using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Displays the game settings
/// </summary>
public class SettingsMenu : MonoBehaviour {

    private GameState gameState;

	// Use this for initialization
	void Start () {

        this.gameState = Camera.main.GetComponent<GameState>();

        GameObject musicOption = GameObject.FindGameObjectWithTag("MusicToggle");
        GameObject effectsOption = GameObject.FindGameObjectWithTag("EffectsToggle");

        Toggle musicToggle = musicOption.GetComponent<Toggle>();
        Toggle effectsToggle = effectsOption.GetComponent<Toggle>();

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(musicOption);

        musicToggle.isOn = !gameState.DisableMusic;
        effectsToggle.isOn = !gameState.DisableSoundEffects;

        //Add listener for when the state of the Toggle changes, to take action
        musicToggle.onValueChanged.AddListener(delegate {
            MusicToggleValueChanged(musicToggle);
        });

        effectsToggle.onValueChanged.AddListener(delegate {
            EffectsToggleValueChanged(effectsToggle);
        });
    }

    //Output the new state of the Toggle into Text
    void MusicToggleValueChanged(Toggle change)
    {
        this.gameState.DisableMusic = !change.isOn;

        AudioManager.Instance.Stop();
        if (change.isOn)
            AudioManager.Instance.Play(AudioClipName.MenuMusic);
    }

    void EffectsToggleValueChanged(Toggle change)
    {
        this.gameState.DisableSoundEffects = !change.isOn;
    }

    private void OnDestroy()
    {
        this.gameState.Save();
    }

    public void HandleBackButtonOnClickEvent()
    {
        PlayClickSound();
        Back();
    }

    private void Back()
    {
        GameObject mainMenu = GameObject.Find(GameConstants.MAINMENU);
        GameObject mainMenuCanvas = mainMenu.transform.GetChild(0).gameObject;
        if (mainMenuCanvas != null)
            mainMenuCanvas.SetActive(true);
        Destroy(this.gameObject);
    }

    private static void PlayClickSound()
    {
        AudioManager.Instance.PlayOneShot(AudioClipName.Click);
    }
}
