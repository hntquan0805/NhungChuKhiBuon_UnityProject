using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Component tự động tạo HeroAvatar UI từ CharacterRepository
/// Attach vào parent object của các HeroAvatar trong TeamSelection scene
/// Lấy avatar sprites trực tiếp từ PlayerStats.characterIcon của mỗi character prefab
/// </summary>
public class HeroAvatarGenerator : MonoBehaviour
{
    [Header("Prefab")]
    [Tooltip("Prefab của HeroAvatar (phải có component HeroAvatar, Image, Button)")]
    public GameObject heroAvatarPrefab;

    [Header("Container")]
    [Tooltip("(Tùy chọn) Content transform của ScrollView để spawn avatars. Nếu không set sẽ dùng transform của object này")]
    public Transform contentContainer;

    [Header("Manager Reference")]
    [Tooltip("Reference đến TeamSelectionManager để khởi tạo các hero avatars")]
    public TeamSelectionManager teamSelectionManager;

    void Start()
    {
        GenerateHeroAvatars();
    }

    private void GenerateHeroAvatars()
    {
        // Kiểm tra CharacterRepository đã được khởi tạo chưa
        if (!CharacterRepository.Instance.IsInitialized())
        {
            Debug.LogError("[HeroAvatarGenerator] ⚠ CharacterRepository chưa được khởi tạo! Hãy chắc chắn đã chạy qua Menu scene trước.");
            return;
        }

        if (heroAvatarPrefab == null)
        {
            Debug.LogError("[HeroAvatarGenerator] ⚠ Chưa gán HeroAvatar prefab!");
            return;
        }

        // Xác định container để spawn (ưu tiên contentContainer, nếu không có thì dùng transform hiện tại)
        Transform spawnParent = contentContainer != null ? contentContainer : transform;

        // Xóa các hero avatars cũ nếu có
        ClearExistingAvatars();

        List<PlayerCharacter> characters = CharacterRepository.Instance.GetAllCharacters();
        
        for (int i = 0; i < characters.Count; i++)
        {
            PlayerCharacter character = characters[i];
            if (character == null) continue;

            // Tạo HeroAvatar GameObject
            GameObject avatarGO = Instantiate(heroAvatarPrefab, spawnParent);
            HeroAvatar heroAvatar = avatarGO.GetComponent<HeroAvatar>();

            if (heroAvatar != null)
            {
                // Set thông tin hero
                heroAvatar.heroName = character.stats.characterName;
                
                // Lấy avatar sprite từ PlayerStats.avatarSprite (hiển thị trong khu vực chọn)
                Sprite avatar = character.stats.avatarSprite;
                
                if (avatar != null)
                {
                    heroAvatar.avatarSprite = avatar;
                    
                    // Set sprite cho Image component
                    Image image = avatarGO.GetComponent<Image>();
                    if (image != null)
                    {
                        image.sprite = avatar;
                    }
                }
                else
                {
                    Debug.LogWarning($"[HeroAvatarGenerator] Character '{character.stats.characterName}' không có avatarSprite! Hãy kéo sprite avatar vào PlayerStats.avatarSprite trong prefab.");
                }
                
                // Lấy full body sprite từ PlayerStats.characterIcon (hiển thị trong slot đã chọn)
                Sprite fullBody = character.stats.characterIcon;
                
                if (fullBody != null)
                {
                    heroAvatar.fullBodySprite = fullBody;
                }
                else if (avatar != null)
                {
                    // Fallback: nếu không có characterIcon thì dùng avatarSprite
                    heroAvatar.fullBodySprite = avatar;
                    Debug.LogWarning($"[HeroAvatarGenerator] Character '{character.stats.characterName}' không có characterIcon! Đang dùng avatarSprite thay thế.");
                }

                // Lưu reference đến character prefab gốc
                HeroPrefabReference prefabRef = avatarGO.GetComponent<HeroPrefabReference>();
                if (prefabRef == null)
                {
                    prefabRef = avatarGO.AddComponent<HeroPrefabReference>();
                }
                prefabRef.prefab = character.gameObject;

                // QUAN TRỌNG: Khởi tạo HeroAvatar với TeamSelectionManager
                if (teamSelectionManager != null)
                {
                    heroAvatar.Initialize(teamSelectionManager);
                }
                else
                {
                    Debug.LogWarning("[HeroAvatarGenerator] TeamSelectionManager chưa được gán! Hero avatar sẽ không hoạt động.");
                }
            }

            avatarGO.name = $"HeroAvatar_{character.stats.characterName}";
        }

        Debug.Log($"[HeroAvatarGenerator] ✓ Đã tạo {characters.Count} hero avatars");
    }

    private void ClearExistingAvatars()
    {
        // Xác định container để xóa
        Transform targetParent = contentContainer != null ? contentContainer : transform;
        
        // Xóa tất cả child objects
        for (int i = targetParent.childCount - 1; i >= 0; i--)
        {
            Destroy(targetParent.GetChild(i).gameObject);
        }
    }
}
