using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseWatcher : MonoBehaviour {
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuManager.GoToMenu(MenuName.Pause);
        }
	}
}
