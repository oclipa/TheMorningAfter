using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Interface to be implemented by anything that wants to notify that the player died
/// </summary>
public interface IPlayerDiedInvoker {

    /// <summary>
    /// Adds the player died listener.
    /// </summary>
    /// <param name="listener">Listener.</param>
    void AddPlayerDiedListener(UnityAction<Vector3> listener);
}
