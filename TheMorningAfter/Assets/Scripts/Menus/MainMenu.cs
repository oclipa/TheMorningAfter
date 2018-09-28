using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
        AudioManager.Stop();
        AudioManager.Play(AudioClipName.MenuMusic);
	}

    public void HandlePlayButtonOnClickEvent()
    {
        AudioManager.PlayOneShot(AudioClipName.Click);
        SceneManager.LoadScene("Loading");
    }

    public void HandleQuitButtonOnClickEvent()
    {
        AudioManager.PlayOneShot(AudioClipName.Click);
        MenuManager.GoToMenu(MenuName.Quit);
    }

    public void HandleHelpButtonOnClickEvent()
    {
        AudioManager.PlayOneShot(AudioClipName.Click);
        GameObject mainMenuCanvas = GameObject.Find(GameConstants.MAINMENUCANVAS);
        if (mainMenuCanvas != null)
            mainMenuCanvas.SetActive(false);
        MenuManager.GoToMenu(MenuName.Help);
    }

    public void HandleBackButtonOnClickEvent()
    {
        AudioManager.PlayOneShot(AudioClipName.Click);
        MenuManager.GoToMenu(MenuName.Main);
    }
}
