using UnityEngine;
using UnityEngine.SceneManagement; // Needed to restart the game

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("King Towers")]
    public TowerHealth enemyKingTower;
    public TowerHealth playerKingTower;

    private bool gameHasEnded = false;

    void Awake()
    {
        // Setup singleton instance so other scripts can find it easily
        if (Instance == null) 
        {
            Instance = this;
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (gameHasEnded) return;

        // 1. Check if Enemy King Tower is destroyed -> PLAYER WINS
        if (enemyKingTower != null && enemyKingTower.isDestroyed)
        {
            WinGame();
        }

        // 2. Check if Player King Tower is destroyed -> PLAYER LOSES
        if (playerKingTower != null && playerKingTower.isDestroyed)
        {
            LoseGame();
        }
    }

    void WinGame()
    {
        gameHasEnded = true;
        Debug.Log("VICTORY! Enemy King Tower has fallen!");
        
        // Match ends, restarts after 3 seconds
        Invoke("RestartMatch", 3f);
    }

    void LoseGame()
    {
        gameHasEnded = true;
        Debug.Log("DEFEAT! Your King Tower was destroyed!");
        
        // Match ends, restarts after 3 seconds
        Invoke("RestartMatch", 3f);
    }

    void RestartMatch()
    {
        // Reloads the active scene to reset the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}