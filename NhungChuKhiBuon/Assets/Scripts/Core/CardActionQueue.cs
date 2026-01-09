using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardActionQueue : MonoBehaviour
{
    public static CardActionQueue Instance;

    private Queue<CardAction> actionQueue = new Queue<CardAction>();
    private bool isProcessing = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EnqueueCardAction(CardData cardData, PlayerCharacter owner, CardUI cardUI, EnemyCharacter targetEnemy = null)
    {
        CardAction action = new CardAction
        {
            cardData = cardData,
            owner = owner,
            cardUI = cardUI,
            targetEnemy = targetEnemy
        };

        actionQueue.Enqueue(action);

        if (!isProcessing)
        {
            StartCoroutine(ProcessQueue());
        }
    }

    private IEnumerator ProcessQueue()
    {
        isProcessing = true;

        while (actionQueue.Count > 0)
        {
            CardAction action = actionQueue.Dequeue();

            // Giảm AP
            BattleManager.Instance.context.playerAP--;

            // Giảm CP của TẤT CẢ enemies
            List<EnemyCharacter> enemiesToInterrupt = new List<EnemyCharacter>();
            foreach (var enemy in BattleManager.Instance.enemies)
            {
                if (enemy != null && enemy.HasCPRemaining())
                {
                    enemy.ReduceCP(1);

                    if (enemy.GetCurrentCP() <= 0 && !BattleManager.Instance.HasEnemyInterrupted(enemy))
                    {
                        enemiesToInterrupt.Add(enemy);
                    }
                }
            }

            // Execute effect của card
            action.cardData.effect.Execute(action.owner, action.targetEnemy);

            // Đợi animation
            yield return new WaitForSeconds(GetAnimationDuration(action.cardData.type));

            // Đưa card vào DiscardPile
            BattleManager.Instance.handController.AddToDiscardPile(action.cardData);

            // Xóa CardUI
            if (action.cardUI != null)
            {
                Destroy(action.cardUI.gameObject);
            }

            // Xử lý tất cả enemies có CP = 0 interrupt
            foreach (var enemy in enemiesToInterrupt)
            {
                yield return StartCoroutine(ExecuteEnemyInterrupt(enemy));
                BattleManager.Instance.MarkEnemyInterrupted(enemy);
            }

            // Kiểm tra hết AP
            if (BattleManager.Instance.context.playerAP <= 0)
            {
                BattleManager.Instance.SetWaitingForSpace(true);
                break;
            }
        }

        isProcessing = false;
    }

    private IEnumerator ExecuteEnemyInterrupt(EnemyCharacter enemy)
    {
        BattleManager.Instance.state = TurnState.EnemyInterrupt;

        if (BattleManager.Instance.playerTeam == null)
        {
            yield break;
        }

        enemy.SetTarget(BattleManager.Instance.playerTeam);
        enemy.PlayAttack();

        // Đợi animation attack
        yield return new WaitForSeconds(1.0f);

        // Deal damage
        enemy.DealDamage();

        // Đợi animation hurt
        yield return new WaitForSeconds(0.5f);

        BattleManager.Instance.state = TurnState.PlayerTurn;
    }

    private float GetAnimationDuration(CardType type)
    {
        switch (type)
        {
            case CardType.Attack:
                return 1.0f;
            case CardType.Heal:
                return 1.0f;
            case CardType.Shield:
                return 1.0f;
            case CardType.Cast:
                return 1.0f;
            default:
                return 1.0f;
        }
    }

    public void ClearQueue()
    {
        actionQueue.Clear();
        isProcessing = false;
    }

    public int GetQueueCount()
    {
        return actionQueue.Count;
    }

    public bool IsProcessing()
    {
        return isProcessing;
    }
}

public class CardAction
{
    public CardData cardData;
    public PlayerCharacter owner;
    public CardUI cardUI;
    public EnemyCharacter targetEnemy;
}