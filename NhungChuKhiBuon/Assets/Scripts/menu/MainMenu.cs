using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Các nút bấm")]
    public Button playButton;
    public Button enterCasinoButton;
    public Button enterRestAreaButton;
    public Button hospital;
    public Button upgradeButton;

    void Start()
    {
        if (playButton != null)
            playButton.onClick.AddListener(EnterMap);

        if (enterCasinoButton != null)
            enterCasinoButton.onClick.AddListener(EnterCasino);

        if (enterRestAreaButton != null)
            enterRestAreaButton.onClick.AddListener(EnterRestArea);

        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(EnterUpgrade);
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

    void EnterUpgrade()
    {
        MenuManager.Instance.LoadScene("ListUpgrade");
    }
}