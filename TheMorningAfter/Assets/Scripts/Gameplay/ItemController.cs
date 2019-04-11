using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles the behaviour of collectable items
/// </summary>
public class ItemController : MonoBehaviour {

    // the number of points associated with the item (not used)
    private int points;

    // the unique id for the item
    private string itemID;

    // the id for the room in which the item is present
    private string roomID;

    // notify that an item has been collected
    ItemCollectedEvent itemCollectedEvent;

    // notify that a powerup has been collected
    PowerUpEvent powerUpEvent;

    private void Start()
    {
        this.itemCollectedEvent = new ItemCollectedEvent();
        EventManager.AddItemCollectedInvoker(this);

        this.powerUpEvent = new PowerUpEvent();
        EventManager.AddPowerUpInvoker(this);
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
        // only interested in collisions with player
        if (collision.gameObject.CompareTag(GameConstants.PLAYER))
        {
            // powerups are special cases
            if (this.CompareTag(GameConstants.POWERUP))
            {
                AudioManager.Instance.PlayOneShot(AudioClipName.PowerUp);
                if (this.name.Equals(GameConstants.EXTRALIFE))
                {
                    this.powerUpEvent.Invoke(this.itemID);
                }
                else if (this.name.Equals(GameConstants.WEAPON))
                {
                    this.powerUpEvent.Invoke(this.itemID);
                }
            }
            else // just a normal item
            {
                AudioManager.Instance.PlayOneShot(AudioClipName.PickUp);
                this.itemCollectedEvent.Invoke(this.itemID);
            }
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

    /// <summary>
    /// Adds the power up listener.
    /// </summary>
    /// <param name="listener">Listener.</param>
    public void AddPowerUpListener(UnityAction<string> listener)
    {
        powerUpEvent.AddListener(listener);
    }
}
