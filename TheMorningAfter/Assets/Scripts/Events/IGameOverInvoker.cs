using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IGameOverInvoker {

    /// <summary>
    /// Adds the games over listener.
    /// </summary>
    /// <param name="listener">Listener.</param>
    void AddGameOverListener(UnityAction<int> listener);
}
