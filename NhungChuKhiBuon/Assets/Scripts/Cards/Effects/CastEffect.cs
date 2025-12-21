using UnityEngine;

[CreateAssetMenu(menuName = "Card Effects/Cast")]
public class CastEffect : CardEffect
{
    public string castName = "Special Skill";

    public override void Execute(PlayerCharacter player, EnemyCharacter enemy)
    {
        // Trigger animation Cast
        player.PlayCast(castName);

        // Thêm logic cast effect ở đây
        Debug.Log(player.name + " cast: " + castName);
    }
}