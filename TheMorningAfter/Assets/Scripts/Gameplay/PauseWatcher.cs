using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Watches for the player to issue a pause command
/// </summary>
public class PauseWatcher : MonoBehaviour {

    private CommandHandler commandHandler;
    private bool isPaused;

    private void Start()
    {
        this.commandHandler = new KeyboardCommandHandler();
    }

    // Update is called once per frame
    void Update () 
    {
        if (!isPaused && this.commandHandler.IsEscapeSelected())
        {
            isPaused = true;
            MenuManager.GoToMenu(MenuName.Pause);
        }
	}

    public void Resume()
    {
        isPaused = false;
    }
}
