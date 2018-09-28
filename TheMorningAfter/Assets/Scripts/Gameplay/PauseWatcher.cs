using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseWatcher : MonoBehaviour {

    bool isPaused;

	// Update is called once per frame
	void Update () 
    {
        if (!isPaused && Input.GetKeyDown(KeyCode.Escape))
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
