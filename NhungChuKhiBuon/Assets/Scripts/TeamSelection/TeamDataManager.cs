using UnityEngine;
using System.Collections.Generic;

public class TeamDataManager : MonoBehaviour
{
    public static TeamDataManager Instance;

    [System.Serializable]
    public class SelectedHeroData
    {
        public string heroName;
        public Sprite avatarSprite;
        public Sprite fullBodySprite;
        public GameObject heroPrefab; // Prefab của hero này
    }

    public List<SelectedHeroData> selectedTeam = new List<SelectedHeroData>();

    private void Awake()
    {
        // Singleton pattern với DontDestroyOnLoad
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Lưu team đã chọn
    public void SetSelectedTeam(List<HeroAvatar> heroes)
    {
        selectedTeam.Clear();

        foreach (var hero in heroes)
        {
            SelectedHeroData data = new SelectedHeroData
            {
                heroName = hero.heroName,
                avatarSprite = hero.avatarSprite,
                fullBodySprite = hero.fullBodySprite,
                heroPrefab = hero.GetComponent<HeroPrefabReference>()?.prefab
            };

            selectedTeam.Add(data);
        }

        Debug.Log($"Saved {selectedTeam.Count} heroes to TeamDataManager");
    }

    // Lấy team đã lưu
    public List<SelectedHeroData> GetSelectedTeam()
    {
        return selectedTeam;
    }

    // Clear data (tùy chọn)
    public void ClearTeam()
    {
        selectedTeam.Clear();
    }
}