using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryUIController : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string menuSceneName = "Menu";

    // Gán hàm này cho Button → OnClick()
    public void OnContinueButton()
    {
        // (Optional) Clear time scale nếu trước đó có pause
        Time.timeScale = 1f;

        SceneManager.LoadScene(menuSceneName);
    }
}
