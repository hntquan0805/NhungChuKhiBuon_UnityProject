using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Các nút bấm")]
    public Button playButton;
    public Button enterCasinoButton;
    public Button enterRestAreaButton;

    void Start()
    {
        if (playButton != null)
            playButton.onClick.AddListener(EnterMap);

        if (enterCasinoButton != null)
            enterCasinoButton.onClick.AddListener(EnterCasino);

        if (enterRestAreaButton != null)
            enterRestAreaButton.onClick.AddListener(EnterRestArea);
    }

    void EnterMap()
    {
        MenuManager.Instance.LoadScene("TeamSelection");
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