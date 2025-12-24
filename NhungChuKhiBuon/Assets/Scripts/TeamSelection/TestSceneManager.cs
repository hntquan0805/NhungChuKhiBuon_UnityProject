using UnityEngine;
using System.Collections.Generic;

public class TestTeamReceiver : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("╔══════════════════════════════════════╗");
        Debug.Log("║     ĐÃ CHUYỂN SANG SCENE MỚI!       ║");
        Debug.Log("╚══════════════════════════════════════╝");
        Debug.Log("");

        // Kiểm tra xem có TeamDataManager không
        if (TeamDataManager.Instance == null)
        {
            Debug.LogError("❌ KHÔNG TÌM THẤY TeamDataManager!");
            Debug.LogError("   → Có thể bạn không chạy từ Team Selection scene");
            return;
        }

        // Lấy team data
        List<TeamDataManager.SelectedHeroData> team = TeamDataManager.Instance.GetSelectedTeam();

        if (team == null || team.Count == 0)
        {
            Debug.LogError("❌ KHÔNG CÓ DỮ LIỆU TEAM!");
            return;
        }

        // Log ra thông tin team
        Debug.Log($"✓ ĐÃ NHẬN ĐƯỢC TEAM VỚI {team.Count} HEROES:");
        Debug.Log("════════════════════════════════════════");

        for (int i = 0; i < team.Count; i++)
        {
            TeamDataManager.SelectedHeroData hero = team[i];

            Debug.Log($"[{i + 1}] {hero.heroName}");
            Debug.Log($"    • Avatar Sprite: {(hero.avatarSprite != null ? "✓" : "✗")}");
            Debug.Log($"    • FullBody Sprite: {(hero.fullBodySprite != null ? "✓" : "✗")}");
            Debug.Log($"    • Prefab: {(hero.heroPrefab != null ? hero.heroPrefab.name : "⚠ CHƯA GÁN")}");
            Debug.Log("");
        }

        Debug.Log("════════════════════════════════════════");
        Debug.Log("✓ CHUYỂN SCENE THÀNH CÔNG!");
    }
}

// HƯỚNG DẪN:
// 1. Tạo scene mới (ví dụ: "TestScene")
// 2. Tạo empty GameObject trong scene đó
// 3. Attach script này vào GameObject đó
// 4. Add scene vào Build Settings (File → Build Settings → Add Open Scenes)
// 5. Trong TeamSelectionManager, set nextSceneName = "TestScene"
// 6. Chạy Team Selection scene, chọn 3 heroes, nhấn Enter
// 7. Xem Console để thấy log!