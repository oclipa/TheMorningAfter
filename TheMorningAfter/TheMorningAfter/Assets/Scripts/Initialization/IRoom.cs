using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public interface IRoom {

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <returns>The name.</returns>
    string GetName();

    /// <summary>
    /// Gets the unique id for this room.
    /// </summary>
    /// <returns>The id.</returns>
    string GetID();

    /// <summary>
    /// Gets the music for this room.
    /// </summary>
    /// <returns>The id.</returns>
    string GetMusic();

    /// <summary>
    /// Gets the total number of items in this room.
    /// </summary>
    /// <returns>The item count.</returns>
    int GetItemCount();

    /// <summary>
    /// Gets the room details.
    /// </summary>
    /// <returns>The room details.</returns>
    ReadOnlyCollection<string> GetRoomDetails();

    /// <summary>
    /// Gets the start position for the player
    /// </summary>
    /// <returns>The start position.</returns>
    Vector3 GetStartPosition(string enteringFrom);

    bool HasItems { get; }

    bool AddItem(string itemID);
}
