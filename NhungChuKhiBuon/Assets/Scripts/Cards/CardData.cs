using UnityEngine;

public enum CardType { Attack, Heal, Shield, Cast }

[CreateAssetMenu(fileName = "NewCard", menuName = "Card Game/CardData")]
public class CardData : ScriptableObject
{
    public string cardName;
    public CardType type;
    public Sprite artwork;
    public CardEffect effect;
}
