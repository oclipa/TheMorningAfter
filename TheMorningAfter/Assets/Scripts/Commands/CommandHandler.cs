using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

/// <summary>
/// Abstract base class for player input command handlers
/// </summary>
public abstract class CommandHandler 
{
    protected DoNothingCommand doNothing = new DoNothingCommand();
    protected GoLeftCommand goLeft = new GoLeftCommand();
    protected GoRightCommand goRight = new GoRightCommand();
    protected GoUpCommand goUp = new GoUpCommand();
    protected GoDownCommand goDown = new GoDownCommand();
    protected JumpCommand jump = new JumpCommand();
    protected FireCommand fire = new FireCommand();

    //protected Dictionary<float, Command> commandHistory = new Dictionary<float, Command>();

    public abstract IEnumerable<Command> HandleInput(PlayerController playerController);

    /// <summary>
    /// Has a confirm command been issued?
    /// </summary>
    /// <returns><c>true</c>, if confirm is selected, <c>false</c> otherwise.</returns>
    public abstract bool IsConfirmSelected();

    /// <summary>
    /// Has an escape command been issued?
    /// </summary>
    /// <returns><c>true</c>, if escape is selected, <c>false</c> otherwise.</returns>
    public abstract bool IsEscapeSelected();
}
