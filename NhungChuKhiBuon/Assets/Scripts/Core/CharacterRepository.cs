using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Kho lưu trữ chung cho tất cả characters trong game.
/// Singleton pattern - tồn tại xuyên suốt các scene.
/// </summary>
public class CharacterRepository : MonoBehaviour
{
    private static CharacterRepository instance;
    public static CharacterRepository Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("CharacterRepository");
                instance = go.AddComponent<CharacterRepository>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    [Header("Character Database")]
    [SerializeField] private List<PlayerCharacter> allCharacters = new List<PlayerCharacter>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Khởi tạo danh sách characters từ Menu scene
    /// </summary>
    public void InitializeCharacters(List<PlayerCharacter> characters)
    {
        if (characters == null || characters.Count == 0)
        {
            Debug.LogWarning("[CharacterRepository] Danh sách characters trống!");
            return;
        }

        allCharacters = new List<PlayerCharacter>(characters);
        Debug.Log($"[CharacterRepository] Đã khởi tạo {allCharacters.Count} characters");
    }

    /// <summary>
    /// Lấy toàn bộ danh sách characters
    /// </summary>
    public List<PlayerCharacter> GetAllCharacters()
    {
        return new List<PlayerCharacter>(allCharacters);
    }

    /// <summary>
    /// Lấy character theo index
    /// </summary>
    public PlayerCharacter GetCharacter(int index)
    {
        if (index >= 0 && index < allCharacters.Count)
            return allCharacters[index];
        
        Debug.LogWarning($"[CharacterRepository] Index {index} ngoài phạm vi!");
        return null;
    }

    /// <summary>
    /// Lấy số lượng characters
    /// </summary>
    public int GetCharacterCount()
    {
        return allCharacters.Count;
    }

    /// <summary>
    /// Kiểm tra repository đã được khởi tạo chưa
    /// </summary>
    public bool IsInitialized()
    {
        return allCharacters != null && allCharacters.Count > 0;
    }

    /// <summary>
    /// Clear toàn bộ dữ liệu (dùng khi reset game)
    /// </summary>
    public void ClearAll()
    {
        allCharacters.Clear();
        Debug.Log("[CharacterRepository] Đã xóa toàn bộ characters");
    }
}
