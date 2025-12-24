using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    public int PlayerCoins { get; private set; } = 10000;
    public int CurrentBet { get; set; }

    public event System.Action<int> OnCoinsChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
        SceneManager.LoadScene(sceneName);
    }
}