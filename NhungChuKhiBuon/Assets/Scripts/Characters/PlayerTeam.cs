using System.Collections.Generic;
using UnityEngine;

public class PlayerTeam : MonoBehaviour
{
    [HideInInspector]
    public List<PlayerCharacter> players = new List<PlayerCharacter>();

    private int totalDefense = 0;

    private void Awake()
    {
        // KHÔNG collect ở đây nữa
        players.Clear();
    }

    // 🔥 TỰ ĐỘNG gọi khi spawn / destroy child
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

    // ===== Team HP helpers =====

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
