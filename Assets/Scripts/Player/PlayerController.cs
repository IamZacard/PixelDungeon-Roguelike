using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    public PlayerMovement playerMovement;
    public PlayerAnimations playerAnimations;
    public PlayerHealth playerHealth;
    public PlayerInteraction playerInteraction;
    public Collider2D playerCollider;

    public static event Action OnPlayerTurnEnded;
    private bool isPlayerTurn = true;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>() ?? gameObject.AddComponent<PlayerMovement>();
        playerAnimations = GetComponent<PlayerAnimations>() ?? gameObject.AddComponent<PlayerAnimations>();
        playerHealth = GetComponent<PlayerHealth>() ?? gameObject.AddComponent<PlayerHealth>();
        playerInteraction = GetComponent<PlayerInteraction>() ?? gameObject.AddComponent<PlayerInteraction>();

        if (playerCollider == null)
        {
            playerCollider = GetComponent<Collider2D>();
            if (playerCollider == null) Debug.LogError("Player needs a Collider2D!");
        }

        playerMovement.Setup(this);
        playerInteraction.Setup(this);

        Debug.Log("PlayerController initialized");
    }

    void Update()
    {
        if (isPlayerTurn && !playerMovement.IsMoving)
        {
            Vector2 direction = Vector2.zero;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) direction = Vector2.up;
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) direction = Vector2.down;
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) direction = Vector2.left;
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) direction = Vector2.right;

            if (direction != Vector2.zero)
            {
                isPlayerTurn = false;
                Debug.Log($"Before Move: {direction}");
                playerMovement.Move(direction);
                Debug.Log($"After Move, before UpdateFacing");
                playerAnimations.UpdateFacing(direction);
                Debug.Log($"Player moving: {direction}");
            }
        }

        if (playerMovement.JustFinishedMoving())
        {
            Debug.Log("Move finished, handling tile effect");
            playerInteraction.HandleTileEffect();
            Debug.Log("Tile effect handled, ending turn");
            EndPlayerTurn();
        }
    }

    void EndPlayerTurn()
    {
        Debug.Log("Player turn ended");
        OnPlayerTurnEnded?.Invoke();
    }

    public void OnEnemyTurnsComplete()
    {
        isPlayerTurn = true;
        Debug.Log("Player turn resumed");
    }

    public bool IsPlayerTurn => isPlayerTurn;
}