using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    private GameState gameState;

    //GameObject loadButton;
    //GameObject playButton;
    //GameObject settingsButton;
    GameObject quitButton;
    GameObject helpButton;
    //GameObject creditsButton;

    // Use this for initialization
    void Start () 
    {
        // if WebGl, we don't want to display the Quit button, so 
        // we will replace it with the help button.
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            //loadButton = GameObject.FindGameObjectWithTag("LoadButton");
            //playButton = GameObject.FindGameObjectWithTag("PlayButton");
            //settingsButton = GameObject.FindGameObjectWithTag("SettingsButton");
            quitButton = GameObject.FindGameObjectWithTag("QuitButton");
            helpButton = GameObject.FindGameObjectWithTag("HelpButton");
            //creditsButton = GameObject.FindGameObjectWithTag("CreditsButton");

            if (quitButton != null)
                Destroy(quitButton);

            helpButton.transform.localPosition = new Vector3(0f, -150f, 0f);
        }

        gameState = Camera.main.GetComponent<GameState>();
        gameState.Load(); // load saved state

        AudioManager.Instance.Play(AudioClipName.MenuMusic);
	}

    public void HandlePlayButtonOnClickEvent()
    {
        PlayClickSound();

        // first check if there is already a saved game
        if (IsGameActive())
            PromptToOverwrite();
        else // if not previously saved game, just go ahead and start the new game
            NewGame();
    }

    private static void PromptToOverwrite()
    {
        // if there is a saved game, give the player the option to cancel
        // (otherwise they will overwrite the saved game)
        MenuManager.GoToMenu(MenuName.Overwrite);

        // Ensure back button is selected by default so player
        // has less chance of overwriting the saved game by accident
        GameObject button = GameObject.FindGameObjectWithTag("BackButton");
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button);
    }

    /// <summary>
    /// A game is assumed to be active if:
    /// a) either, the player is not in the bathroom
    /// b) or, they are in the bathroom but they have collected one or more items
    /// </summary>
    /// <returns><c>true</c>, if game active was ised, <c>false</c> otherwise.</returns>
    private bool IsGameActive()
    {
        return !string.IsNullOrEmpty(gameState.CurrentRoom) && (!gameState.CurrentRoom.Equals(GameConstants.DefaultRoom) || gameState.ItemsCollected > 0);
    }

    public void HandleContinueButtonOnClickEvent()
    {
        PlayClickSound();
        NewGame();
    }

    public void HandleLoadButtonOnClickEvent()
    {
        PlayClickSound();

        // if there is an active game, load that game, otherwise prompt to start a new game
        if (IsGameActive())
            LoadGame();
        else
            NewGame();
    }

    private void NewGame()
    {
        // delete any previously saved game
        Blueprints.ClearAll();
        gameState.DeleteAll();
        gameState.Save();

        ChooseDifficulty();
    }

    private void ChooseDifficulty()
    {
        // show the difficulty chooser menu
        MenuManager.GoToMenu(MenuName.Difficulty);

        // Ensure medium button is selected by default
        GameObject button = GameObject.FindGameObjectWithTag("EasyButton");
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button);
    }

    public void EasyGame()
    {
        gameState.DifficultyMultiplier = GameConstants.EasyDifficulty;
        gameState.Save();

        LoadGame();
    }

    public void MediumGame()
    {
        gameState.DifficultyMultiplier = GameConstants.MediumDifficulty;
        gameState.Save();

        LoadGame();
    }

    public void HardGame()
    {
        gameState.DifficultyMultiplier = GameConstants.HardDifficulty;
        gameState.Save();

        LoadGame();
    }

    private void LoadGame()
    {
        // clear existing room blueprints
        Blueprints.ClearAll();

        // during the loading scene, the config files will be read.
        SceneManager.LoadScene("Loading");
    }

    public void HandleQuitButtonOnClickEvent()
    {
        PlayClickSound();
        Quit();
    }

    private void Quit()
    {
        MenuManager.GoToMenu(MenuName.Quit);
    }

    private static void HideMainMenu()
    {
        GameObject mainMenuCanvas = GameObject.Find(GameConstants.MAINMENUCANVAS);
        if (mainMenuCanvas != null)
            mainMenuCanvas.SetActive(false);
    }

    public void HandleHelpButtonOnClickEvent()
    {
        PlayClickSound();
        Help();
    }

    private static void Help()
    {
        HideMainMenu();
        MenuManager.GoToMenu(MenuName.Help);
    }

    public void HandleCreditsButtonOnClickEvent()
    {
        PlayClickSound();
        Credits();
    }

    private static void Credits()
    {
        HideMainMenu();
        MenuManager.GoToMenu(MenuName.Credits);
    }

    public void HandleSettingsButtonOnClickEvent()
    {
        PlayClickSound();
        Settings();
    }

    private static void Settings()
    {
        HideMainMenu();
        MenuManager.GoToMenu(MenuName.Settings);
    }

    public void HandleBackButtonOnClickEvent()
    {
        PlayClickSound();
        Back();
    }

    private void Back()
    {
        MenuManager.GoToMenu(MenuName.Main);
    }

    private static void PlayClickSound()
    {
        AudioManager.Instance.PlayOneShot(AudioClipName.Click);
    }
}
