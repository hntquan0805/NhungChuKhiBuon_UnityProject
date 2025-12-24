using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button enterCasinoButton;
    public Button enterRestAreaButton;

    void Start()
    {
        enterCasinoButton.onClick.AddListener(EnterCasino);
        enterRestAreaButton.onClick.AddListener(EnterRestArea);
    }

    void EnterCasino()
    {
        MenuManager.Instance.LoadScene("BettingScene");
    }

    void EnterRestArea()
    {
        MenuManager.Instance.LoadScene("RestArea");
    }
}