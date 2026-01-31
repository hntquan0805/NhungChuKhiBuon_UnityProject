using UnityEngine;

/// <summary>
/// Script này nên được gắn vào MapLv1, MapLv2, MapLv3 scenes
/// Khởi tạo coin khi bắt đầu map
/// </summary>
public class MapInitializer : MonoBehaviour
{
    [Header("Settings")]
    public int mapLevel = 1; // Gán trong Inspector: 1 cho MapLv1, 2 cho MapLv2, 3 cho MapLv3

    [Header("Starting Resources")]
    public int startingCoins = 0; // Số coin ban đầu khi vào map

    private void Start()
    {
        InitializeMap();
    }

    private void InitializeMap()
    {
        // Kiểm tra xem đây có phải là lần đầu vào map này không
        if (MapProgressManager.Instance != null)
        {
            // Nếu chưa có map đang chơi dở -> đây là map mới
            if (!MapProgressManager.Instance.HasActiveMap())
            {
                // Đánh dấu bắt đầu map mới
                MapProgressManager.Instance.StartMapProgress(mapLevel);

                Debug.Log($"[MapInitializer] ✓ Khởi tạo map Level {mapLevel} với {startingCoins} coins");
            }
            else
            {
                // Đang tiếp tục map cũ
                int currentMapLevel = MapProgressManager.Instance.GetCurrentMapLevel();

                if (currentMapLevel == mapLevel)
                {
                    Debug.Log($"[MapInitializer] Tiếp tục map Level {mapLevel} đang chơi dở");
                }
                else
                {
                    // Trường hợp lạ: đang có map khác chưa hoàn thành
                    Debug.LogWarning($"[MapInitializer] ⚠ Đang có map Level {currentMapLevel} chưa hoàn thành!");
                }
            }
        }
        else
        {
            Debug.LogError("[MapInitializer] MapProgressManager không tồn tại!");
        }
    }
}