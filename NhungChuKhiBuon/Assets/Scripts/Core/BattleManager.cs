using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement; // Thêm dòng này

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    [Header("Characters")]
    public PlayerCharacter player;
    public List<EnemyCharacter> enemies = new List<EnemyCharacter>();

    [Header("Turn Config")]
    public int playerMaxAP = 4;
    [Range(3, 5)]
    public int enemyCPMin = 3;
    [Range(3, 5)]
    public int enemyCPMax = 5;

    [Header("Hand Controller")]
    public HandController handController;

    [Header("Victory Settings")]
    public string victorySceneName = "VictoryScene"; // Tên scene Victory
    public float delayBeforeVictory = 1.5f; // Delay trước khi chuyển scene

    public BattleContext context;
    public TurnState state;

    private bool waitingForSpaceBar = false;
    private List<EnemyCharacter> enemiesInterrupted = new List<EnemyCharacter>();
    private bool isVictory = false; // Flag để tránh trigger nhiều lần

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitializeEnemiesCP();
        StartPlayerTurn();
    }

    private void Update()
    {
        if (waitingForSpaceBar && Input.GetKeyDown(KeyCode.Space))
        {
            ResetTurn();
        }

        // Kiểm tra victory condition
        CheckVictoryCondition();
    }

    void InitializeEnemiesCP()
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.InitializeCP(enemyCPMin, enemyCPMax);
            }
        }
    }

    void StartPlayerTurn()
    {
        context = new BattleContext
        {
            playerAP = playerMaxAP
        };

        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.ResetCP();
            }
        }

        state = TurnState.PlayerTurn;
        waitingForSpaceBar = false;
        enemiesInterrupted.Clear();
    }

    public bool CanPlayCard()
    {
        return state == TurnState.PlayerTurn && context.playerAP > 0 && !waitingForSpaceBar;
    }

    void ResetTurn()
    {
        List<EnemyCharacter> enemiesToInterrupt = new List<EnemyCharacter>();
        foreach (var enemy in enemies)
        {
            if (enemy != null && enemy.HasCPRemaining() && !enemiesInterrupted.Contains(enemy))
            {
                enemiesToInterrupt.Add(enemy);
            }
        }

        foreach (var enemy in enemiesToInterrupt)
        {
            enemy.SetTarget(player);
            enemy.PlayAttack();
            enemy.DealDamage();
            enemiesInterrupted.Add(enemy);
        }

        if (CardActionQueue.Instance != null)
        {
            CardActionQueue.Instance.ClearQueue();
        }

        if (handController != null)
        {
            handController.DiscardCurrentHand();
        }

        StartPlayerTurn();

        if (handController != null)
        {
            handController.DrawNewHand();
        }
    }

    public bool IsWaitingForSpace()
    {
        return waitingForSpaceBar;
    }

    public void SetWaitingForSpace(bool waiting)
    {
        waitingForSpaceBar = waiting;
    }

    public bool HasEnemyInterrupted(EnemyCharacter enemy)
    {
        return enemiesInterrupted.Contains(enemy);
    }

    public void MarkEnemyInterrupted(EnemyCharacter enemy)
    {
        if (!enemiesInterrupted.Contains(enemy))
        {
            enemiesInterrupted.Add(enemy);
        }
    }

    public EnemyCharacter GetFirstAliveEnemy()
    {
        if (enemies == null || enemies.Count == 0)
            return null;

        foreach (var enemy in enemies)
        {
            if (enemy != null && enemy.GetCurrentHP() > 0)
            {
                return enemy;
            }
        }

        return null;
    }

    // PHƯƠNG THỨC MỚI: Kiểm tra điều kiện chiến thắng
    void CheckVictoryCondition()
    {
        // Tránh trigger nhiều lần
        if (isVictory) return;

        // Kiểm tra có còn enemy nào còn sống không
        bool hasAliveEnemy = false;
        foreach (var enemy in enemies)
        {
            if (enemy != null && enemy.GetCurrentHP() > 0 && !enemy.IsDead())
            {
                hasAliveEnemy = true;
                break;
            }
        }

        // Nếu không còn enemy nào → Victory!
        if (!hasAliveEnemy && enemies.Count == 0)
        {
            TriggerVictory();
        }
    }

    void TriggerVictory()
    {
        isVictory = true;
        state = TurnState.BattleEnd;

        Debug.Log("🎉 VICTORY! All enemies defeated!");

        // Chuyển sang scene Victory sau delay
        Invoke("LoadVictoryScene", delayBeforeVictory);
    }

    void LoadVictoryScene()
    {
        SceneManager.LoadScene(victorySceneName);
    }

    // PUBLIC METHOD: Có thể gọi từ nơi khác nếu cần
    public bool IsVictory()
    {
        return isVictory;
    }
}