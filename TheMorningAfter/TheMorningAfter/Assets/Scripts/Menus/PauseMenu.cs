using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        AudioManager.Stop();
        Time.timeScale = 0; // pauses the game1

        GameObject resumeButton = GameObject.FindGameObjectWithTag("ResumeButton");
        GameObject.FindGameObjectWithTag("MainCameraEventSystem").GetComponent<EventSystem>().SetSelectedGameObject(resumeButton);
    }

    public void HandleResumeButtonOnClickEvent()
    {
        resume();
    }

    private void resume()
    {
        AudioManager.PlayOneShot(AudioClipName.Click);
        Time.timeScale = 1; // restarts the game
        IRoom currentRoom = Camera.main.GetComponent<RoomBuilder>().CurrentRoom;
        AudioManager.Play(currentRoom.GetMusic());
        Camera.main.GetComponent<PauseWatcher>().Resume();
        Destroy(gameObject);
    }

    public void HandleQuitButtonOnClickEvent()
    {
        quit();
    }

    private void quit()
    {
        AudioManager.PlayOneShot(AudioClipName.Click);
        Time.timeScale = 1; // ensure that the game can run if restarted
        Destroy(gameObject);
        MenuManager.GoToMenu(MenuName.Main);
    }
}
