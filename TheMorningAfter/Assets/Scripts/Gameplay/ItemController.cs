using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemController : MonoBehaviour {

    private int points;
    private string itemID;
    private string roomID;

    ItemCollectedEvent itemCollectedEvent;

    private void Start()
    {
        this.itemCollectedEvent = new ItemCollectedEvent();
        EventManager.AddItemCollectedInvoker(this);
    }

    ///// <summary>
    ///// The number of points associated with this item
    ///// </summary>
    ///// <value>The points.</value>
    //public int Points
    //{
    //    get 
    //    {
    //        return this.points;
    //    }
    //    set
    //    {
    //        this.points = value;
    //    }
    //}

    /// <summary>
    /// Unique identifier for this item
    /// </summary>
    /// <value>The identifier.</value>
    public string ID
    {
        get { return this.itemID; }
        set { this.itemID = value; }
    }

    /// <summary>
    /// The room to which this item belongs
    /// </summary>
    /// <value>The room identifier.</value>
    public string RoomID
    {
        get { return this.roomID; }
        set { this.roomID = value; }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(GameConstants.PLAYER))
        {
            AudioManager.PlayOneShot(AudioClipName.Click);
            this.itemCollectedEvent.Invoke(this.itemID);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Adds the item collected listener.
    /// </summary>
    /// <param name="listener">Listener.</param>
    public void AddItemCollectedListener(UnityAction<string> listener)
    {
        itemCollectedEvent.AddListener(listener);
    }
}
