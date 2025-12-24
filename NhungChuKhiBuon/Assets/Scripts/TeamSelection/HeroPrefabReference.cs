using UnityEngine;

// Component này attach vào HeroAvatar để lưu prefab reference
public class HeroPrefabReference : MonoBehaviour
{
    [Header("Hero Prefab")]
    [Tooltip("Kéo PlayerCharacter prefab tương ứng vào đây")]
    public GameObject prefab;
}