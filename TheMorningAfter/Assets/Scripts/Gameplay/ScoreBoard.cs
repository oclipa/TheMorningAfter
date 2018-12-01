using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour, IGameOverInvoker
{
    //private Text livesRemainingText;
    private int livesRemaining;
    private GameObject[] livesSprites;

    private Text itemsCollectedText;
    private int itemsCollected;

    private bool gameOver;

    private GameOverEvent gameOverEvent;

    private bool mariaGivenInstructions;
    private bool mariaGivenWarning;
    private bool bedReached;

    private bool died;
    private Image blankScreen;
    private bool isBlank;
    Timer deathTimer;
    Vector3 rebirthPosition;

    private void Awake()
    {
        deathTimer = gameObject.AddComponent<Timer>();
        deathTimer.Duration = 0.5f;;
    }

    // Use this for initialization
    void Start()
    {
        GameObject.FindGameObjectWithTag(GameConstants.SCOREBOARD).GetComponent<Canvas>().worldCamera = Camera.main;
        GameObject statusPanel = GameObject.FindGameObjectWithTag(GameConstants.SCOREBOARDSTATUSPANEL);

        //livesRemainingText = GameObject.FindGameObjectWithTag("LivesRemainingText").GetComponent<Text>();
        livesRemaining = GameConstants.MaxLives;
        //livesRemainingText.text = createLivesRemainingText();

        livesSprites = new GameObject[GameConstants.MaxLives];
        float lifeSpriteX = -18.5f;
        for (int i = 0; i < GameConstants.MaxLives;i++)
        {
            GameObject lifeSprite = UnityEngine.Object.Instantiate(Resources.Load("Objects/LifeSprite")) as GameObject;
            lifeSprite.transform.SetParent(statusPanel.transform);

            Vector3 position = new Vector3(lifeSpriteX, -10f, 0f);
            lifeSpriteX = lifeSpriteX + 1.5f;
            lifeSprite.transform.position = position;
            lifeSprite.GetComponent<SpriteRenderer>().color = HSBColor.ToColor(new HSBColor(0.333f, 1f, 1f));

            livesSprites[i] = lifeSprite;
        }

        itemsCollectedText = GameObject.FindGameObjectWithTag(GameConstants.ITEMSCOLLECTEDTEXT).GetComponent<Text>();
        itemsCollected = 0;
        itemsCollectedText.text = createItemsCollectedText();

        blankScreen = GameObject.FindGameObjectWithTag(GameConstants.BLANKSCREENBACKGROUND).GetComponent<Image>();

        EventManager.AddItemCollectedListener(AddItem);
        EventManager.AddPlayerDiedListener(SubtractLife);

        gameOverEvent = new GameOverEvent();
        EventManager.AddGameOverInvoker(this);

        Color color = blankScreen.color;
        color.a = 0f;
        blankScreen.color = color;

    }

    private void FixedUpdate()
    {
        if (died)
        {
            if (!isBlank)
            {
                deathTimer.Run();
                Color color = blankScreen.color;
                color.a = 255f;
                blankScreen.color = color;
                isBlank = true;
            }

            if (deathTimer.Finished)
            {
                died = false;
                Color color = blankScreen.color;
                color.a = 0f;
                blankScreen.color = color;
                isBlank = false;
                PlayerSpawner.SpawnNewPlayer(rebirthPosition);
            }
        }
    }

    /// <summary>
    /// Adds the games over listener.
    /// </summary>
    /// <param name="listener">Listener.</param>
    public void AddGameOverListener(UnityAction<int> listener)
    {
        gameOverEvent.AddListener(listener);
    }

    /// <summary>
    /// Adds an item to the current score
    /// </summary>
    public void AddItem(string itemId)
    {
        //Debug.Log("Item Added: " + itemId);
        if (itemsCollectedText != null) // prevent access of destroyed Text object
        {
            itemsCollected++;
            itemsCollectedText.text = createItemsCollectedText();
        }
    }

    private string createItemsCollectedText()
    {
        return "Items Collected: " + itemsCollected + " of " + GameConstants.TotalTreasures;
    }


    private string createLivesRemainingText()
    {
        return "Lives Remaining: " + livesRemaining;
    }

    /// <summary>
    /// Reduce the number of remaining lives
    /// </summary>
    /// <param name="startPosition">The start position.</param>
    public void SubtractLife(Vector3 startPosition)
    {
        if (!gameOver)
        {
            died = true;
            rebirthPosition = startPosition;

            killPlayer();

            livesRemaining--;
            livesSprites[livesRemaining].SetActive(false);

            //if (livesRemainingText != null) // prevent access of destroyed Text object
            //{
            //    livesRemaining--;
            //    livesRemainingText.text = createLivesRemainingText();
            //}

            if (livesRemaining <= 0)
            {
                gameOverEvent.Invoke(itemsCollected);
                gameOver = true;
            }
        }
    }

    private static void killPlayer()
    {
        AudioManager.PlayOneShot(AudioClipName.Died);

        GameObject player = GameObject.FindGameObjectWithTag(GameConstants.PLAYER);

        Animator animator = player.GetComponent<Animator>();
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        animator.speed = 0;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;

        Destroy(player);
    }

    /// <summary>
    /// Gets the items collected.
    /// </summary>
    /// <value>The items collected.</value>
    public int ItemsCollected
    {
        get { return this.itemsCollected; }
    }

    /// <summary>
    /// Indicates whether Maria has given the basic instructions
    /// </summary>
    /// <value><c>true</c> if has maria given instructions; otherwise, <c>false</c>.</value>
    public bool HasMariaGivenInstructions
    {
        get { return this.mariaGivenInstructions; }
        set { this.mariaGivenInstructions = value; }
    }

    /// <summary>
    /// Indicates whether Maria has given a warning
    /// </summary>
    /// <value><c>true</c> if has maria given warning; otherwise, <c>false</c>.</value>
    public bool HasMariaGivenWarning
    {
        get { return this.mariaGivenWarning; }
        set { this.mariaGivenWarning = value; }
    }

    /// <summary>
    /// Has the bed been reached (i.e. successful game over)
    /// </summary>
    /// <value><c>true</c> if bed reached; otherwise, <c>false</c>.</value>
    public bool BedReached
    {
        get { return this.bedReached; }
        set { this.bedReached = value; }
    }
}
