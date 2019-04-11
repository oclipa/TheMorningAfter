using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Responds to button click to open the main menu
/// </summary>
public class GoToMainMenu : MonoBehaviour
{
    private CommandHandler commandHandler;
    private Timer timer;

    private void Awake()
    {
        // give player 0.5s to release the return button, in the event
        // that they still have it held down from the previous menu.
        timer = gameObject.AddComponent<Timer>();
        timer.Duration = 0.5f;

        this.commandHandler = new KeyboardCommandHandler();
    }

    // Use this for initialization
    void Start()
    {
        timer.Run();

        // set the QuitButton as the currently selected button
        GameObject button = GameObject.FindGameObjectWithTag("QuitButton");
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button);
    }

    // Update is called once per frame
    void Update()
    {
        // only allow quit button to be interacted with if
        // the timer has finished (this avoids a problem where
        // the player could still be holding down return from 
        // the previous menu and so immediately hit quit
        // on this menu).   
        if (timer.Finished && this.commandHandler.IsConfirmSelected())
        {
            gameObject.GetComponent<Button>().onClick.Invoke();
        }
    }
}
