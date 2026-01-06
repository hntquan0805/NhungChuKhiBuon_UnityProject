using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapNode : MonoBehaviour
{
    [Header("UI Components")]
    public Button button;
    public Image shadowImage;
    public Image faceImage;
    public Image iconImage;

    // Tọa độ của nút này trong lưới (để tìm lại trong dữ liệu)
    private int gridX;
    private int gridY;
    private string sceneToLoad;

    // Hàm Setup nhận thêm tọa độ x, y và trạng thái Node
    public void Setup(Sprite icon, Color nodeColor, string sceneName, int x, int y, NodeState state)
    {
        gridX = x;
        gridY = y;
        sceneToLoad = sceneName;

        // 1. Cài đặt Icon
        if (icon != null)
        {
            iconImage.sprite = icon;
            iconImage.gameObject.SetActive(true);
        }
        else iconImage.gameObject.SetActive(false);

        // 2. Cài đặt màu sắc dựa trên trạng thái (State)
        switch (state)
        {
            case NodeState.Locked:
                button.interactable = false;
                faceImage.color = nodeColor * 0.5f; // Tối đi
                shadowImage.color = nodeColor * 0.3f;
                break;

            case NodeState.Unlocked: // Có thể đi
                button.interactable = true;
                faceImage.color = nodeColor; // Sáng bình thường
                shadowImage.color = nodeColor * 0.6f;
                break;

            case NodeState.Completed: // Đã đi xong
                button.interactable = false;
                faceImage.color = Color.gray; // Hóa xám
                shadowImage.color = Color.black;
                break;
        }

        // Đăng ký sự kiện click
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClickNode);
    }

    public void OnClickNode()
    {
        // 1. Cập nhật dữ liệu vào bộ nhớ Tĩnh (Static)
        MapGenerator.OnNodeSelected(gridX, gridY);

        // 2. Chuyển cảnh
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            if (MenuManager.Instance != null)
                MenuManager.Instance.LoadScene(sceneToLoad);
            else
                Debug.LogError("Thiếu MenuManager!");
        }
    }
}