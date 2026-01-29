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
    public const string BATTLE_SCENE = "BossScene";
    public const string ARENA_SCENE = "Arena";
    public const string CASINO_SCENE = "BettingScene";
    public const string REST_AREA_SCENE = "RestArea";
    public const string TEAM_SELECTION_SCENE = "TeamSelection";
    public const string HOSPITAL_SCENE = "Hospital";
    public const string UPGRADE_SCENE = "ListUpgrade";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Khởi tạo MapProgressManager nếu chưa có
            if (MapProgressManager.Instance == null)
            {
                GameObject mapProgressObj = new GameObject("MapProgressManager");
                mapProgressObj.AddComponent<MapProgressManager>();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCoins(int amount)
    {
        PlayerCoins += amount;
        OnCoinsChanged?.Invoke(PlayerCoins);
    }

    public bool SpendCoins(int amount)
    {
        if (PlayerCoins >= amount)
        {
            PlayerCoins -= amount;
            OnCoinsChanged?.Invoke(PlayerCoins);
            return true;
        }

        return false;
    }

    public void LoadScene(string sceneName)
    {
        if (!SceneExists(sceneName)) return;
        
        SceneManager.LoadScene(sceneName);
    }

    // Trở về main menu
    public void ReturnToMenu()
    {
        LoadScene(MENU_SCENE);
    }

    // Check scene có trong build settings không
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

    // Lấy tên scene hiện tại
    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    // Check có đang ở battle scene không
    public bool IsInBattle()
    {
        string currentScene = GetCurrentSceneName();
        return currentScene == BATTLE_SCENE || currentScene == ARENA_SCENE;
    }

    public void ResetGameData()
    {
        PlayerCoins = 10000;
        CurrentBet = 0;
        OnCoinsChanged?.Invoke(PlayerCoins);

        if (PersistentTeamManager.Instance != null)
        {
            PersistentTeamManager.Instance.ClearTeamData();
        }
    }

    // Debug: Log game state
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            string currentScene = GetCurrentSceneName();

            if (currentScene != MENU_SCENE && currentScene != TEAM_SELECTION_SCENE)
            {
                ReturnToMenu();
            }
        }
    }
}