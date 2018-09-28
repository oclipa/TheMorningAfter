using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IPlayerDiedInvoker {

    /// <summary>
    /// Adds the player died listener.
    /// </summary>
    /// <param name="listener">Listener.</param>
    void AddPlayerDiedListener(UnityAction<Vector3> listener);
}
