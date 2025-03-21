using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class PlayerInteraction : MonoBehaviour
{
    private LevelSettings levelSettings; // Reference to the settings asset
    private LevelGenerator levelGenerator;
    private Tilemap encountersTilemap;
    private PlayerController controller;
    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;
    private PlayerAnimations playerAnimations;

    [SerializeField] private int wealth = 0;

    public void Setup(PlayerController ctrl)
    {
        controller = ctrl;
        playerHealth = ctrl.GetComponent<PlayerHealth>();
        playerMovement = ctrl.GetComponent<PlayerMovement>();
        playerAnimations = ctrl.GetComponent<PlayerAnimations>();

        // Find LevelGenerator to access its settings
        LevelGenerator levelGenerator = FindObjectOfType<LevelGenerator>();
        if (levelGenerator != null)
        {
            // Access LevelSettings through reflection or a public method (we'll add a getter to LevelGenerator)
            levelSettings = levelGenerator.GetLevelSettings();
        }

        // Find TilemapManager to get the encountersTilemap
        TilemapManager tilemapManager = FindObjectOfType<TilemapManager>();
        if (tilemapManager != null)
        {
            encountersTilemap = tilemapManager.GetEncountersTilemap();
        }

        Debug.Log($"PlayerInteraction setup: LevelSettings {(levelSettings != null ? "found" : "null")}, Tilemap {(encountersTilemap != null ? "found" : "null")}");

        if (levelSettings == null || encountersTilemap == null)
        {
            Debug.LogError("Could not automatically find LevelSettings or encountersTilemap!");
        }
    }

    public void HandleTileEffect()
    {
        if (encountersTilemap == null || !encountersTilemap.gameObject.activeInHierarchy) return;

        Vector3Int tilePos = encountersTilemap.WorldToCell(transform.position);
        TileBase currentTile = encountersTilemap.GetTile(tilePos);
        if (currentTile == null) return;

        if (levelSettings == null)
        {
            Debug.LogError("LevelSettings is null in HandleTileEffect!");
            return;
        }

        if (currentTile == levelSettings.escapeTile)
        {
            OnEscapeTile(tilePos);
        }
        else if (currentTile == levelSettings.trapTile)
        {
            OnTrapTile(tilePos);
        }
        else if (currentTile == levelSettings.enemyTile)
        {
            OnEnemyTile(tilePos);
        }
        else if (currentTile == levelSettings.coinTile)
        {
            OnCoinTile(tilePos);
        }
        else if (currentTile == levelSettings.potionTile)
        {
            OnPotionTile(tilePos);
        }
    }

    void OnEscapeTile(Vector3Int tilePos)
    {
        Debug.Log("Player reached the escape tile! Level complete!");
    }

    void OnTrapTile(Vector3Int tilePos)
    {
        Debug.Log("Player stepped on a trap!");
        playerHealth.TakeDamage(10);

        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        Vector2 jumpDirection = directions[Random.Range(0, directions.Length)];
        Vector2 newPos = (Vector2)transform.position + jumpDirection;

        if (playerMovement.IsTileWalkable(newPos))
        {
            playerMovement.Move(jumpDirection);
            playerAnimations.UpdateFacing(jumpDirection);
        }
        else
        {
            Debug.Log("Trap jump blocked by wall! Moving to opposite side.");
            Vector2 oppositeDirection = -jumpDirection;
            Vector2 oppositePos = (Vector2)transform.position + oppositeDirection;

            if (playerMovement.IsTileWalkable(oppositePos))
            {
                transform.position = oppositePos;
            }
            else
            {
                Debug.Log("Opposite direction also blocked! Staying put.");
            }
        }
    }

    void OnEnemyTile(Vector3Int tilePos)
    {
        Debug.Log("Player encountered an enemy!");
        playerHealth.TakeDamage(5);
        encountersTilemap.SetTile(tilePos, null);
    }

    void OnCoinTile(Vector3Int tilePos)
    {
        Debug.Log("Player picked up a coin!");
        GetWealthy(50);
        encountersTilemap.SetTile(tilePos, null);
        Debug.Log($"Post-coin state: isPlayerTurn={controller.IsPlayerTurn}, IsMoving={playerMovement.IsMoving}");
    }

    void OnPotionTile(Vector3Int tilePos)
    {
        Debug.Log("Player picked up a potion!");
        playerHealth.GainHealth(30);
        encountersTilemap.SetTile(tilePos, null);
    }

    private void GetWealthy(int amount)
    {
        wealth += amount;
        Debug.Log($"Player got {amount} money! Wealth: {wealth}");
    }

    public int Wealth => wealth;

    public Vector3Int GetTilePosition()
    {
        return encountersTilemap != null ?
            encountersTilemap.WorldToCell(transform.position) : Vector3Int.zero;
    }

    public Tilemap GetEncountersTilemap()
    {
        return encountersTilemap;
    }
}