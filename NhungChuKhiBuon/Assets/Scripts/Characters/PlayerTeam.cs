using System.Collections.Generic;
using UnityEngine;

public class PlayerTeam : MonoBehaviour
{
    public List<PlayerCharacter> players = new List<PlayerCharacter>();
    private int totalDefense = 0;

    private void Awake()
    {
        // Lấy tất cả PlayerCharacter con một lần duy nhất
        players.Clear();
        foreach (Transform t in transform)
        {
            PlayerCharacter pc = t.GetComponent<PlayerCharacter>();
            if (pc != null)
            {
                players.Add(pc);
            }
        }

        // Tính tổng defense của team
        CalculateTotalDefense();
    }

    private void CalculateTotalDefense()
    {
        totalDefense = 0;
        foreach (var player in players)
        {
            totalDefense += player.GetDefense();
        }
    }

    public int GetTotalDefense()
    {
        return totalDefense;
    }

    // Lấy tổng HP team
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