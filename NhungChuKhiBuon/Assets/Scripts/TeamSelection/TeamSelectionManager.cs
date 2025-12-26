using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class TeamSelectionManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform availableHeroesParent;
    public SlotController[] selectedSlots;

    [Header("Settings")]
    public Color disabledColor = new Color(1f, 1f, 1f, 0.3f);

    [Header("Scene Management")]
    public string nextSceneName = "Menu"; // Chuyển đến Menu thay vì TestScene

    [Header("Optional")]
    public Button startGameButton;

    private List<HeroAvatar> selectedHeroes = new List<HeroAvatar>();
    private int maxTeamSize = 3;

    private void Start()
    {
        HeroAvatar[] heroes = availableHeroesParent.GetComponentsInChildren<HeroAvatar>();
        foreach (var hero in heroes)
        {
            hero.Initialize(this);
        }

        foreach (var slot in selectedSlots)
        {
            if (slot != null)
            {
                slot.ClearSlot();
            }
        }

        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(OnStartGameClicked);
        }
    }

    private void Update()
    {
        // Nhấn Enter để chuyển scene
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnStartGameClicked();
        }
    }

    public void OnHeroClicked(HeroAvatar hero)
    {
        if (hero.isSelected)
        {
            DeselectHero(hero);
        }
        else
        {
            if (selectedHeroes.Count < maxTeamSize)
            {
                SelectHero(hero);
            }
            else
            {
                Debug.Log("Team đã đủ " + maxTeamSize + " heroes!");
            }
        }
    }

    private void SelectHero(HeroAvatar hero)
    {
        selectedHeroes.Add(hero);
        hero.SetSelected(true, disabledColor);

        int slotIndex = selectedHeroes.Count - 1;
        UpdateSlot(slotIndex, hero.fullBodySprite);

        Debug.Log("Đã chọn: " + hero.heroName + " vào Slot " + (slotIndex + 1));
    }

    private void DeselectHero(HeroAvatar hero)
    {
        int index = selectedHeroes.IndexOf(hero);
        if (index == -1) return;

        selectedHeroes.RemoveAt(index);
        hero.SetSelected(false, Color.white);
        RefreshSlots();

        Debug.Log("Đã bỏ chọn: " + hero.heroName);
    }

    private void UpdateSlot(int slotIndex, Sprite heroSprite)
    {
        if (slotIndex < 0 || slotIndex >= selectedSlots.Length) return;

        if (selectedSlots[slotIndex] != null)
        {
            selectedSlots[slotIndex].SetHero(heroSprite);
        }
    }

    private void RefreshSlots()
    {
        for (int i = 0; i < selectedSlots.Length; i++)
        {
            if (selectedSlots[i] != null)
            {
                selectedSlots[i].ClearSlot();
            }
        }

        for (int i = 0; i < selectedHeroes.Count; i++)
        {
            UpdateSlot(i, selectedHeroes[i].fullBodySprite);
        }
    }

    public void OnStartGameClicked()
    {
        if (selectedHeroes.Count == maxTeamSize)
        {
            Debug.Log("╔════════════════════════════════════╗");
            Debug.Log("║       BẮT ĐẦU CHUYỂN SCENE!         ║");
            Debug.Log("╚════════════════════════════════════╝");
            Debug.Log("");
            Debug.Log("✓ Team đã chọn gồm " + selectedHeroes.Count + " heroes:");

            for (int i = 0; i < selectedHeroes.Count; i++)
            {
                Debug.Log($"  [{i + 1}] {selectedHeroes[i].heroName}");
            }

            Debug.Log("");
            Debug.Log($"→ Đang chuyển sang scene: {nextSceneName}");
            Debug.Log("════════════════════════════════════");

            // Lưu team data vào TeamDataManager
            SaveTeamData();

            // ===== NEW: Khởi tạo PersistentTeamManager =====
            InitializePersistentTeam();

            // Chuyển scene
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning($"⚠ Chưa đủ heroes! Cần {maxTeamSize}, đang chọn: {selectedHeroes.Count}");
        }
    }

    private void SaveTeamData()
    {
        // Tạo TeamDataManager nếu chưa có
        if (TeamDataManager.Instance == null)
        {
            GameObject dataManager = new GameObject("TeamDataManager");
            dataManager.AddComponent<TeamDataManager>();
        }

        // Lưu team
        TeamDataManager.Instance.SetSelectedTeam(selectedHeroes);
    }

    // ===== NEW: Initialize Persistent Team =====
    private void InitializePersistentTeam()
    {
        // Tạo PersistentTeamManager nếu chưa có
        if (PersistentTeamManager.Instance == null)
        {
            GameObject persistentManager = new GameObject("PersistentTeamManager");
            persistentManager.AddComponent<PersistentTeamManager>();
        }

        // Clear old data nếu có
        PersistentTeamManager.Instance.ClearTeamData();

        // Initialize từ team selection
        PersistentTeamManager.Instance.InitializeFromTeamSelection();

        Debug.Log("✓ PersistentTeamManager initialized with full HP team");
    }

    public List<HeroAvatar> GetSelectedHeroes()
    {
        return selectedHeroes;
    }
}