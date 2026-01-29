using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Popup xác nhận khi người chơi muốn rời khỏi map
/// Có 2 lựa chọn: Tạm rời (giữ tiến trình) và Thoát (xóa tiến trình)
/// </summary>
public class ExitConfirmationPopup : MonoBehaviour
{
    [Header("UI References")]
    public GameObject popupPanel;
    public Button pauseButton;           // Nút "Tạm rời"
    public Button quitButton;            // Nút "Thoát"
    public Button closeButton;           // Nút đóng popup (X)
    
    [Header("Optional Text")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI messageText;

    [Header("Scene Settings")]
    public string mapChooseSceneName = "MapChoose";

    private void Start()
    {
        // Đảm bảo popup ẩn khi bắt đầu
        if (popupPanel != null)
            popupPanel.SetActive(false);

        // Gán sự kiện cho các nút
        if (pauseButton != null)
            pauseButton.onClick.AddListener(OnPauseClicked);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
        
        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseClicked);

        // Thiết lập text mặc định
        if (titleText != null)
            titleText.text = "Rời khỏi Map?";
        
        if (messageText != null)
            messageText.text = "Bạn muốn tạm rời hay thoát hoàn toàn?";
    }

    /// <summary>
    /// Hiển thị popup xác nhận
    /// </summary>
    public void ShowPopup()
    {
        // Enable cả parent object (ExitConfirmationPopup)
        gameObject.SetActive(true);
        
        if (popupPanel != null)
            popupPanel.SetActive(true);
        else
            Debug.Log("[ExitConfirmationPopup] popupPanel chưa được gán trong Inspector!");
        Debug.Log("[ExitConfirmationPopup] Popup hiển thị");
    }

    /// <summary>
    /// Ẩn popup
    /// </summary>
    public void HidePopup()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
        
        // Disable cả parent object (ExitConfirmationPopup)
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Xử lý khi nhấn nút "Tạm rời"
    /// Quay lại menu nhưng GIỮ tiến trình map
    /// </summary>
    private void OnPauseClicked()
    {
        Debug.Log("[ExitConfirmationPopup] Người chơi chọn 'Tạm rời'");
        
        if (MapProgressManager.Instance != null)
            MapProgressManager.Instance.PauseMapProgress();
        
        // Quay lại màn hình chọn map
        ReturnToMapSelection();
    }

    /// <summary>
    /// Xử lý khi nhấn nút "Thoát"
    /// Quay lại menu và XÓA tiến trình map
    /// </summary>
    private void OnQuitClicked()
    {
        Debug.Log("[ExitConfirmationPopup] Người chơi chọn 'Thoát' - Xóa tiến trình");
        
        if (MapProgressManager.Instance != null)
            MapProgressManager.Instance.ClearMapProgress();
        
        // Quay lại màn hình chọn map
        ReturnToMapSelection();
    }

    /// <summary>
    /// Đóng popup mà không làm gì (nút X)
    /// </summary>
    private void OnCloseClicked()
    {
        Debug.Log("[ExitConfirmationPopup] Đóng popup");
        HidePopup();
    }

    /// <summary>
    /// Quay lại màn hình chọn map
    /// </summary>
    private void ReturnToMapSelection()
    {
        HidePopup();
        
        if (MenuManager.Instance != null)
            MenuManager.Instance.LoadScene(mapChooseSceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(mapChooseSceneName);
    }
}
