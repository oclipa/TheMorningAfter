using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Displayed when the player tries to access a room that has not been loaded
/// </summary>
public class MissingRoomMenu : MonoBehaviour {

    private void Start()
    {
        GameObject backButton = GameObject.FindGameObjectWithTag("MissingRoomMessageButton");
        GameObject.FindGameObjectWithTag("MainCameraEventSystem").GetComponent<EventSystem>().SetSelectedGameObject(backButton);
    }

    public void CloseMissingRoomMessage()
    {
        Destroy(gameObject);

        Time.timeScale = 1; // restarts the game
    }
}
