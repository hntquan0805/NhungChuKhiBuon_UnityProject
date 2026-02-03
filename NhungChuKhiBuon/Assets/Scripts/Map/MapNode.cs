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

    public void Setup(Sprite icon, Color nodeColor, string sceneName, int x, int y, NodeState state)
    {
        gridX = x;
        gridY = y;
        sceneToLoad = sceneName;

        if (icon != null)
        {
            iconImage.sprite = icon;
            iconImage.gameObject.SetActive(true);
        }
        else iconImage.gameObject.SetActive(false);

        switch (state)
        {
            case NodeState.Locked:
                button.interactable = false;
                faceImage.color = nodeColor * 0.5f;
                shadowImage.color = nodeColor * 0.3f;
                break;

            case NodeState.Unlocked:
                button.interactable = true;
                faceImage.color = nodeColor;
                shadowImage.color = nodeColor * 0.6f;
                break;

            case NodeState.Completed:
                button.interactable = false;
                faceImage.color = Color.gray;
                shadowImage.color = Color.black;
                break;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClickNode);
    }

    public void OnClickNode()
    {
        Debug.Log($"MapNode: Đã click vào Node tại [{gridX}, {gridY}]");

        MapGenerator.OnNodeSelected(gridX, gridY);

        // --- SỬA ĐOẠN NÀY ---
        // Nếu Instance bị null, thử tìm lại một lần nữa cho chắc
        if (MapPlayerController.Instance == null)
        {
            MapPlayerController.Instance = FindFirstObjectByType<MapPlayerController>(); // Hoặc FindFirstObjectByType trong Unity mới
        }

        if (MapPlayerController.Instance != null)
        {
            button.interactable = false;
            Debug.Log("MapNode: Đang gọi MapPlayer di chuyển...");
            MapPlayerController.Instance.MoveToNode(this.transform.position, () =>
            {
                Debug.Log("MapNode: Callback nhận được -> Chuyển cảnh ngay.");
                EnterScene();
            });
        }
        else
        {
            // Nếu tìm mọi cách vẫn không thấy thì đành chịu
            Debug.LogError("LỖI: Vẫn không tìm thấy MapPlayerController! Hãy kiểm tra xem GameObject MapPlayer có bật không?");
            EnterScene();
        }
    }

    // Tách hàm load scene ra riêng cho gọn
    void EnterScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            if (MenuManager.Instance != null)
            {
                MenuManager.Instance.LoadScene(sceneToLoad);
            }
            else
            {
                // Dự phòng
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}