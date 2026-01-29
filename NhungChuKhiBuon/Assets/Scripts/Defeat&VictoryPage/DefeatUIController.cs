using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DefeatUIController : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button backToMenuButton;

    private void Start()
    {
        if (backToMenuButton != null)
        {
            backToMenuButton.onClick.AddListener(OnBackToMenuButton);
        }
    }

    public void OnBackToMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
