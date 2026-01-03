using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Các nút bấm")]
    public Button playButton;
    public Button enterCasinoButton;
    public Button enterRestAreaButton;

    //[Header("Optional - Team HP Display")]
    //public TeamHPDisplay teamHPDisplay;

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
        MenuManager.Instance.LoadScene("Map");
    }

    void EnterCasino()
    {
        Debug.Log("[Menu] Entering Casino...");
        MenuManager.Instance.LoadScene(MenuManager.CASINO_SCENE);
    }

    void EnterRestArea()
    {
        Debug.Log("[Menu] Entering Rest Area...");
        MenuManager.Instance.LoadScene(MenuManager.REST_AREA_SCENE);
    }

    // Cập nhật trạng thái các nút dựa vào team
    void UpdateButtonStates()
    {
        if (PersistentTeamManager.Instance == null)
            return;

        bool teamAlive = PersistentTeamManager.Instance.IsTeamAlive();

        // Battle và Arena chỉ available khi team còn sống
        if (enterBattleButton != null)
            enterBattleButton.interactable = teamAlive;

        if (enterArenaButton != null)
            enterArenaButton.interactable = teamAlive;

        // Rest Area luôn available
        if (enterRestAreaButton != null)
            enterRestAreaButton.interactable = true;

        // Casino luôn available
        if (enterCasinoButton != null)
            enterCasinoButton.interactable = true;
    }

    /// <summary>
    /// Clear all shields when returning to menu (optional feature)
    /// </summary>
    void ClearTeamShields()
    {
        if (PersistentTeamManager.Instance == null)
            return;

        PersistentTeamManager.Instance.ClearTeamShield();

        Debug.Log("[Menu] Team shields cleared");
    }

    // Cập nhật hiển thị team
    public void RefreshTeamDisplay()
    {
        //if (teamHPDisplay != null)
        //{
        //    teamHPDisplay.UpdateDisplay();
        //}

        if (PersistentTeamManager.Instance != null)
        {
            PersistentTeamManager.Instance.LogTeamStatus();
        }
    }
}