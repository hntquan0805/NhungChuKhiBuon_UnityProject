using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [Header("Player Resources")]
    public int PlayerCoins { get; private set; } = 10000;
    public int CurrentBet { get; set; }

    // Events
    public event System.Action<int> OnCoinsChanged;

    // Scene name constants - đảm bảo tên đúng với Build Settings
    public const string MENU_SCENE = "Menu";
    public const string BATTLE_SCENE = "BattleScene";
    public const string ARENA_SCENE = "Arena";
    public const string CASINO_SCENE = "BettingScene";
    public const string REST_AREA_SCENE = "RestArea";
    public const string TEAM_SELECTION_SCENE = "TeamSelection";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[MenuManager] Initialized");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Add coins to player
    /// </summary>
    public void AddCoins(int amount)
    {
        PlayerCoins += amount;
        OnCoinsChanged?.Invoke(PlayerCoins);
        Debug.Log($"[MenuManager] +{amount} coins. Total: {PlayerCoins}");
    }

    /// <summary>
    /// Spend coins if player has enough
    /// </summary>
    public bool SpendCoins(int amount)
    {
        if (PlayerCoins >= amount)
        {
            PlayerCoins -= amount;
            OnCoinsChanged?.Invoke(PlayerCoins);
            Debug.Log($"[MenuManager] -{amount} coins. Remaining: {PlayerCoins}");
            return true;
        }

        Debug.LogWarning($"[MenuManager] Not enough coins! Need {amount}, have {PlayerCoins}");
        return false;
    }

    /// <summary>
    /// Load a scene by name
    /// </summary>
    public void LoadScene(string sceneName)
    {
        Debug.Log($"[MenuManager] Loading scene: {sceneName}");

        // Validate scene exists in build settings
        if (!SceneExists(sceneName))
        {
            Debug.LogError($"[MenuManager] Scene '{sceneName}' not found in Build Settings!");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Return to main menu
    /// </summary>
    public void ReturnToMenu()
    {
        LoadScene(MENU_SCENE);
    }

    /// <summary>
    /// Check if scene exists in build settings
    /// </summary>
    private bool SceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (name == sceneName)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Get current scene name
    /// </summary>
    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// Check if currently in battle scene
    /// </summary>
    public bool IsInBattle()
    {
        string currentScene = GetCurrentSceneName();
        return currentScene == BATTLE_SCENE || currentScene == ARENA_SCENE;
    }

    /// <summary>
    /// Reset game data (for new game)
    /// </summary>
    public void ResetGameData()
    {
        PlayerCoins = 10000;
        CurrentBet = 0;
        OnCoinsChanged?.Invoke(PlayerCoins);

        if (PersistentTeamManager.Instance != null)
        {
            PersistentTeamManager.Instance.ClearTeamData();
        }

        Debug.Log("[MenuManager] Game data reset");
    }

    /// <summary>
    /// Debug: Log current game state
    /// </summary>
    public void LogGameState()
    {
        Debug.Log("=== GAME STATE ===");
        Debug.Log($"Coins: {PlayerCoins}");
        Debug.Log($"Current Scene: {GetCurrentSceneName()}");

        if (PersistentTeamManager.Instance != null)
        {
            PersistentTeamManager.Instance.LogTeamStatus();
        }
        else
        {
            Debug.Log("No team data available");
        }
        Debug.Log("==================");
    }

    void Update()
    {
        // ESC để về menu từ bất kỳ scene nào (trừ team selection)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            string currentScene = GetCurrentSceneName();

            if (currentScene != MENU_SCENE && currentScene != TEAM_SELECTION_SCENE)
            {
                Debug.Log("[MenuManager] ESC pressed - Returning to menu");
                ReturnToMenu();
            }
        }

        // Debug hotkey: L để log game state
        if (Input.GetKeyDown(KeyCode.L))
        {
            LogGameState();
        }
    }
}