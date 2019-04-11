using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles door behaviour
/// </summary>
public class Door : MonoBehaviour {

    string currentRoom; // the room from which this door exits
    string nextRoom; // the room to which this door leads
    bool playSound; // should the door play a sound effect?
    NextRoomEvent nextRoomEvent; // triggered when the door is used

    private void Start()
    {
        this.nextRoomEvent = new NextRoomEvent();
        EventManager.AddNextRoomInvoker(this);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // if player exits the door, let the RoomBuilder know to build
        // the next room
        if (collision.gameObject.CompareTag(GameConstants.PLAYER))
        {
            // if there is no next room, show the "under construction" message
            if (nextRoom.Equals(currentRoom) || Blueprints.GetRoom(nextRoom) == null)
                ShowMissingRoomMessage();

            this.nextRoomEvent.Invoke(nextRoom, currentRoom, playSound);
        }
    }

    /// <summary>
    /// The rooms this door leads to
    /// </summary>
    /// <value>The next room.</value>
    public string NextRoom
    {
        get { return this.nextRoom; }
        set { this.nextRoom = value; }
    }

    /// <summary>
    /// The rooms this door exits from
    /// </summary>
    /// <value>The next room.</value>
    public string CurrentRoom
    {
        get { return this.currentRoom; }
        set { this.currentRoom = value; }
    }

    /// <summary>
    /// Should this door make a sound when opened?
    /// </summary>
    /// <value>The next room.</value>
    public bool PlaySound
    {
        get { return this.playSound; }
        set { this.playSound = value; }
    }

    /// <summary>
    /// Adds the next room listener.
    /// </summary>
    /// <param name="listener">Listener.</param>
    public void AddNextRoomListener(UnityAction<string, string, bool> listener)
    {
        nextRoomEvent.AddListener(listener);
    }

    /// <summary>
    /// Displayed when the next room is not available
    /// </summary>
    public void ShowMissingRoomMessage()
    {
        Object.Instantiate(Resources.Load("Menus/MissingRoomMenu"));
        Time.timeScale = 0; // pauses the game
    }
}
