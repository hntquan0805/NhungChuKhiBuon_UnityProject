using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Buttons")]
    public Button enterBattleButton;
    public Button enterArenaButton;
    public Button enterCasinoButton;
    public Button enterRestAreaButton;

    //[Header("Optional - Team HP Display")]
    //public TeamHPDisplay teamHPDisplay;

    void Start()
    {
        // Setup button listeners
        if (enterBattleButton != null)
            enterBattleButton.onClick.AddListener(EnterBattle);

        if (enterArenaButton != null)
            enterArenaButton.onClick.AddListener(EnterArena);

        if (enterCasinoButton != null)
            enterCasinoButton.onClick.AddListener(EnterCasino);

        if (enterRestAreaButton != null)
            enterRestAreaButton.onClick.AddListener(EnterRestArea);

        // Update team HP display
        //if (teamHPDisplay != null)
        //{
        //    teamHPDisplay.UpdateDisplay();
        //}

        // Log team status when entering menu
        if (PersistentTeamManager.Instance != null)
        {
            PersistentTeamManager.Instance.LogTeamStatus();

            // Optional: Clear shields when returning to menu
            ClearTeamShields();
        }
    }

    void Update()
    {
        // Optional: Update button states based on team status
        UpdateButtonStates();
    }

    void EnterBattle()
    {
        if (PersistentTeamManager.Instance == null || !PersistentTeamManager.Instance.IsTeamAlive())
        {
            Debug.LogWarning("[Menu] Team is dead or not initialized! Visit Rest Area first.");
            return;
        }

        Debug.Log("[Menu] Entering Battle...");
        MenuManager.Instance.LoadScene(MenuManager.BATTLE_SCENE);
    }

    void EnterArena()
    {
        if (PersistentTeamManager.Instance == null || !PersistentTeamManager.Instance.IsTeamAlive())
        {
            Debug.LogWarning("[Menu] Team is dead or not initialized! Visit Rest Area first.");
            return;
        }

        // Cảnh báo về Arena debuff
        Debug.Log("[Menu] ⚠ Entering Arena - Your team will lose 30% HP at start!");
        MenuManager.Instance.LoadScene(MenuManager.ARENA_SCENE);
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