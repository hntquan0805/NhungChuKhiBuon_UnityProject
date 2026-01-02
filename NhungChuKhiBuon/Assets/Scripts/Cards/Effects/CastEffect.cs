using UnityEngine;

[CreateAssetMenu(menuName = "Card Effects/Cast")]
public class CastEffect : CardEffect
{
    [Header("Cast Settings")]
    public string castName = "Special Skill";

    [Header("Debug Info")]
    public string debugMessage = "Cast effect executed!";

    public override void Execute(PlayerCharacter player, EnemyCharacter enemy)
    {
        // Trigger animation Cast
        player.PlayCast(castName);
    }
}