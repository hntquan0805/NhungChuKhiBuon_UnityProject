using UnityEngine;

/// <summary>
/// Quản lý tiến trình của người chơi trong map
/// Lưu trạng thái map đang chơi và cho phép tạm rời/thoát game
/// </summary>
public class MapProgressManager : MonoBehaviour
{
    public static MapProgressManager Instance { get; private set; }

    private const string CURRENT_MAP_KEY = "CurrentMapInProgress";
    private const string HAS_ACTIVE_MAP_KEY = "HasActiveMap";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Đánh dấu đã bắt đầu chơi một map
    /// </summary>
    public void StartMapProgress(int mapLevel)
    {
        PlayerPrefs.SetInt(CURRENT_MAP_KEY, mapLevel);
        PlayerPrefs.SetInt(HAS_ACTIVE_MAP_KEY, 1);
        PlayerPrefs.Save();

        // ✅ Chỉ reset coins, KHÔNG clear team data
        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.ResetCoins();
            Debug.Log("[MapProgressManager] ✓ Đã reset coin về 0 khi bắt đầu map (giữ nguyên team)");
        }

        Debug.Log($"[MapProgressManager] Đã bắt đầu map Level {mapLevel}");
    }

    /// <summary>
    /// Kiểm tra có map đang chơi dở không
    /// </summary>
    public bool HasActiveMap()
    {
        return PlayerPrefs.GetInt(HAS_ACTIVE_MAP_KEY, 0) == 1;
    }

    /// <summary>
    /// Lấy level của map đang chơi dở
    /// </summary>
    public int GetCurrentMapLevel()
    {
        return PlayerPrefs.GetInt(CURRENT_MAP_KEY, 0);
    }

    /// <summary>
    /// Xóa tiến trình map hiện tại (khi người chơi chọn "Thoát")
    /// </summary>
    public void ClearMapProgress()
    {
        PlayerPrefs.DeleteKey(CURRENT_MAP_KEY);
        PlayerPrefs.DeleteKey(HAS_ACTIVE_MAP_KEY);
        PlayerPrefs.Save();

        // Xóa luôn dữ liệu map đã generate
        MapGenerator.savedMapData = null;

        // ✅ Reset coins VÀ clear team khi thoát map
        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.ResetCoins();
            MenuManager.Instance.ClearTeamData();
            Debug.Log("[MapProgressManager] ✓ Đã reset coin và clear team khi thoát map");
        }

        Debug.Log("[MapProgressManager] ✓ Đã xóa tiến trình map");
    }

    /// <summary>
    /// Tạm rời map (giữ lại tiến trình)
    /// </summary>
    public void PauseMapProgress()
    {
        Debug.Log($"[MapProgressManager] Tạm rời map Level {GetCurrentMapLevel()}");
        // Không làm gì cả, chỉ để thông báo
    }

    /// <summary>
    /// Hoàn thành map (khi người chơi đánh bại boss)
    /// </summary>
    public void CompleteMap()
    {
        ClearMapProgress();
        Debug.Log("[MapProgressManager] ✓ Đã hoàn thành map");
    }

    /// <summary>
    /// Lấy tên scene map tương ứng với level hiện tại
    /// </summary>
    public string GetCurrentMapScene()
    {
        int level = GetCurrentMapLevel();

        switch (level)
        {
            case 1:
                return "MapLv1";
            case 2:
                return "MapLv2";
            case 3:
                return "MapLv3";
            default:
                Debug.LogWarning($"[MapProgressManager] Level không hợp lệ: {level}. Sử dụng MapLv1 mặc định.");
                return "MapLv1";
        }
    }
}