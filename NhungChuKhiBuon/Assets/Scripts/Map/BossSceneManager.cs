using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BossSceneManager : MonoBehaviour
{
    [Header("Setup")]
    public Transform spawnPoint; // Kéo vị trí đứng của Boss vào đây
    public Text bossNameText;    // Kéo UI Text tên boss vào đây

    void Start()
    {
        // Lấy dữ liệu từ BattleSetup
        BossData data = BattleSetup.currentBossData;

        if (data != null)
        {
            // 1. Sinh Boss
            if (data.bossPrefab != null)
            {
                GameObject boss = Instantiate(data.bossPrefab, spawnPoint.position, Quaternion.identity);

                // Ở đây bạn có thể gọi hàm Setup chỉ số cho boss
                // Ví dụ: boss.GetComponent<EnemyController>().SetStats(data.maxHP, data.damage);
            }

            // 2. Hiển thị tên
            if (bossNameText != null) bossNameText.text = data.bossName;
        }
        else
        {
            Debug.LogError("LỖI: Vào Scene Boss mà không có dữ liệu! Hãy chạy từ Map.");
        }
    }

    // GỌI HÀM NÀY KHI CHIẾN THẮNG
    public void OnVictory()
    {
        string nextScene = "MainMenu"; // Mặc định
        if (BattleSetup.currentBossData != null)
        {
            nextScene = BattleSetup.currentBossData.nextMapSceneName;
        }

        Debug.Log("Chiến thắng! Chuyển sang: " + nextScene);
        SceneManager.LoadScene(nextScene);
    }
}