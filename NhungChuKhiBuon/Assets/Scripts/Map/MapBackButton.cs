using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script cho nút Back trong Map scene
/// Khi nhấn sẽ hiển thị popup xác nhận thoát
/// </summary>
public class MapBackButton : MonoBehaviour
{
    [Header("References")]
    public Button backButton;
    public ExitConfirmationPopup exitPopup;

    [Header("Optional - Auto Find Popup")]
    public bool autoFindPopup = true;

    private void Start()
    {
        // Tự động tìm button nếu chưa gán
        if (backButton == null)
            backButton = GetComponent<Button>();

        // Tự động tìm popup nếu chưa gán
        if (exitPopup == null && autoFindPopup)
            exitPopup = FindObjectOfType<ExitConfirmationPopup>();

        // Gán sự kiện
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }
        else
        {
            Debug.LogError("[MapBackButton] Không tìm thấy Button component!");
        }

        if (exitPopup == null)
        {
            Debug.LogError("[MapBackButton] Không tìm thấy ExitConfirmationPopup trong scene! Hãy gán popup vào inspector.");
        }
    }

    /// <summary>
    /// Xử lý khi nhấn nút Back
    /// </summary>
    private void OnBackButtonClicked()
    {
        if (exitPopup != null)
        {
            // Enable popup parent object trước
            if (!exitPopup.gameObject.activeSelf)
                exitPopup.gameObject.SetActive(true);
            
            exitPopup.ShowPopup();
            Debug.Log("[MapBackButton] Hiển thị popup xác nhận thoát");
        }
        else
        {
            Debug.LogWarning("[MapBackButton] Không có popup để hiển thị! Quay lại menu trực tiếp.");
            // Fallback: quay lại menu nếu không có popup
            if (MenuManager.Instance != null)
                MenuManager.Instance.LoadScene("MapChoose");
        }
    }
}
