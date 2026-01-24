using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Lưu trữ dữ liệu level và stats của từng character
/// Singleton pattern - tồn tại xuyên suốt các scene và lưu vào PlayerPrefs
/// </summary>
public class CharacterSaveData : MonoBehaviour
{
    private static CharacterSaveData instance;
    public static CharacterSaveData Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("CharacterSaveData");
                instance = go.AddComponent<CharacterSaveData>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    // Lưu level của từng character theo tên
    private Dictionary<string, int> characterLevels = new Dictionary<string, int>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllCharacterData();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Lấy level của character
    /// </summary>
    public int GetCharacterLevel(string characterName)
    {
        if (characterLevels.ContainsKey(characterName))
            return characterLevels[characterName];
        
        return 10; // Level mặc định (MIN_LEVEL)
    }

    /// <summary>
    /// Lưu level của character
    /// </summary>
    public void SaveCharacterLevel(string characterName, int level)
    {
        characterLevels[characterName] = level;
        
        // Lưu vào PlayerPrefs
        PlayerPrefs.SetInt($"Character_{characterName}_Level", level);
        PlayerPrefs.Save();
        
        Debug.Log($"[CharacterSaveData] Đã lưu {characterName} level {level}");
    }

    /// <summary>
    /// Load tất cả dữ liệu character từ PlayerPrefs
    /// </summary>
    private void LoadAllCharacterData()
    {
        characterLevels.Clear();
        
        // Load data từ CharacterRepository nếu đã khởi tạo
        if (CharacterRepository.Instance.IsInitialized())
        {
            List<PlayerCharacter> characters = CharacterRepository.Instance.GetAllCharacters();
            foreach (var character in characters)
            {
                if (character == null) continue;
                
                string charName = character.stats.characterName;
                int savedLevel = PlayerPrefs.GetInt($"Character_{charName}_Level", 10);
                characterLevels[charName] = savedLevel;
            }
        }
        
        Debug.Log($"[CharacterSaveData] Đã load dữ liệu cho {characterLevels.Count} characters");
    }

    /// <summary>
    /// Apply level đã lưu vào character
    /// </summary>
    public void ApplySavedLevel(PlayerCharacter character)
    {
        if (character == null) return;
        
        string charName = character.stats.characterName;
        int savedLevel = GetCharacterLevel(charName);
        
        character.stats.levelData.currentLevel = savedLevel;
        
        Debug.Log($"[CharacterSaveData] Đã apply level {savedLevel} cho {charName}");
    }

    /// <summary>
    /// Reset tất cả character về level 10
    /// </summary>
    public void ResetAllCharacters()
    {
        foreach (var charName in characterLevels.Keys)
        {
            PlayerPrefs.DeleteKey($"Character_{charName}_Level");
        }
        
        characterLevels.Clear();
        PlayerPrefs.Save();
        
        Debug.Log("[CharacterSaveData] Đã reset tất cả characters về level 10");
    }
}
