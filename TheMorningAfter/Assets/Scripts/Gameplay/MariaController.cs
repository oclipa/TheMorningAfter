using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MariaController : MonoBehaviour, IPlayerDiedInvoker {

    float mariaWarningPositionX = 2.60f;
    float mariaKillPositionX = 2.88f;
    ScoreBoard scoreBoard;
    GameObject canvas;
    GameObject player;

    bool isWithinCollider;
    bool hasAllItems;
    bool isGameOver;

    PlayerDiedEvent playerDiedEvent;

    private void Start()
    {
        this.playerDiedEvent = new PlayerDiedEvent();
        EventManager.AddPlayerDiedInvoker(this);

        EventManager.AddGameOverListener(gameOver);

        scoreBoard = GameObject.FindGameObjectWithTag(GameConstants.SCOREBOARD).GetComponent<ScoreBoard>();
    }

    private void gameOver(int score)
    {
        isGameOver = true;
    }

    private void Update()
    {
        if (isWithinCollider && player != null && scoreBoard != null)
        {
            if (player.transform.position.x > mariaWarningPositionX)
            {
                if (!hasAllItems && !scoreBoard.HasMariaGivenWarning)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Oh, so you think you can charm your way past me.  You're lucky I don't throw you out!");
                    sb.AppendLine();
                    sb.AppendLine("Don't try that again or you'll regret it!");
                    sb.AppendLine();
                    sb.AppendLine("Now, be off with you - and don't come back without having clean up all that mess!");

                    canvas.GetComponentInChildren<Text>().text = sb.ToString();

                    AudioManager.PlayOneShot(AudioClipName.Boing);

                    player.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 300, ForceMode2D.Force);

                    scoreBoard.HasMariaGivenWarning = true;
                }
                else if (!hasAllItems && player.transform.position.x > mariaKillPositionX)
                {
                    this.playerDiedEvent.Invoke(new Vector3());
                }
                else if (hasAllItems)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    /// <summary>
    /// Adds the player died listener.
    /// </summary>
    /// <param name="listener">Listener.</param>
    public void AddPlayerDiedListener(UnityAction<Vector3> listener)
    {
        playerDiedEvent.AddListener(listener);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        player = collision.gameObject;
        if (!isGameOver && player.CompareTag(GameConstants.PLAYER))
        {
            canvas = Object.Instantiate(Resources.Load("Objects/MariaSpeech")) as GameObject;

            StringBuilder sb = new StringBuilder();

            if (scoreBoard.ItemsCollected < GameConstants.TotalTreasures)
            {
                if (!scoreBoard.HasMariaGivenInstructions)
                {
                    sb.AppendLine("Oh, good morning!  So, you've decided to appear then.  How's the hangover?");
                    sb.AppendLine();
                    sb.AppendLine("You want to go to bed?  Well now, isn't that a fine thing.  I suppose you expect me to clean up all of the mess while you have your beauty sleep.");
                    sb.AppendLine();
                    sb.AppendLine("No, I don't care; you only have yourself to blame.  Once you have picked up all of the items that have been littered around the house, come back here and I might let you have a rest...");

                    scoreBoard.HasMariaGivenInstructions = true;
                }
                else
                {
                    sb.AppendLine("What do you think you are doing?  Away with you until you've collected all the items!");
                    sb.AppendLine();
                    sb.AppendLine("Don't try to get past me or I'll give you a clip round the ear!");
                }
            } 
            else
            {
                sb.AppendLine("Well done, you finally did a good day's work!  Maybe there is hope for you after all.");
                sb.AppendLine();
                sb.AppendLine("OK, I'll let you through - come here and give me the rubbish.");

                hasAllItems = true;
            }

            canvas.GetComponentInChildren<Text>().text = sb.ToString();

            isWithinCollider = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        player = collision.gameObject;
        if (player.CompareTag(GameConstants.PLAYER))
        {
            Destroy(canvas);

            player = null;
            canvas = null;
            isWithinCollider = false;
        }
    }
}
