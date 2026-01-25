using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Card Effects/Buff + Pull Strongest Ally")]
public class BuffPullStrongestAllyEffect : CardEffect
{
    [Header("Cast Settings")]
    public string castName = "War Cry";

    [Header("Buff Settings")]
    public int buffStacks = 1; // Số stack buff (1 turn)
    public Sprite buffIcon; // Icon của buff

    public override void Execute(PlayerCharacter player, EnemyCharacter enemy)
    {
        // ===== BƯỚC 1: CAST BUFF INCREASE ATTACK CHO PLAYER HIỆN TẠI =====
        if (player != null && player.GetCurrentHP() > 0)
        {
            player.AddBuff(BuffType.IncreaseAttack, buffStacks, buffIcon);
        }

        // Trigger animation Cast
        player.PlayCast(castName);

        // ===== BƯỚC 2: TÌM PLAYER CÓ ATK CAO NHẤT TRONG TEAM =====
        PlayerTeam team = player.GetComponentInParent<PlayerTeam>();
        if (team != null)
        {
            // Lấy danh sách players còn sống
            var alivePlayers = team.players.Where(p => p != null && p.GetCurrentHP() > 0).ToList();

            if (alivePlayers.Count > 0)
            {
                // Tìm player có ATK cao nhất
                PlayerCharacter strongestAlly = alivePlayers.OrderByDescending(p => p.GetATK()).First();

                // Tìm card Base Attack từ deck của ally này
                CardData baseAttackCard = GetBaseAttackCard(strongestAlly);

                // Enqueue card BaseAttack cho ally này
                if (CardActionQueue.Instance != null && enemy != null && baseAttackCard != null)
                {
                    CardActionQueue.Instance.EnqueueCardAction(baseAttackCard, strongestAlly, null, enemy);
                    Debug.Log($"[BuffPullStrongestAllyEffect] {strongestAlly.stats.characterName} (ATK: {strongestAlly.GetATK()}) được kéo dùng {baseAttackCard.cardName}!");
                }
                else if (enemy == null)
                {
                    Debug.LogWarning("[BuffPullStrongestAllyEffect] Không có enemy target để tấn công!");
                }
                else if (baseAttackCard == null)
                {
                    Debug.LogWarning($"[BuffPullStrongestAllyEffect] Không tìm thấy card Base Attack trong deck của {strongestAlly.stats.characterName}!");
                }
            }
            else
            {
                Debug.Log("[BuffPullStrongestAllyEffect] Không có player nào còn sống!");
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
