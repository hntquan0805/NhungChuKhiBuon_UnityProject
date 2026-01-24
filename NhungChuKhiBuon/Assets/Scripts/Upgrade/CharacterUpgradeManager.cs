using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CharacterUpgradeManager : MonoBehaviour
{
    private PlayerCharacter currentCharacter;
    private int characterIndex = -1;
    
    [Header("Character Display")]
    public Transform characterSpawnPoint;
    public float characterScale = 300f;
    private GameObject spawnedCharacter;

    [Header("UI - Character Info")]
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI characterClassText;
    public TextMeshProUGUI currentLevelText;
    public Image characterIcon;

    [Header("UI - Current Stats")]
    public TextMeshProUGUI currentHPText;
    public TextMeshProUGUI currentATKText;
    public TextMeshProUGUI currentDEFText;
    public TextMeshProUGUI currentCritText;
    public TextMeshProUGUI currentCritDamText;

    [Header("UI - Resources")]
    public TextMeshProUGUI expCostText;
    public TextMeshProUGUI goldCostText;

    [Header("UI - Buttons")]
    public Button levelUpButton;
    public Button backButton;
    public TextMeshProUGUI levelUpButtonText;

    void Start()
    {
        characterIndex = PlayerPrefs.GetInt("SelectedCharacterIndex", -1);

        if (characterIndex < 0)
        {
            Debug.LogError("[CharacterUpgradeManager] No character selected!");
            return;
        }

        LoadCharacter();
        UpdateUI();

        if (levelUpButton != null)
            levelUpButton.onClick.AddListener(OnLevelUpClicked);

        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);
    }

    void LoadCharacter()
    {
        // Sử dụng CharacterRepository thay vì CharacterDataManager
        List<PlayerCharacter> characterPrefabs = CharacterRepository.Instance.GetAllCharacters();
        
        if (characterPrefabs == null || characterPrefabs.Count == 0)
        {
            Debug.LogError("[CharacterUpgradeManager] Character Prefabs list is empty! Hãy chạy qua Menu scene trước.");
            return;
        }

        if (characterIndex < 0 || characterIndex >= characterPrefabs.Count)
        {
            Debug.LogError($"[CharacterUpgradeManager] Invalid character index: {characterIndex}");
            return;
        }

        PlayerCharacter prefab = characterPrefabs[characterIndex];
        if (prefab == null)
        {
            Debug.LogError($"[CharacterUpgradeManager] Character prefab at index {characterIndex} is null!");
            return;
        }

        if (characterSpawnPoint != null)
        {
            spawnedCharacter = Instantiate(prefab.gameObject, characterSpawnPoint.position, Quaternion.identity, characterSpawnPoint);
            currentCharacter = spawnedCharacter.GetComponent<PlayerCharacter>();
            
            // QUAN TRỌNG: Apply saved level từ CharacterSaveData
            CharacterSaveData.Instance.ApplySavedLevel(currentCharacter);
            
            float scale = characterScale;
            string charName = prefab.name.ToLower();
            if (charName.Contains("mage"))
                scale = 700f;
            else if (charName.Contains("cat"))
                scale = 1200f;
            
            spawnedCharacter.transform.localScale = Vector3.one * scale;
            
            SpriteRenderer spriteRenderer = spawnedCharacter.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                spriteRenderer.flipX = false;
            
            SpriteRenderer[] childRenderers = spawnedCharacter.GetComponentsInChildren<SpriteRenderer>();
            foreach (var renderer in childRenderers)
                renderer.flipX = false;
        }
        else
        {
            currentCharacter = prefab;
        }
    }

    void UpdateUI()
    {
        if (currentCharacter == null)
        {
            Debug.LogError("[CharacterUpgradeManager] Current character is null!");
            return;
        }

        PlayerStats stats = currentCharacter.stats;
        CharacterLevelData levelData = stats.levelData;

        if (characterNameText != null)
        {
            string charName = string.IsNullOrEmpty(stats.characterName) 
                ? currentCharacter.name.Replace("(Clone)", "").Trim() 
                : stats.characterName;
            characterNameText.text = charName;
        }
        
        if (characterClassText != null)
            characterClassText.text = stats.characterClass.ToString();
        
        if (currentLevelText != null)
            currentLevelText.text = $"Level {levelData.currentLevel}";

        if (characterIcon != null && currentCharacter.GetComponent<SpriteRenderer>() != null)
            characterIcon.sprite = currentCharacter.GetComponent<SpriteRenderer>().sprite;

        if (currentHPText != null)
            currentHPText.text = $"HP: {stats.maxHP}";
        
        if (currentATKText != null)
            currentATKText.text = $"ATK: {stats.atk}";
        
        if (currentDEFText != null)
            currentDEFText.text = $"DEF: {stats.def}";
        
        if (currentCritText != null)
            currentCritText.text = $"CRIT: {stats.crit}%";
        
        if (currentCritDamText != null)
            currentCritDamText.text = $"CRIT DMG: {stats.critDam}%";

        if (levelData.CanLevelUp())
        {
            int expCost = levelData.GetExpCostForNextLevel();
            int goldCost = levelData.GetGoldCostForNextLevel();
            int currentExp = PlayerResourceManager.Instance.CurrentExp;
            int currentGold = PlayerResourceManager.Instance.CurrentGold;

            if (expCostText != null)
            {
                expCostText.text = $"{currentExp} / {expCost}";
                expCostText.color = currentExp >= expCost ? Color.black : Color.red;
            }
            
            if (goldCostText != null)
            {
                goldCostText.text = $"{currentGold} / {goldCost}";
                goldCostText.color = currentGold >= goldCost ? Color.black : Color.red;
            }

            bool canAfford = PlayerResourceManager.Instance.CanAffordUpgrade(expCost, goldCost);
            if (levelUpButton != null)
            {
                levelUpButton.interactable = canAfford;
                if (levelUpButtonText != null)
                    levelUpButtonText.text = canAfford ? "LEVEL UP" : "Không đủ tài nguyên";
            }
        }
        else
        {
            if (levelUpButton != null)
            {
                levelUpButton.interactable = false;
                if (levelUpButtonText != null)
                    levelUpButtonText.text = "Đã đạt level tối đa";
            }
        }
    }

    void OnLevelUpClicked()
    {
        if (currentCharacter == null) return;

        if (currentCharacter.stats.LevelUp())
        {
            // Lưu level vào CharacterSaveData
            CharacterSaveData.Instance.SaveCharacterLevel(
                currentCharacter.stats.characterName, 
                currentCharacter.stats.levelData.currentLevel
            );
            
            currentCharacter.RefreshStatsAfterLevelUp();
            UpdateUI();
            
            Debug.Log($"[CharacterUpgradeManager] ✓ Nâng cấp {currentCharacter.stats.characterName} lên level {currentCharacter.stats.levelData.currentLevel}");
        }
    }

    void OnBackClicked()
    {
        if (spawnedCharacter != null)
            Destroy(spawnedCharacter);
        
        UnityEngine.SceneManagement.SceneManager.LoadScene("ListUpgrade");
    }
}
