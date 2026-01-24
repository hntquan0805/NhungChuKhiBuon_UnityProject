using UnityEngine;

public class PlayerResourceManager : MonoBehaviour
{
    private static PlayerResourceManager instance;
    public static PlayerResourceManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("PlayerResourceManager");
                instance = go.AddComponent<PlayerResourceManager>();
                DontDestroyOnLoad(go);
                instance.LoadResources();
            }
            return instance;
        }
    }

    [Header("Player Resources")]
    [SerializeField] private int currentExp = 999999;
    [SerializeField] private int currentGold = 999999;

    private const string EXP_KEY = "PlayerExp";
    private const string GOLD_KEY = "PlayerGold";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadResources();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public int CurrentExp => currentExp;
    public int CurrentGold => currentGold;

    public void AddExp(int amount)
    {
        currentExp += amount;
        SaveResources();
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        SaveResources();
    }

    public bool SpendExp(int amount)
    {
        if (currentExp >= amount)
        {
            currentExp -= amount;
            SaveResources();
            return true;
        }
        return false;
    }

    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            SaveResources();
            return true;
        }
        return false;
    }

    public bool CanAffordUpgrade(int expCost, int goldCost)
    {
        return currentExp >= expCost && currentGold >= goldCost;
    }

    private void SaveResources()
    {
        PlayerPrefs.SetInt(EXP_KEY, currentExp);
        PlayerPrefs.SetInt(GOLD_KEY, currentGold);
        PlayerPrefs.Save();
    }

    private void LoadResources()
    {
        currentExp = PlayerPrefs.GetInt(EXP_KEY, 999999);
        currentGold = PlayerPrefs.GetInt(GOLD_KEY, 999999);
    }

    public void ResetResources()
    {
        currentExp = 999999;
        currentGold = 999999;
        SaveResources();
    }
}
