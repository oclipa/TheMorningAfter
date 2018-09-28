using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        AudioManager.Stop();
        Time.timeScale = 0; // pauses the game
    }

    public void HandleResumeButtonOnClickEvent()
    {
        AudioManager.PlayOneShot(AudioClipName.Click);
        Time.timeScale = 1; // restarts the game
        IRoom currentRoom = Camera.main.GetComponent<RoomBuilder>().CurrentRoom;
        AudioManager.Play(currentRoom.GetMusic());
        Destroy(gameObject);
    }

    public void HandleQuitButtonOnClickEvent()
    {
        AudioManager.PlayOneShot(AudioClipName.Click);
        Time.timeScale = 1; // ensure that the game can run if restarted
        Destroy(gameObject);
        MenuManager.GoToMenu(MenuName.Main);
    }
}
