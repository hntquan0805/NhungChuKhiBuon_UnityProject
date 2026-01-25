using UnityEngine;

[CreateAssetMenu(menuName = "Card Effects/Attack + Pull Random Ally")]
public class AttackPullAllyEffect : CardEffect
{
    [Header("Damage Calculation")]
    [Range(0, 500)]
    public int damagePercent = 75; // % ATK của player (75% = 0.75x ATK)

    public override void Execute(PlayerCharacter player, EnemyCharacter enemy)
    {
        // ===== BƯỚC 1: PLAYER HIỆN TẠI TẤN CÔNG 75% ATK =====
        if (enemy == null || enemy.GetCurrentHP() <= 0)
        {
            Debug.LogWarning("AttackPullAllyEffect: No valid enemy target!");
            return;
        }

        // Tracking attacker cho boss
        BossEnemyLevel1 boss = enemy as BossEnemyLevel1;
        if (boss != null)
        {
            boss.SetLastAttacker(player);
        }

        // Tính damage 75% ATK
        int playerATK = player.GetATK();
        int calculatedDamage = Mathf.RoundToInt(playerATK * damagePercent / 100f);

        // Set target
        player.SetTarget(enemy);

        // Lưu damage tạm
        TempDamageHolder holder = player.gameObject.GetComponent<TempDamageHolder>();
        if (holder == null)
        {
            holder = player.gameObject.AddComponent<TempDamageHolder>();
        }
        holder.damage = calculatedDamage;

        // Trigger animation attack
        player.PlayAttack();

        // ===== BƯỚC 2: CHỌN RANDOM 1 PLAYER KHÁC TRONG TEAM =====
        PlayerTeam team = player.GetComponentInParent<PlayerTeam>();
        if (team != null)
        {
            // Lấy danh sách players còn sống, trừ player hiện tại
            var aliveAllies = team.players.FindAll(p => p != null && p != player && p.GetCurrentHP() > 0);

            if (aliveAllies.Count > 0)
            {
                // Chọn ngẫu nhiên 1 ally
                PlayerCharacter randomAlly = aliveAllies[Random.Range(0, aliveAllies.Count)];

                // Tìm card Base Attack từ deck của ally này
                CardData baseAttackCard = GetBaseAttackCard(randomAlly);

                if (baseAttackCard != null && CardActionQueue.Instance != null)
                {
                    CardActionQueue.Instance.EnqueueCardAction(baseAttackCard, randomAlly, null, enemy);
                    Debug.Log($"[AttackPullAllyEffect] {randomAlly.stats.characterName} được kéo dùng {baseAttackCard.cardName}!");
                }
                else
                {
                    Debug.LogWarning($"[AttackPullAllyEffect] Không tìm thấy card Base Attack trong deck của {randomAlly.stats.characterName}!");
                }
            }
            else
            {
                Debug.Log("[AttackPullAllyEffect] Không có ally nào khác còn sống để kéo!");
            }
        }
    }

    // Tìm card có tên chứa "Base Attack" từ deck của player
    private CardData GetBaseAttackCard(PlayerCharacter targetPlayer)
    {
        PlayerDeck deck = targetPlayer.GetComponent<PlayerDeck>();
        if (deck != null)
        {
            foreach (var card in deck.deckCards)
            {
                if (card != null && card.cardName.Contains("Base Attack"))
                {
                    return card;
                }
            }
        }
        return null;
    }
}
