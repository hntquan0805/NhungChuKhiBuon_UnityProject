using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapNode : MonoBehaviour
{
    [Header("Các thành phần UI")]
    public Button button;
    public Image shadowImage;
    public Image faceImage;
    public Image iconImage;

    [Header("Dữ liệu")]
    private string sceneToLoad;

    public List<MapNode> outgoingNodes = new List<MapNode>();

    public void Setup(Sprite icon, Color nodeColor, bool locked, string sceneName)
    {
        sceneToLoad = sceneName;

        if (icon != null)
        {
            iconImage.sprite = icon;
            iconImage.gameObject.SetActive(true);
        }
        else iconImage.gameObject.SetActive(false);

        faceImage.color = nodeColor;
        shadowImage.color = nodeColor * 0.6f;

        if (locked) button.interactable = false;
        else button.interactable = true;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClickNode);
    }

    public void OnClickNode()
    {
        Debug.Log("Bấm nút: " + gameObject.name + " -> Vào scene: " + sceneToLoad);

        faceImage.color = Color.gray;
        shadowImage.color = Color.black;
        button.interactable = false;

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            if (MenuManager.Instance != null)
                MenuManager.Instance.LoadScene(sceneToLoad);
            else
                Debug.LogError("Lỗi: Không tìm thấy MenuManager! Bạn đã chạy game từ màn hình Menu chưa?");
        }
        else
        {
            Debug.LogError("Quên nhập tên Scene cho nút này rồi!");
        }
    }
}