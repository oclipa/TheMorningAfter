using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Event manager.  
/// 
/// This is good enough for this small game, however
/// for more complex games, or ones where multi-threading becomes
/// important, having a single static class handling all events 
/// may (will) be a problem.
/// 
/// It would be more scaleable to implement events using native .NET 
/// EventHandlers or Actions.
/// 
/// As a side-note, the following is an excellent blog post discussing
/// the pros and cons of EventHandlers vs Actions: 
/// http://www.paulrohde.com/events-eventhandler-or-action/
/// 
/// 
/// Note that the only real advantage of using UnityAction (or UnityEvent)
/// is that they can be serialized by the Unity Inspector, so you can
/// assign the object and method to notify via drag and drop.
/// </summary>
public static class EventManager
{
    // The ItemController notifies listeners when an item is collected.
    static List<ItemController> itemCollectedInvokers = new List<ItemController>();
    static List<UnityAction<string>> itemCollectedListeners = new List<UnityAction<string>>();

    // The ObstacleController notifies listeners when a creature is killed.
    static List<ObstacleController> creatureKilledInvokers = new List<ObstacleController>();
    static List<UnityAction<string>> creatureKilledListeners = new List<UnityAction<string>>();

    // The ItemController notifies listeners when a power up has been obtained.
    static List<ItemController> powerUpInvokers = new List<ItemController>();
    static List<UnityAction<string>> powerUpListeners = new List<UnityAction<string>>();

    // The PlayerController notifies listeners when the player dies.
    static List<IPlayerDiedInvoker> playerDiedInvokers = new List<IPlayerDiedInvoker>();
    static List<UnityAction<Vector3>> playerDiedListeners = new List<UnityAction<Vector3>>();

    // The GameStatusWatcher notifies listeners when all lives have been used up.
    // The event includes the final score.
    static List<IGameOverInvoker> gameOverInvokers = new List<IGameOverInvoker>();
    static List<UnityAction<int>> gameOverListeners = new List<UnityAction<int>>();

    // The Door checks to see if the player has gone outside the bounds of
    // the rooms (which means they have moved to the next room).
    // The event includes the new room id.
    static List<Door> nextRoomInvokers = new List<Door>();
    static List<UnityAction<string, string, bool>> nextRoomListeners = new List<UnityAction<string, string, bool>>();

    // Rooms notify when they are visited
    // The event includes the room.
    static List<IRoom> roomVisitedInvokers = new List<IRoom>();
    static List<UnityAction<IRoom>> roomVisitedListeners = new List<UnityAction<IRoom>>();


    #region ItemCollected

    public static void AddItemCollectedInvoker(ItemController itemCollectedInvoker)
    {
        // add the new invoker to the list of invokers
        itemCollectedInvokers.Add(itemCollectedInvoker);

        // ensure that all existing listeners are added to this new invoker
        foreach (UnityAction<string> listener in itemCollectedListeners)
            itemCollectedInvoker.AddItemCollectedListener(listener);
    }

    public static void AddItemCollectedListener(UnityAction<string> itemCollectedListener)
    {
        // add the new listener to the list of listeners
        itemCollectedListeners.Add(itemCollectedListener);

        // ensure that this new listener is added to all existing new invokers
        foreach (ItemController invoker in itemCollectedInvokers)
            invoker.AddItemCollectedListener(itemCollectedListener);
    }

    #endregion

    #region CreatureKilled

    public static void AddCreatureKilledInvoker(ObstacleController creatureKilledInvoker)
    {
        // add the new invoker to the list of invokers
        creatureKilledInvokers.Add(creatureKilledInvoker);

        // ensure that all existing listeners are added to this new invoker
        foreach (UnityAction<string> listener in creatureKilledListeners)
            creatureKilledInvoker.AddCreatureKilledListener(listener);
    }

    public static void AddCreatureKilledListener(UnityAction<string> creatureKilledListener)
    {
        // add the new listener to the list of listeners
        creatureKilledListeners.Add(creatureKilledListener);

        // ensure that this new listener is added to all existing new invokers
        foreach (ObstacleController invoker in creatureKilledInvokers)
            invoker.AddCreatureKilledListener(creatureKilledListener);
    }

    #endregion

    #region PowerUp

    public static void AddPowerUpInvoker(ItemController powerUpInvoker)
    {
        // add the new invoker to the list of invokers
        powerUpInvokers.Add(powerUpInvoker);

        // ensure that all existing listeners are added to this new invoker
        foreach (UnityAction<string> listener in powerUpListeners)
            powerUpInvoker.AddPowerUpListener(listener);
    }

    public static void AddPowerUpListener(UnityAction<string> powerUpListener)
    {
        // add the new listener to the list of listeners
        powerUpListeners.Add(powerUpListener);

        // ensure that this new listener is added to all existing new invokers
        foreach (ItemController invoker in powerUpInvokers)
            invoker.AddPowerUpListener(powerUpListener);
    }

    #endregion

    #region PlayerDied

    public static void AddPlayerDiedInvoker(IPlayerDiedInvoker playerDiedInvoker)
    {
        // add the new invoker to the list of invokers
        playerDiedInvokers.Add(playerDiedInvoker);

        // ensure that all existing listeners are added to this new invoker
        foreach (UnityAction<Vector3> listener in playerDiedListeners)
            playerDiedInvoker.AddPlayerDiedListener(listener);
    }

    public static void AddPlayerDiedListener(UnityAction<Vector3> playerDiedListener)
    {
        // add the new listener to the list of listeners
        playerDiedListeners.Add(playerDiedListener);

        // ensure that this new listener is added to all existing new invokers
        foreach (IPlayerDiedInvoker invoker in playerDiedInvokers)
            invoker.AddPlayerDiedListener(playerDiedListener);
    }

    #endregion

    #region GameOver

    public static void AddGameOverInvoker(IGameOverInvoker gameOverInvoker)
    {
        // add the new invoker to the list of invokers
        gameOverInvokers.Add(gameOverInvoker);

        // ensure that all existing listeners are added to this new invoker
        foreach (UnityAction<int> listener in gameOverListeners)
            gameOverInvoker.AddGameOverListener(listener);
    }

    public static void AddGameOverListener(UnityAction<int> gameOverListener)
    {
        // add the new listener to the list of listeners
        gameOverListeners.Add(gameOverListener);

        // ensure that this new listener is added to all existing new invokers
        foreach (IGameOverInvoker invoker in gameOverInvokers)
            invoker.AddGameOverListener(gameOverListener);
    }

    #endregion

    #region NextRoom

    public static void AddNextRoomInvoker(Door nextRoomInvoker)
    {
        // add the new invoker to the list of invokers
        nextRoomInvokers.Add(nextRoomInvoker);

        // ensure that all existing listeners are added to this new invoker
        foreach (UnityAction<string, string, bool> listener in nextRoomListeners)
            nextRoomInvoker.AddNextRoomListener(listener);
    }

    public static void AddNextRoomListener(UnityAction<string, string, bool> nextRoomListener)
    {
        // add the new listener to the list of listeners
        nextRoomListeners.Add(nextRoomListener);

        // ensure that this new listener is added to all existing new invokers
        foreach (Door invoker in nextRoomInvokers)
            invoker.AddNextRoomListener(nextRoomListener);
    }

    #endregion

    #region RoomVisited

    public static void AddRoomVisitedInvoker(IRoom roomVisitedInvoker)
    {
        // add the new invoker to the list of invokers
        roomVisitedInvokers.Add(roomVisitedInvoker);

        // ensure that all existing listeners are added to this new invoker
        foreach (UnityAction<IRoom> listener in roomVisitedListeners)
            roomVisitedInvoker.AddRoomVisitedListener(listener);
    }

    public static void AddRoomVisitedListener(UnityAction<IRoom> roomVisitedListener)
    {
        // add the new listener to the list of listeners
        roomVisitedListeners.Add(roomVisitedListener);

        // ensure that this new listener is added to all existing new invokers
        foreach (IRoom invoker in roomVisitedInvokers)
            invoker.AddRoomVisitedListener(roomVisitedListener);
    }

    #endregion
}
