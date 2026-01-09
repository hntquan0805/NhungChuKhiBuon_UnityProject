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
        MapGenerator.OnNodeSelected(gridX, gridY);

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            if (MenuManager.Instance != null)
                MenuManager.Instance.LoadScene(sceneToLoad);
        }
    }
}