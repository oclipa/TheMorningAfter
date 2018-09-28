using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Event manager.
/// </summary>
public static class EventManager
{
    // The ItemController notifies listeners when an item is collected.
    static List<ItemController> itemCollectedInvokers = new List<ItemController>();
    static List<UnityAction<string>> itemCollectedListeners = new List<UnityAction<string>>();

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
    static List<UnityAction<string, string>> nextRoomListeners = new List<UnityAction<string, string>>();


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
        foreach (UnityAction<string, string> listener in nextRoomListeners)
            nextRoomInvoker.AddNextRoomListener(listener);
    }

    public static void AddNextRoomListener(UnityAction<string, string> nextRoomListener)
    {
        // add the new listener to the list of listeners
        nextRoomListeners.Add(nextRoomListener);

        // ensure that this new listener is added to all existing new invokers
        foreach (Door invoker in nextRoomInvokers)
            invoker.AddNextRoomListener(nextRoomListener);
    }

    #endregion
}
