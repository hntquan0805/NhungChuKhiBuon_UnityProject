using UnityEngine;

public abstract class CardEffect : ScriptableObject
{
    public abstract void Execute(
        PlayerCharacter player,
        EnemyCharacter enemy
    );
}
