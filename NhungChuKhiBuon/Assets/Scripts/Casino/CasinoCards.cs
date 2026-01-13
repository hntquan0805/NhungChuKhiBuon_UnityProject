using UnityEngine;
using System.Collections;

public enum CasinoCardType
{
    Type1 = 0,
    Type2 = 1,
    Type3 = 2,
    Type4 = 3,
    Type5 = 4,
    Type6 = 5
}

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class CasinoCards : MonoBehaviour
{
    [Header("Card Data")]
    public CasinoCardType cardType;
    public bool isRevealed = false;

    [Header("Sprites")]
    public Sprite cardFrontSprite;
    public Sprite cardBackSprite;

    [Header("Flip Settings")]
    public float flipDuration = 0.25f;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private bool isFlipping = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    /// <summary>
    /// Khởi tạo lá bài – LUÔN ÚP
    /// </summary>
    public void Initialize(CasinoCardType type, Sprite frontSprite, Sprite backSprite)
    {
        cardType = type;
        cardFrontSprite = frontSprite;
        cardBackSprite = backSprite;

        isRevealed = false;
        isFlipping = false;

        spriteRenderer.sprite = cardBackSprite;
        spriteRenderer.color = Color.white;

        transform.localScale = new Vector3(0.75f, 0.75f, 1f);

        if (boxCollider != null)
            boxCollider.enabled = true;
    }

    /* =========================
       FLIP LOGIC
       ========================= */

    public void Reveal()
    {
        if (isRevealed || isFlipping) return;

        // Phát âm thanh lật bài
        if (AudioCasinoManager.Instance != null)
        {
            AudioCasinoManager.Instance.PlayCardFlip();
        }

        StartCoroutine(FlipCoroutine());
    }

    IEnumerator FlipCoroutine()
    {
        isFlipping = true;
        boxCollider.enabled = false;

        Vector3 originalScale = transform.localScale;
        float time = 0f;

        // Thu nhỏ trục X
        while (time < flipDuration)
        {
            float scaleX = Mathf.Lerp(1f, 0f, time / flipDuration);
            transform.localScale = new Vector3(scaleX, originalScale.y, originalScale.z);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = new Vector3(0f, originalScale.y, originalScale.z);

        // Đổi sprite sang mặt trước
        spriteRenderer.sprite = cardFrontSprite;
        isRevealed = true;

        // Mở lại
        time = 0f;
        while (time < flipDuration)
        {
            float scaleX = Mathf.Lerp(0f, 1f, time / flipDuration);
            transform.localScale = new Vector3(scaleX, originalScale.y, originalScale.z);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;

        isFlipping = false;
    }

    /* =========================
       INTERACTION
       ========================= */

    void OnMouseDown()
    {
        if (isFlipping || isRevealed) return;

        CasinoMangaer manager = FindFirstObjectByType<CasinoMangaer>();
        if (manager != null)
        {
            manager.OnCardClicked(this);
        }
    }
}