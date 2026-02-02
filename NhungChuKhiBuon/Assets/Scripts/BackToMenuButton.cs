using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenuButton : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Tên scene Menu (phải đúng với tên trong Build Settings)")]
    public string menuSceneName = "Menu";

    /// <summary>
    /// Gọi hàm này khi click Button
    /// </summary>
    public void GoToMenu()
    {
        Debug.Log("[BackToMenuButton] Loading Menu scene...");
        SceneManager.LoadScene(menuSceneName);
    }
}
