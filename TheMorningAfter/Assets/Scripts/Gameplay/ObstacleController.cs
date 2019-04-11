using UnityEngine;
using System.Collections;
using UnityEngine.Events;

/// <summary>
/// Abstract base class for all creatures.
/// 
/// Strictly speaking this should be called CreatureController, although
/// the original reqirements did not explicitly require all obstacles
/// to be creatures (hence the more generic "obstacle").
/// </summary>
public abstract class ObstacleController : MonoBehaviour
{
    // An creature has a motion (including static)
    protected MovementDirection direction;

    // An creature has a Rigidbody2D for collisions
    protected Rigidbody2D rb;

    // The current state of the game
    protected GameState gameState;

    // uniquely identifies each creature
    private string creatureID;

    // used to notify that the creature has been killed
    CreatureKilledEvent creatureKilledEvent;


    // Use this for initialization
    protected virtual void Start()
    {
        gameState = Camera.main.GetComponent<GameState>();
        rb = this.GetComponent<Rigidbody2D>();

        this.creatureKilledEvent = new CreatureKilledEvent();
        EventManager.AddCreatureKilledInvoker(this);
    }

    protected virtual void Update()
    {
    }

    /// <summary>
    /// Creature is exploded by replacing the creature game object with 
    /// an explosion game object.
    /// </summary>
    public void Explode()
    {
        GameObject explosion = Object.Instantiate(Resources.Load("Objects/Explosion")) as GameObject;
        explosion.transform.position = transform.position;

        Destroy(this.gameObject);

        this.creatureKilledEvent.Invoke(this.creatureID);
    }

    /// <summary>
    /// Unique identifier for this creature
    /// </summary>
    /// <value>The identifier.</value>
    public string ID
    {
        get { return this.creatureID; }
        set { this.creatureID = value; }
    }

    /// <summary>
    /// Adds the creature killed listener.
    /// </summary>
    /// <param name="listener">Listener.</param>
    public void AddCreatureKilledListener(UnityAction<string> listener)
    {
        creatureKilledEvent.AddListener(listener);
    }
}