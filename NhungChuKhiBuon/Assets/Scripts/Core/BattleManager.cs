using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    [Header("Characters")]
    public PlayerTeam playerTeam;
    public List<EnemyCharacter> enemies = new List<EnemyCharacter>();

    [Header("Turn Config")]
    public int playerMaxAP = 4;
    [Range(3, 5)]
    public int enemyCPMin = 3;
    [Range(3, 5)]
    public int enemyCPMax = 5;

    [Header("Hand Controller")]
    public HandController handController;
    
    [Header("UI")]
    public FinishTurnButton finishTurnButton;

    [Header("Victory Settings")]
    public string victorySceneName = "VictoryScene";
    public float delayBeforeVictory = 1.5f;

    public BattleContext context;
    public TurnState state;

    private bool waitingForSpaceBar = false;
    private List<EnemyCharacter> enemiesInterrupted = new List<EnemyCharacter>();
    private bool isVictory = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitializeEnemiesCP();

        StartPlayerTurn();

        if (playerTeam != null)
        {
            playerTeam.ClearShield();
        }

        if (handController != null)
        {
            handController.DrawNewHand();
        }
    }


    private void Update()
    {
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

    public void FinishTurn()
    {
        ResetTurn();
    }

    void ResetTurn()
    {
        if (playerTeam == null) return;

        playerTeam.ClearShield();

        // ========== XỬ LÝ DEBUFF TRƯỚC KHI ENEMY ATTACK ==========
        foreach (var enemy in enemies.ToList())
        {
            if (enemy != null && enemy.GetCurrentHP() > 0)
            {
                enemy.ProcessDebuffsAtTurnStart();
            }
        }

        // ========== XỦ LÝ BUFF CỦA PLAYER TEAM ==========
        if (playerTeam != null)
        {
            foreach (var player in playerTeam.players)
            {
                if (player != null && player.GetCurrentHP() > 0)
                {
                    player.ProcessBuffsAtTurnStart();
                }
            }
        }

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
            enemy.SetTarget(playerTeam);
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
        
        // Reset button sprite khi bắt đầu turn mới
        if (finishTurnButton != null)
        {
            finishTurnButton.ResetButton();
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
    
    public EnemyCharacter GetFirstAliveEnemyWithoutStealth()
    {
        if (enemies == null || enemies.Count == 0)
            return null;

        foreach (var enemy in enemies)
        {
            if (enemy != null && enemy.GetCurrentHP() > 0 && !enemy.HasStealth())
            {
                return enemy;
            }
        }

        return null;
    }

    void CheckVictoryCondition()
    {
        if (isVictory) return;

        bool hasAliveEnemy = false;
        foreach (var enemy in enemies)
        {
            if (enemy != null && enemy.GetCurrentHP() > 0 && !enemy.IsDead())
            {
                hasAliveEnemy = true;
                break;
            }
        }

        if (!hasAliveEnemy && enemies.Count == 0)
        {
            TriggerVictory();
        }
    }

    void TriggerVictory()
    {
        isVictory = true;
        state = TurnState.BattleEnd;

        Invoke("LoadVictoryScene", delayBeforeVictory);
    }

    void LoadVictoryScene()
    {
        if (PersistentTeamManager.Instance != null && playerTeam != null)
        {
            PersistentTeamManager.Instance.SaveTeamState(playerTeam.players);
        }

        SceneManager.LoadScene(victorySceneName);
    }

    public bool IsVictory()
    {
        return isVictory;
    }
}