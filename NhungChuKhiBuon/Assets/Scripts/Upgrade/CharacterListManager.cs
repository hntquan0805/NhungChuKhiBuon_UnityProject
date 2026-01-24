using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CharacterListManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform characterListContainer;
    public GameObject characterListItemPrefab;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI goldText;
    public Button backButton;

    private List<GameObject> spawnedItems = new List<GameObject>();

    void Start()
    {
        // Sử dụng CharacterRepository thay vì characterPrefabs local
        if (!CharacterRepository.Instance.IsInitialized())
        {
            Debug.LogError("[CharacterListManager] ⚠ CharacterRepository chưa được khởi tạo! Hãy chạy qua Menu scene trước.");
        }

        UpdateResourceUI();
        PopulateCharacterList();

        if (backButton != null)
            backButton.onClick.AddListener(OnBackButtonClicked);
    }

    void UpdateResourceUI()
    {
        if (expText != null)
            expText.text = $"EXP: {PlayerResourceManager.Instance.CurrentExp}";

        if (goldText != null)
            goldText.text = $"Gold: {PlayerResourceManager.Instance.CurrentGold}";
    }

    void PopulateCharacterList()
    {
        foreach (var item in spawnedItems)
            Destroy(item);
        spawnedItems.Clear();

        // Lấy danh sách characters từ CharacterRepository
        List<PlayerCharacter> characterPrefabs = CharacterRepository.Instance.GetAllCharacters();

        if (characterPrefabs == null || characterPrefabs.Count == 0)
        {
            Debug.LogWarning("[CharacterListManager] Không có character nào trong CharacterRepository!");
            return;
        }

        for (int i = 0; i < characterPrefabs.Count; i++)
        {
            PlayerCharacter character = characterPrefabs[i];
            if (character == null) continue;

            // Apply saved level trước khi hiển thị
            CharacterSaveData.Instance.ApplySavedLevel(character);

            GameObject itemGO = Instantiate(characterListItemPrefab, characterListContainer);
            CharacterListItem item = itemGO.GetComponent<CharacterListItem>();
            
            if (item != null)
                item.Setup(character, i);

            spawnedItems.Add(itemGO);
        }

        Debug.Log($"[CharacterListManager] ✓ Đã tạo {characterPrefabs.Count} character list items");
    }

    void OnBackButtonClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}
