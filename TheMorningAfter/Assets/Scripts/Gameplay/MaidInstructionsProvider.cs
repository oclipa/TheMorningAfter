using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Controls the canvas that displays instructions from the maid.
/// This is triggered by a collider that extends in front of the maid.
/// </summary>
public class MaidInstructionsProvider : MonoBehaviour, IPlayerDiedInvoker
{
    GameState gameState;
    GameObject canvas;
    GameObject player;
    GameObject maid;

    bool isWithinCollider; // is player is region that triggers instructions?
    bool hasAllItems; // does player have all collectable items?
    bool isGameOver; // is the game over?

    float maidWarningPositionX = 2.60f; // X position in room that triggers warning
    float maidKillPositionX = 2.88f; // X position in room that triggers death

    PlayerDiedEvent playerDiedEvent;

    // Start is called before the first frame update
    void Start()
    {
        this.playerDiedEvent = new PlayerDiedEvent();
        EventManager.AddPlayerDiedInvoker(this);

        EventManager.AddGameOverListener(gameOver);

        gameState = Camera.main.GetComponent<GameState>();

        maid = transform.parent.gameObject;
    }

    private void Update()
    {
        // if player is inside collider
        if (isWithinCollider && player != null)
        {
            // and player has passed the warning position
            if (player.transform.position.x > maidWarningPositionX)
            {
                // if player does not yet have all items, and has not yet been warned
                if (!hasAllItems && !gameState.MaidGivenWarning)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Oh, so you think you can charm your way past me.  You're lucky I don't throw you out!");
                    sb.AppendLine();
                    sb.AppendLine("Don't try that again or you'll regret it!");
                    sb.AppendLine();
                    sb.AppendLine("Now, be off with you - and don't come back without having clean up all that mess!");

                    canvas.GetComponentInChildren<Text>().text = sb.ToString();

                    // player is bounced backwards, as an additional warning
                    AudioManager.Instance.PlayOneShot(AudioClipName.Boing);
                    player.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 300, ForceMode2D.Force);

                    gameState.MaidGivenWarning = true;
                }
                // if player does not yet have all items and is in the kill zone
                else if (!hasAllItems && player.transform.position.x > maidKillPositionX)
                {
                    // kill the player
                    this.playerDiedEvent.Invoke(new Vector3());
                }
                // if player has all items
                else if (hasAllItems)
                {
                    // maid disappears to allow player through to bed
                    AudioManager.Instance.PlayOneShot(AudioClipName.Fanfare);
                    gameState.MaidReceivedItems = true;
                    Destroy(maid);
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

    private void gameOver(int score)
    {
        isGameOver = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        player = collision.gameObject;

        // if the game is not yet over, but the player enters the collider
        if (!isGameOver && player.CompareTag(GameConstants.PLAYER))
        {
            if (canvas == null)
                canvas = Object.Instantiate(Resources.Load("Objects/MaidSpeech")) as GameObject;

            StringBuilder sb = new StringBuilder();

            // if player does not yet all items
            if (gameState.ItemsCollected < GameConstants.TotalTreasures)
            {
                // and player has not yet been given instructions
                if (!gameState.MaidGivenInstructions)
                {
                    // display the instructions
                    AudioManager.Instance.PlayOneShot(AudioClipName.Chorus);

                    sb.AppendLine("Oh, good morning!  So, you've decided to appear then.  How's the hangover?");
                    sb.AppendLine();
                    sb.AppendLine("You want to go to bed?  Well now, isn't that a fine thing.  I suppose you expect me to clean up all of the mess while you have your beauty sleep.");
                    sb.AppendLine();
                    sb.AppendLine("No, I don't care; you only have yourself to blame.  Once you have picked up all of the items that have been littered around the house, come back here and I might let you have a rest...");
                    sb.AppendLine();
                    sb.AppendLine("You can start with that one right in front of you...");

                    gameState.MaidGivenInstructions = true;
                }
                else // warn player to get their a*** into gear
                {
                    AudioManager.Instance.PlayOneShot(AudioClipName.Alert);

                    sb.AppendLine("What do you think you are doing?  Away with you until you've collected all the items!");
                    sb.AppendLine();
                    sb.AppendLine("Don't try to get past me or I'll give you a clip round the ear!");
                }
            }
            else // has all items
            {
                // hurrah!
                AudioManager.Instance.PlayOneShot(AudioClipName.Chorus);

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
        // once player leaves the collider, hide all messages.
        player = collision.gameObject;
        if (player.CompareTag(GameConstants.PLAYER))
        {
            Destroy(canvas);

            player = null;
            canvas = null;
            isWithinCollider = false;
        }
    }

    /// <summary>
    /// Ye Olde Easter Egg
    /// </summary>
    public void Swear()
    {
        // make the maid issue a random shakesperean insult
        if (canvas == null)
            canvas = Object.Instantiate(Resources.Load("Objects/MaidSpeech")) as GameObject;

        StringBuilder sb = new StringBuilder();

        string adjective = adjectives[Random.Range(0, adjectives.Length)];
        string noun = nouns[Random.Range(0, nouns.Length)];

        sb.AppendLine("Oi!  Thou art " + adjective + " " + noun + "!  That tickles.");

        canvas.GetComponentInChildren<Text>().text = sb.ToString();

        AudioManager.Instance.PlayOneShot(AudioClipName.Oof, 1.0f);
    }

    string[] adjectives = new string[] {
                    "a fen sucked",
                    "a scurvy",
                    "a dismal",
                    "a spongy",
                    "an unwashed",
                    "a loathsome",
                    "a thin faced",
                    "an abhorrent",
                    "a drunken",
                    "a slothful",
                    "a greedy",
                    "a mad",
                    "an insolent",
                    "a muddy",
                    "a deformed",
                    "a crooked",
                    "an ill faced",
                    "a worse bodied",
                    "an ungentle",
                    "a foolish",
                    "an unkind",
                    "a stigmatical",
                    "a mad mustachio",
                    "a purple-hued",
                    "a mad headed",
                    "a pernicious",
                    "a rump-fed",
                    "a mouse-eaten",
                    "a swoll'n",
                    "an oderiferous",
                    "a sodden witted",
                    "a damnable",
                    "a cruel",
                    "a crusty",
                    "a detestable",
                    "a dreadful",
                    "a foul",
                    "a withered",
                    "a mongrel",
                    "a beef-witted",
                    "an untoward",
                    "a naughty",
                    "an ordinary",
                    "a silly",
                    "a tedious",
                    "an unnecessary",
                    "a base",
                    "an abominable",
                    "a clay brained",
                    "a knotty pated",
                    "an obscene",
                    "a greesy",
                    "a wasp-stung",
                    "a whining",
                    "a troublesome",
                    "a ruinious",
                    "a cullionly",
                    "an idle"
                    };

    string[] nouns = new string[] {
                    "puttock",
                    "toad",
                    "fustilerean",
                    "gudgeon",
                    "lout",
                    "scab",
                    "bog",
                    "ass head",
                    "knave",
                    "coxcomb",
                    "gull",
                    "boil",
                    "plague",
                    "ape",
                    "jack",
                    "milksop",
                    "swine",
                    "fox",
                    "dog",
                    "cur",
                    "whoreson",
                    "noisemaker",
                    "conger",
                    "maltworm",
                    "bloodsucker",
                    "ronyon",
                    "wench",
                    "bitch",
                    "dry cheese",
                    "carbuncle",
                    "pigeon egg",
                    "deformity",
                    "zed",
                    "boy-queller",
                    "burr",
                    "cacodemon",
                    "cat",
                    "core of envy",
                    "botch of nature",
                    "box of envy",
                    "maw",
                    "satan",
                    "dissembler",
                    "double villain",
                    "finch egg",
                    "hag",
                    "hag-seed",
                    "coward",
                    "nit",
                    "stench",
                    "commoner",
                    "rag",
                    "remnant",
                    "thief",
                    "scarlet sin",
                    "rogue",
                    "thimble",
                    "thing",
                    "thread",
                    "tomb",
                    "tortoise",
                    "villain",
                    "weed",
                    "winter-cricket",
                    "vile",
                    "fool",
                    "greasy tallow",
                    "butt",
                    "cur",
                    "barbermonger",
                    "quat"
                    };
}
