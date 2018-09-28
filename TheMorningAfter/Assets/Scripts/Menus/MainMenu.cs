using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            GameObject quitButton = GameObject.FindGameObjectWithTag("QuitButton");
            if (quitButton != null)
            {
                Vector3 position = quitButton.transform.position;
                quitButton.GetComponent<Image>().enabled = false;

                GameObject helpButton = GameObject.FindGameObjectWithTag("HelpButton");
                helpButton.transform.position = position;
            }
        }

        GameObject backButton = GameObject.FindGameObjectWithTag("BackButton");
        if (backButton != null)
            GameObject.FindGameObjectWithTag("MainCameraEventSystem").GetComponent<EventSystem>().SetSelectedGameObject(backButton);


        AudioManager.Stop();
        AudioManager.Play(AudioClipName.MenuMusic);
	}

    bool hasKeyUp;

    private void Update()
    {
        if (GameObject.FindGameObjectWithTag("HelpMenu") != null && hasKeyUp)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                hasKeyUp = true;
                back();
            }
        }

        if (Input.GetKeyUp(KeyCode.Return))
            hasKeyUp = true;
    }

    public void HandlePlayButtonOnClickEvent()
    {
        play();
    }

    private static void play()
    {
        AudioManager.PlayOneShot(AudioClipName.Click);
        SceneManager.LoadScene("Loading");
    }

    public void HandleQuitButtonOnClickEvent()
    {
        quit();
    }

    private static void quit()
    {
        AudioManager.PlayOneShot(AudioClipName.Click);
        MenuManager.GoToMenu(MenuName.Quit);
    }

    public void HandleHelpButtonOnClickEvent()
    {
        help();
    }

    private static void help()
    {
        AudioManager.PlayOneShot(AudioClipName.Click);
        GameObject mainMenuCanvas = GameObject.Find(GameConstants.MAINMENUCANVAS);
        if (mainMenuCanvas != null)
            mainMenuCanvas.SetActive(false);
        MenuManager.GoToMenu(MenuName.Help);
    }

    public void HandleBackButtonOnClickEvent()
    {
        back();
    }

    private static void back()
    {
        AudioManager.PlayOneShot(AudioClipName.Click);
        MenuManager.GoToMenu(MenuName.Main);
    }
}
