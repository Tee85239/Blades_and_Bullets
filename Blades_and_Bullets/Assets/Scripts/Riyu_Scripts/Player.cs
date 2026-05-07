using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Unity.VisualScripting;
using Game.Collectibles.Player;
using System.Collections;

public class Player : MonoBehaviour
{
    
    public static Player Instance{get; private set;}
    public enum MoveState
    {
        Normal,
        Focused,
        Death
    }
    public MoveState moveState;
    [SerializeField] private float speed;
    [SerializeField] private GameObject bombPrefab;
    private float bombCooldown;
    private float deathTimer;
    private PlayerResourceInventory inventory;
    private const string IS_KILLED = "isKilled";
    public Animator animator;

    [SerializeField] private AudioClip playerHitClip;
    [SerializeField] private AudioClip playerDeathClip;
    private bool hasPlayedDeathSound; // prevents repeated game over sound playback

    //Events

    //Firing bullets Logic;    
    public static EventHandler PlayerFiresBullet;

    //Player gets hit logic
      public static EventHandler<OnPlayerGetsHitArgs> OnPlayerGetsHit;
      public class OnPlayerGetsHitArgs : EventArgs
     {
         public GameObject TargetHit;
     }

    // UI events
  
    //Special slash variables
    public static EventHandler<ModifyAbilityCooldownArgs> ModifyAbilityCooldown;
    public class ModifyAbilityCooldownArgs : EventArgs
    {
        public float changeAmount;
    }

    private void Awake()
    {
        Instance = this;
        moveState = MoveState.Normal;
        inventory = GetComponent<PlayerResourceInventory>();
    }
    private void Start()
    {
        SlashScript.OnSlashingSomething += OnSlashingSomething;
        GameControllerScript.OnPlayerDeath += OnPlayerDeath;
        animator = gameObject.GetComponent<Animator>();

    }

    private void OnPlayerDeath(object sender, EventArgs e)
    {
        Destroy(gameObject);
    }


    private void OnSlashingSomething(object sender, SlashScript.OnSlashingSomethingArgs e)
    {
        if (sender is SlashScript slashScript)
        {
            if (slashScript.GetBulletType() == SlashScript.BulletType.Normal || slashScript.GetBulletType() == SlashScript.BulletType.Bomb)
            {
                ModifyAbilityCooldown?.Invoke(this, new ModifyAbilityCooldownArgs
                {
                    changeAmount = 0.03f
                });
            } else if (slashScript.GetBulletType() == SlashScript.BulletType.Special){
        
                ModifyAbilityCooldown?.Invoke(this, new ModifyAbilityCooldownArgs
                {
                    changeAmount = 0f
                });
            }
        }
    }

    private void Update()
    {
        bombCooldown -= Time.deltaTime;
        deathTimer -= Time.deltaTime;

        if (deathTimer < 0f)
        {
            moveState = MoveState.Normal;
        }

        if (moveState != MoveState.Death) // not dead
        {
            HandleMovement();
            HandleInteraction();

        }
        else if (inventory.Lives <= 0f) // checks if player has no lives left
        {
            if (!hasPlayedDeathSound) // prevents repeated game-over handling
            {
                hasPlayedDeathSound = true; // prevents this from running repeatedly
                StartCoroutine(GameOverRoutine()); // starts game-over handling
            }
        }

    }

    private void HandleInteraction()
    {        
        if(Keyboard.current.xKey.wasPressedThisFrame || Keyboard.current.periodKey.wasPressedThisFrame)
        {   
            if (inventory.Bombs > 0 && bombCooldown <= 0)
            {
                
                Instantiate(bombPrefab, transform.position, Quaternion.Euler(0f, 0f, 0f));
                bombCooldown = 6f;
                inventory.SubtractBomb();
            } 
        }
    }

    private void FireBullets()
    {
        PlayerFiresBullet?.Invoke(this, EventArgs.Empty);

    }
    private void HandleMovement()
    {
        Vector2 moveVector = new Vector2(0f, 0f);
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
        {
            moveVector.y = 1f;
        } 
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
        {
            moveVector.y = -1f;
        } 
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            moveVector.x = -1f;
        } 
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            moveVector.x = 1f;
        } 

        Vector3 endMoveVector = new Vector3(moveVector.x, moveVector.y, 0f).normalized;
        if (Keyboard.current.shiftKey.isPressed)
        {
            endMoveVector.x *= .4f;
            endMoveVector.y *= .4f;
            moveState = MoveState.Focused;
        } else
        {
            moveState = MoveState.Normal;
        }
        transform.position += endMoveVector * speed * Time.deltaTime;
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -8.75f, 2.75f), Mathf.Clamp(transform.position.y, -4.8f, 4.8f), .8f);

        if (transform.position.y > 20)
        {
            // Shoot event for Quick Collect
        }
    }
    public void Death() // runs when the player is hit
    {
        deathTimer = 1f; // starts temporary death/invulnerability timer
        bombCooldown = 8f; // resets bomb cooldown after hit

        inventory.SubtractLife(); // removes one life first so we can check final life state

        if (inventory.Lives <= 0f) // checks if this hit killed the player permanently
        {
            GameplayAudioManager.Instance.MuteAllOtherAudioSources(0f); // mutes all other scene audio
            GameplayAudioManager.Instance.PlaySFX(playerDeathClip, 2f); // plays final death sound

            hasPlayedDeathSound = true; // prevents duplicate final death handling
            StartCoroutine(GameOverRoutine()); // delays pause/game-over handling

            return; // prevents normal hit sound and respawn logic
        }

        GameplayAudioManager.Instance.PlaySFX(playerHitClip, 1.5f); // plays normal hit sound

        OnPlayerGetsHit?.Invoke(this, new OnPlayerGetsHitArgs()); // broadcasts hit event
        StartCoroutine(RespawnPoint()); // starts normal respawn sequence
    }

    IEnumerator RespawnPoint()
    {
        yield return new WaitForSeconds(.8f);  // Pause for 2 seconds
        transform.position = new Vector3(-3f, -4f, transform.position.z);
        Instantiate(bombPrefab, transform.position, Quaternion.Euler(0f, 0f, 0f));
    }

    private void OnDestroy()
    {
        SlashScript.OnSlashingSomething -= OnSlashingSomething;
        GameControllerScript.OnPlayerDeath -= OnPlayerDeath;
    }

    private IEnumerator GameOverRoutine() // waits briefly before pausing the game
    {
        yield return new WaitForSecondsRealtime(1f); // gives death sound time to play
        Time.timeScale = 0f; // pauses the game after the sound starts
    }

}
