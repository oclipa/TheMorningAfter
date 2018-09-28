using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour {

    string currentRoom;
    string nextRoom;
    NextRoomEvent nextRoomEvent;

    private void Start()
    {
        this.nextRoomEvent = new NextRoomEvent();
        EventManager.AddNextRoomInvoker(this);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(GameConstants.PLAYER))
            this.nextRoomEvent.Invoke(nextRoom, currentRoom);
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
    /// Adds the next room listener.
    /// </summary>
    /// <param name="listener">Listener.</param>
    public void AddNextRoomListener(UnityAction<string, string> listener)
    {
        nextRoomEvent.AddListener(listener);
    }
}
