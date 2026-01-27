using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    [Header("Level Buttons")]
    public Button level1Button;
    public Button level2Button;
    public Button level3Button;

    [Header("Navigation")]
    public Button backButton;

    private const string SELECTED_LEVEL_KEY = "SelectedLevel";

    private void Start()
    {
        if (level1Button != null)
            level1Button.onClick.AddListener(() => SelectLevel(1));

        if (level2Button != null)
            level2Button.onClick.AddListener(() => SelectLevel(2));

        if (level3Button != null)
            level3Button.onClick.AddListener(() => SelectLevel(3));

        if (backButton != null)
            backButton.onClick.AddListener(BackToMenu);
    }

    private void SelectLevel(int level)
    {
        // Lưu level đã chọn vào PlayerPrefs
        PlayerPrefs.SetInt(SELECTED_LEVEL_KEY, level);
        PlayerPrefs.Save();

        Debug.Log($"Đã chọn Level {level}");

        // Chuyển đến TeamSelection scene
        MenuManager.Instance.LoadScene("TeamSelection");
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
}
