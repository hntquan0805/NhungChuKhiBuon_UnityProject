using UnityEngine;
using UnityEngine.UI;

public class SlotController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Kéo Image component sẽ hiển thị avatar hero vào đây")]
    public Image heroImage;

    private Sprite defaultSprite;

    private void Awake()
    {
        if (heroImage != null)
        {
            defaultSprite = heroImage.sprite;
        }
    }

    // Hiển thị hero trong slot
    public void SetHero(Sprite heroSprite)
    {
        if (heroImage != null)
        {
            heroImage.sprite = heroSprite;
            heroImage.enabled = true;
            heroImage.color = Color.white;
        }
    }

    // Xóa hero khỏi slot
    public void ClearSlot()
    {
        if (heroImage != null)
        {
            heroImage.sprite = defaultSprite;
            heroImage.enabled = false;
        }
    }
}