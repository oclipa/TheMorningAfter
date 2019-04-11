using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Details all rooms in the house.
/// 
/// This is not thread-safe, but for the purposes of this small game, 
/// this is good enough.
/// </summary>
public static class Blueprints {

    private static Dictionary<string, IRoom> rooms = new Dictionary<string, IRoom>();

    /// <summary>
    /// Clears all rooms from Blueprints
    /// </summary>
    public static void ClearAll()
    {
        rooms.Clear();
    }

    /// <summary>
    /// Adds a new room to Blueprints
    /// </summary>
    /// <param name="room">Room.</param>
    public static bool AddRoom(IRoom room)
    {
        // don't want to add a room twice
        if (!rooms.ContainsKey(room.GetID()))
        {
            rooms.Add(room.GetID(), room);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets the specified room from the Blueprints
    /// </summary>
    /// <returns>The room.</returns>
    /// <param name="roomID">Room identifier.</param>
    public static IRoom GetRoom(string roomID)
    {
        IRoom room = null;
        if (rooms.TryGetValue(roomID, out room))
        {
            return room;
        }

        return null;
    }

    /// <summary>
    /// Gets all room IDs
    /// </summary>
    /// <returns>The room identifiers.</returns>
    public static List<string> GetRoomIDs()
    {
        return new List<string>(rooms.Keys);
    }

    /// <summary>
    /// Resets all rooms to their previously saved state
    /// </summary>
    /// <param name="gameState">Game state.</param>
    public static void ResetRooms(GameState gameState = null)
    {
        foreach(KeyValuePair<string, IRoom> kvp in rooms)
        {
            kvp.Value.Reset(gameState);
        }
    }
}
