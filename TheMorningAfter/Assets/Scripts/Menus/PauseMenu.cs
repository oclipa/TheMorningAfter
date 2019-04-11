using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// Displayed when the game is paused.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        AudioManager.Instance.Stop(); // stop all music
        Time.timeScale = 0; // pauses the game1

        GameObject resumeButton = GameObject.FindGameObjectWithTag("ResumeButton");
        GameObject.FindGameObjectWithTag("MainCameraEventSystem").GetComponent<EventSystem>().SetSelectedGameObject(resumeButton);
    }

    public void HandleResumeButtonOnClickEvent()
    {
        PlayClickSound();
        Resume();
    }

    private void Resume()
    {
        Time.timeScale = 1; // restarts the game
        IRoom currentRoom = Camera.main.GetComponent<RoomBuilder>().CurrentRoom;
        AudioManager.Instance.Play(currentRoom.GetMusic());
        Camera.main.GetComponent<PauseWatcher>().Resume();
        Destroy(gameObject);
    }


    public void HandleSaveButtonOnClickEvent()
    {
    }

    public void HandleQuitButtonOnClickEvent()
    {
        PlayClickSound();
        Quit();
    }

    private void Quit()
    {
        Time.timeScale = 1; // ensure that the game can run if restarted
        Destroy(gameObject);
        MenuManager.GoToMenu(MenuName.Main);
    }


    public void HandleLoadButtonOnClickEvent()
    {
        PlayClickSound();
        LoadGame();
    }

    private void LoadGame()
    {
        Time.timeScale = 1; // restarts the game
        // clear existing room blueprints
        Blueprints.ClearAll();
        SceneManager.LoadScene("Loading");
    }

    private static void PlayClickSound()
    {
        AudioManager.Instance.PlayOneShot(AudioClipName.Click);
    }
}
