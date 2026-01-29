using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryUIController : MonoBehaviour
{
    // Gán hàm này cho Button → OnClick()
    public void OnContinueButton()
    {
        // (Optional) Clear time scale nếu trước đó có pause
        Time.timeScale = 1f;

        // Quay về MapLv tương ứng
        if (MapProgressManager.Instance != null && MapProgressManager.Instance.HasActiveMap())
        {
            string mapScene = MapProgressManager.Instance.GetCurrentMapScene();
            SceneManager.LoadScene(mapScene);
        }
        else
        {
            SceneManager.LoadScene("Map");
        }
    }
}
