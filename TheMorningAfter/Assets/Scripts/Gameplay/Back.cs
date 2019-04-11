using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Back : MonoBehaviour {

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
    void Start () {
        timer.Run();

        // set the BackButton as the currently selected button
        GameObject button = GameObject.FindGameObjectWithTag("BackButton");
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button);
    }

    // Update is called once per frame
    void Update () {

        // only allow back button to be interacted with if
        // the timer has finished (this avoids a problem where
        // the player could still be holding down return from 
        // the previous menu and so immediately hit back
        // on this menu).	
        if (timer.Finished && this.commandHandler.IsConfirmSelected())
        {
            gameObject.GetComponent<Button>().onClick.Invoke();
        }
    }
}
