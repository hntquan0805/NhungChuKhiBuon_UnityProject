using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterListItem : MonoBehaviour
{
    [Header("UI References")]
    public Image characterIcon;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI levelText;
    public Button selectButton;

    private PlayerCharacter characterData;
    private int characterIndex;

    public void Setup(PlayerCharacter character, int index)
    {
        characterData = character;
        characterIndex = index;

        if (characterName != null)
            characterName.text = string.IsNullOrEmpty(character.stats.characterName) ? character.name : character.stats.characterName;

        if (levelText != null)
            levelText.text = $"{character.stats.levelData.currentLevel}";

        if (characterIcon != null && character.stats.characterIcon != null)
            characterIcon.sprite = character.stats.characterIcon;

        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnSelectCharacter);
        }
    }

    private void OnSelectCharacter()
    {
        PlayerPrefs.SetInt("SelectedCharacterIndex", characterIndex);
        PlayerPrefs.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene("DetailCharacter");
    }
}
