using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Original idea was that it might be worth differentiating between
/// normal rooms and portal rooms, hence this interface.
/// </summary>
public interface IRoom {

    /// <summary>
    /// Resets the state of the room based on a previously saved state
    /// (or the original state if no saved state is provided)
    /// </summary>
    /// <param name="gameState">Game state.</param>
    void Reset(GameState gameState = null);

    /// <summary>
    /// Add an item to this room (this means it is available to be collected)
    /// </summary>
    /// <param name="itemID">Item identifier.</param>
    void AddItem(string itemID);

    /// <summary>
    /// Collects the item (this removes it from the list of remaining items)
    /// </summary>
    /// <param name="itemID">Item identifier.</param>
    void CollectItem(string itemID);

    /// <summary>
    /// Add an creature to this room (this means it is available to be killed)
    /// </summary>
    /// <param name="creature">Creature identifier.</param>
    void AddCreature(string creature);

    /// <summary>
    /// Collects the creature (this removes it from the list of remaining creatures)
    /// </summary>
    /// <param name="creature">Creature identifier.</param>
    void KillCreature(string creature);

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
    /// Gets the total number of creature in this room.
    /// </summary>
    /// <returns>The creature count.</returns>
    int GetCreatureCount();

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

    /// <summary>
    /// Does this room have any uncollected items?
    /// </summary>
    /// <value><c>true</c> if has items; otherwise, <c>false</c>.</value>
    bool HasRemainingItems { get; }

    /// <summary>
    /// Gets the items currently in the room.
    /// </summary>
    /// <returns>The remaining items.</returns>
    ReadOnlyCollection<string> GetRemainingItems();

    /// <summary>
    /// Gets the items that have already been collected in the room.
    /// </summary>
    /// <returns>The collected items.</returns>
    ReadOnlyCollection<string> GetCollectedItems();

    /// <summary>
    /// Does this room have any unkilled creatures?
    /// </summary>
    /// <value><c>true</c> if has creatures; otherwise, <c>false</c>.</value>
    bool HasRemainingCreatures { get; }

    /// <summary>
    /// Gets the creatures currently in the room.
    /// </summary>
    /// <returns>The remaining creatures.</returns>
    ReadOnlyCollection<string> GetRemainingCreatures();

    /// <summary>
    /// Gets the creatures that have already been killed in the room.
    /// </summary>
    /// <returns>The killed creatures.</returns>
    ReadOnlyCollection<string> GetKilledCreatures();

    /// <summary>
    /// Gets the creature speed multiplier.
    /// </summary>
    /// <value>The creature speed multiplier.</value>
    float CreatureSpeedMultiplier { get; set; }

    /// <summary>
    /// Gets the player speed multiplier.
    /// </summary>
    /// <value>The player speed multiplier.</value>
    float PlayerSpeedMultiplier { get; set; }

    /// <summary>
    /// Gets the multiplier for the gravity X axis.
    /// </summary>
    /// <value>The gravity X multiplier.</value>
    float GravityXMultiplier { get; set; }

    /// <summary>
    /// Gets the multiplier for the gravity Y axis.
    /// </summary>
    /// <value>The gravity Y multiplier.</value>
    float GravityYMultiplier { get; set; }

    /// <summary>
    /// Gets the player control behaviour.
    /// </summary>
    /// <value>The player control behaviour.</value>
    PlayerControlBehavior PlayerControlBehavior { get; set; }

    /// <summary>
    /// Has the player visited the room?
    /// </summary>
    bool Visited { get; set; }

    /// <summary>
    /// Adds the room visited listener.
    /// </summary>
    /// <param name="listener">Listener.</param>
    void AddRoomVisitedListener(UnityAction<IRoom> listener);
}


