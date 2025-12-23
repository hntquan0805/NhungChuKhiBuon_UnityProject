using UnityEngine;

public enum CharacterClass
{
    Mage,
    Knight,
    Priest
}

public static class ClassAdvantage
{
    // Mage > Knight > Priest > Mage
    public static float GetDamageMultiplier(CharacterClass attacker, CharacterClass defender)
    {
        if (attacker == CharacterClass.Mage && defender == CharacterClass.Knight)
            return 1.3f; // +30% damage

        if (attacker == CharacterClass.Knight && defender == CharacterClass.Priest)
            return 1.3f;

        if (attacker == CharacterClass.Priest && defender == CharacterClass.Mage)
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
            default:
                return Color.white;
        }
    }
}