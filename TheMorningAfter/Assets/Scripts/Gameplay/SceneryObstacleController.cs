using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SceneryObstacleController : ObstacleController, IGameOverInvoker
{
    // strictly speaking we should be using a BedController
    // but ho hum.
    bool isBed;

    GameOverEvent gameOverEvent;

    public SceneryObstacleController()
        : base()
    {
        this.direction = MovementDirection.STATIC;
    }

    protected override void Start()
    {
        base.Start();

        this.gameOverEvent = new GameOverEvent();
        EventManager.AddGameOverInvoker(this);
    }

    public bool IsBed
    {
        get { return this.isBed; }
        set { this.isBed = value;  }
    }

    /// <summary>
    /// Adds the games over listener.
    /// </summary>
    /// <param name="listener">Listener.</param>
    public void AddGameOverListener(UnityAction<int> listener)
    {
        gameOverEvent.AddListener(listener);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isBed && collision.gameObject.CompareTag(GameConstants.PLAYER))
        {
            ScoreBoard scoreBoard = GameObject.FindGameObjectWithTag(GameConstants.SCOREBOARD).GetComponent<ScoreBoard>();
            scoreBoard.BedReached = true;
            int itemsCollected = scoreBoard.ItemsCollected;
            this.gameOverEvent.Invoke(itemsCollected);
        }
    }
}
