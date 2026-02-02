using UnityEngine;

public enum CharacterClass
{
    Mage,
    Knight,
    Priest,
    Warrior,
    Assassin
}

public static class ClassAdvantage
{
    // Priest > Knight > Mage > Warrior > Assassin > Priest
    public static float GetDamageMultiplier(CharacterClass attacker, CharacterClass defender)
    {
        if (attacker == CharacterClass.Priest && defender == CharacterClass.Knight)
            return 1.3f; // +30% damage

        if (attacker == CharacterClass.Knight && defender == CharacterClass.Mage)
            return 1.3f;

        if (attacker == CharacterClass.Mage && defender == CharacterClass.Warrior)
            return 1.3f;

        if (attacker == CharacterClass.Warrior && defender == CharacterClass.Assassin)
            return 1.3f;

        if (attacker == CharacterClass.Assassin && defender == CharacterClass.Priest)
            return 1.3f;

        return 1.0f; // Normal damage
    }

    // Helper method để hiển thị tên class (optional)
    public static string GetClassName(CharacterClass characterClass)
    {
        switch (characterClass)
        {
            case CharacterClass.Mage:
                return "Mage";
            case CharacterClass.Knight:
                return "Knight";
            case CharacterClass.Priest:
                return "Priest";
            case CharacterClass.Warrior:
                return "Warrior";
            case CharacterClass.Assassin:
                return "Assassin";
            default:
                return "Unknown";
        }
    }

    // Helper method để lấy màu class (optional, dùng cho UI)
    public static Color GetClassColor(CharacterClass characterClass)
    {
        switch (characterClass)
        {
            case CharacterClass.Mage:
                return new Color(0.4f, 0.6f, 1f); // Blue
            case CharacterClass.Knight:
                return new Color(1f, 0.8f, 0.2f); // Gold
            case CharacterClass.Priest:
                return new Color(1f, 1f, 0.9f); // White/Cream
            case CharacterClass.Warrior:
                return new Color(0.8f, 0.2f, 0.2f); // Red
            case CharacterClass.Assassin:
                return new Color(0.5f, 0.2f, 0.6f); // Purple
            default:
                return Color.white;
        }
    }
}