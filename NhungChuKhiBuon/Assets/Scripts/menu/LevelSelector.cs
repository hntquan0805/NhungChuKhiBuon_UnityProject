using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelector : MonoBehaviour
{
    [Header("Level Buttons")]
    public Button level1Button;
    public Button level2Button;
    public Button level3Button;

    [Header("Navigation")]
    public Button backButton;

    [Header("Button Colors")]
    public Color lockedColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    public Color unlockedColor = Color.white;

    private const string SELECTED_LEVEL_KEY = "SelectedLevel";

    private void Start()
    {
        // Kiểm tra xem có map đang chơi dở không
        CheckActiveMapAndUpdateUI();

        if (level1Button != null)
            level1Button.onClick.AddListener(() => SelectLevel(1));

        if (level2Button != null)
            level2Button.onClick.AddListener(() => SelectLevel(2));

        if (level3Button != null)
            level3Button.onClick.AddListener(() => SelectLevel(3));

        if (backButton != null)
            backButton.onClick.AddListener(BackToMenu);
    }

    /// <summary>
    /// Kiểm tra có map đang chơi dở và cập nhật UI
    /// </summary>
    private void CheckActiveMapAndUpdateUI()
    {
        if (MapProgressManager.Instance == null || !MapProgressManager.Instance.HasActiveMap())
        {
            // Không có map đang chơi dở - enable tất cả nút
            EnableAllButtons();
        }
        else
        {
            // Có map đang chơi dở - chỉ enable nút map cũ
            int currentMapLevel = MapProgressManager.Instance.GetCurrentMapLevel();
            DisableAllButtonsExcept(currentMapLevel);
            
            Debug.Log($"[LevelSelector] Đang có map Level {currentMapLevel} chưa hoàn thành. Các map khác bị khóa.");
        }
    }

    /// <summary>
    /// Enable tất cả các nút map
    /// </summary>
    private void EnableAllButtons()
    {
        SetButtonState(level1Button, true);
        SetButtonState(level2Button, true);
        SetButtonState(level3Button, true);
    }

    /// <summary>
    /// Disable tất cả nút trừ nút của map đang chơi
    /// </summary>
    private void DisableAllButtonsExcept(int activeLevel)
    {
        SetButtonState(level1Button, activeLevel == 1);
        SetButtonState(level2Button, activeLevel == 2);
        SetButtonState(level3Button, activeLevel == 3);
    }

    /// <summary>
    /// Set trạng thái enable/disable cho button và đổi màu
    /// </summary>
    private void SetButtonState(Button button, bool enabled)
    {
        if (button == null) return;

        button.interactable = enabled;
        
        // Đổi màu để rõ ràng hơn
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
            buttonImage.color = enabled ? unlockedColor : lockedColor;
    }

    private void SelectLevel(int level)
    {
        // Lưu level đã chọn vào PlayerPrefs
        PlayerPrefs.SetInt(SELECTED_LEVEL_KEY, level);
        PlayerPrefs.Save();

        Debug.Log($"Đã chọn Level {level}");

        // Kiểm tra xem có map đang chơi dở không
        if (MapProgressManager.Instance != null && MapProgressManager.Instance.HasActiveMap())
        {
            int currentMapLevel = MapProgressManager.Instance.GetCurrentMapLevel();
            
            // Nếu chọn map cũ (map đang chơi dở) -> nhảy thẳng vào map
            if (level == currentMapLevel)
            {
                Debug.Log($"[LevelSelector] Tiếp tục map cũ Level {level} - Nhảy thẳng vào map");
                string mapScene = GetMapSceneByLevel(level);
                MenuManager.Instance.LoadScene(mapScene);
                return;
            }
        }

        // Nếu không có map cũ hoặc chọn map mới -> qua TeamSelection
        MenuManager.Instance.LoadScene("TeamSelection");
    }

    /// <summary>
    /// Lấy tên scene map theo level
    /// </summary>
    private string GetMapSceneByLevel(int level)
    {
        switch (level)
        {
            case 1:
                return "MapLv1";
            case 2:
                return "MapLv2";
            case 3:
                return "MapLv3";
            default:
                Debug.LogWarning($"⚠ Level không hợp lệ: {level}. Sử dụng MapLv1 mặc định.");
                return "MapLv1";
        }
    }

    private void BackToMenu()
    {
        MenuManager.Instance.LoadScene("Menu");
    }

    // Helper method để lấy level đã chọn (dùng trong TeamSelectionManager)
    public static int GetSelectedLevel()
    {
        return PlayerPrefs.GetInt(SELECTED_LEVEL_KEY, 1); // Mặc định là Level 1
    }

#if UNITY_EDITOR
    /// <summary>
    /// Reset map progress khi thoát Play mode trong Unity Editor
    /// </summary>
    private void OnApplicationQuit()
    {
        if (MapProgressManager.Instance != null)
        {
            MapProgressManager.Instance.ClearMapProgress();
            Debug.Log("[LevelSelector] ✅ Reset map progress khi thoát Play mode");
        }
    }
#endif
}
