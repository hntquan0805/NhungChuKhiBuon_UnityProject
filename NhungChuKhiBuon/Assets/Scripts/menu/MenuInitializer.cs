using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Script khởi tạo characters khi vào Menu scene.
/// Attach script này vào một GameObject trong Menu scene.
/// </summary>
public class MenuInitializer : MonoBehaviour
{
    [Header("Character Database")]
    [Tooltip("Danh sách tất cả PlayerCharacter prefabs trong game")]
    public List<PlayerCharacter> characterPrefabs = new List<PlayerCharacter>();

    void Start()
    {
        InitializeCharacterRepository();
    }

    /// <summary>
    /// Khởi tạo CharacterRepository với danh sách characters
    /// </summary>
    private void InitializeCharacterRepository()
    {
        if (characterPrefabs == null || characterPrefabs.Count == 0)
        {
            Debug.LogError("[MenuInitializer] ⚠ Chưa gán character prefabs! Hãy kéo tất cả character prefabs vào Inspector.");
            return;
        }

        // Khởi tạo CharacterRepository
        CharacterRepository.Instance.InitializeCharacters(characterPrefabs);
        
        Debug.Log($"[MenuInitializer] ✓ Đã khởi tạo {characterPrefabs.Count} characters vào CharacterRepository");
    }
}
