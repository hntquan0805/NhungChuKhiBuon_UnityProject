using UnityEngine;
using UnityEngine.UI;

public class HeroAvatar : MonoBehaviour
{
    [Header("Hero Info")]
    public string heroName;

    [Header("Sprites")]
    [Tooltip("Avatar nhỏ hiển thị ở khu vực chọn")]
    public Sprite avatarSprite;

    [Tooltip("Hình full body hiển thị trong slot")]
    public Sprite fullBodySprite;

    [HideInInspector]
    public bool isSelected = false;

    private TeamSelectionManager manager;
    private Image image;
    private Button button;
    private Color originalColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();

        // Lấy avatar sprite từ Image nếu chưa set
        if (avatarSprite == null && image != null)
        {
            avatarSprite = image.sprite;
        }

        originalColor = image != null ? image.color : Color.white;

        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
    }

    public void Initialize(TeamSelectionManager teamManager)
    {
        manager = teamManager;
    }

    private void OnClick()
    {
        if (manager != null)
        {
            manager.OnHeroClicked(this);
        }
    }

    public void SetSelected(bool selected, Color disabledColor)
    {
        isSelected = selected;

        if (image != null)
        {
            image.color = selected ? disabledColor : originalColor;
        }
    }
}