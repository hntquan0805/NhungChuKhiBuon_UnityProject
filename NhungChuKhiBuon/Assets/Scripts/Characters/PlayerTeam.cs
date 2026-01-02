using System.Collections.Generic;
using UnityEngine;

public class PlayerTeam : MonoBehaviour
{
    [HideInInspector]
    public List<PlayerCharacter> players = new List<PlayerCharacter>();

    private int totalDefense = 0;

    [Header("Team Shield")]
    private int teamShield = 0;

    private void Awake()
    {
        players.Clear();
        teamShield = 0; // Khởi tạo shield = 0
    }

    private void OnTransformChildrenChanged()
    {
        RefreshPlayers();
    }

    public void RefreshPlayers()
    {
        players.Clear();
        players.AddRange(GetComponentsInChildren<PlayerCharacter>(true));

        CalculateTotalDefense();

        Debug.Log($"[PlayerTeam] Refreshed players: {players.Count}");
    }

    private void CalculateTotalDefense()
    {
        totalDefense = 0;
        foreach (var player in players)
        {
            if (player != null)
                totalDefense += player.GetDefense();
        }
    }

    public int GetTotalDefense()
    {
        return totalDefense;
    }

    public int GetTeamShield()
    {
        return teamShield;
    }

    public void AddShield(int amount)
    {
        teamShield += amount;
        Debug.Log($"[Team Shield] Added {amount}. Total shield: {teamShield}");
    }

    public void ReduceShield(int amount)
    {
        teamShield -= amount;
        teamShield = Mathf.Max(teamShield, 0);
        Debug.Log($"[Team Shield] Reduced by {amount}. Remaining: {teamShield}");
    }

    public void ClearShield()
    {
        teamShield = 0;
        Debug.Log("[Team Shield] Cleared");
    }

    public int GetTotalCurrentHP()
    {
        int total = 0;
        foreach (var p in players)
            total += p.GetCurrentHP();
        return total;
    }

    public int GetTotalMaxHP()
    {
        int total = 0;
        foreach (var p in players)
            total += p.GetMaxHP();
        return total;
    }
}