using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public abstract class CommandHandler 
{
    protected DoNothingCommand doNothing = new DoNothingCommand();
    protected GoLeftCommand goLeft = new GoLeftCommand();
    protected GoRightCommand goRight = new GoRightCommand();
    protected GoUpCommand goUp = new GoUpCommand();
    protected GoDownCommand goDown = new GoDownCommand();
    protected JumpCommand jump = new JumpCommand();

    //protected Dictionary<float, Command> commandHistory = new Dictionary<float, Command>();

    public abstract IEnumerable<Command> HandleInput(PlayerController playerController);
}
