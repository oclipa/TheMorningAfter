using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuCanvas : MonoBehaviour {

    private void Start()
    {
        SelectLoadButton();
    }

    private void OnEnable()
    {
        SelectLoadButton();
    }

    private static void SelectLoadButton()
    {
        // Ensure that LoadButton is selected by default
        GameObject button = GameObject.FindGameObjectWithTag("LoadButton");
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button);
    }
}
