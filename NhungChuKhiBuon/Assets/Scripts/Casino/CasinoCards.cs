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

public class CasinoCards : MonoBehaviour
{
    [Header("Card Data")]
    public CasinoCardType cardType;
    public bool isRevealed = false;
    public bool isPlayerCard = false;
    public bool isDiscarded = false;
    public bool isSelected = false;

    [Header("Visual")]
    public Color selectedColor = Color.cyan;

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

    public void Initialize(CasinoCardType type, Sprite frontSprite, Sprite backSprite, bool isPlayer)
    {
        cardType = type;
        cardFrontSprite = frontSprite;
        cardBackSprite = backSprite;
        isPlayerCard = isPlayer;

        transform.localScale = new Vector3(0.75f, 0.75f, 1f);

        if (isPlayerCard)
        {
            isRevealed = true;
            spriteRenderer.sprite = cardFrontSprite;
        }
        else
        {
            isRevealed = false;
            spriteRenderer.sprite = cardBackSprite;
        }


        if (boxCollider != null)
        {
            boxCollider.enabled = true;
        }

        UpdateCardColor();
    }

    /* =========================
       FLIP LOGIC
       ========================= */

    public void Reveal()
    {
        if (isRevealed || isFlipping) return;
        StartCoroutine(FlipCoroutine(true));
    }

    public void HideCard()
    {
        if (!isRevealed || isFlipping) return;
        StartCoroutine(FlipCoroutine(false));
    }

    IEnumerator FlipCoroutine(bool reveal)
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

        // Đổi sprite
        if (reveal)
        {
            spriteRenderer.sprite = cardFrontSprite;
            isRevealed = true;
        }
        else
        {
            spriteRenderer.sprite = cardBackSprite;
            isRevealed = false;
        }

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
        boxCollider.enabled = true;
    }

    /* =========================
       INTERACTION & VISUAL
       ========================= */

    public void SetInteractable(bool interactable)
    {
        if (boxCollider != null)
            boxCollider.enabled = interactable;
    }

    public void UpdateCardColor()
    {
        if (isDiscarded)
        {
            spriteRenderer.color = Color.gray;
        }
        else if (isSelected)
        {
            spriteRenderer.color = selectedColor;
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }

    void OnMouseDown()
    {
        if (isFlipping) return;

        if (!isRevealed)
        {
            Debug.Log("Card chưa được mở!");
            return;
        }

        if (isDiscarded)
        {
            Debug.Log("Card đã bị bỏ!");
            return;
        }

        CasinoMangaer gameManager = FindFirstObjectByType<CasinoMangaer>();
        if (gameManager != null)
        {
            gameManager.OnCardClicked(this);
        }
    }

    void OnMouseEnter()
    {
        if (isRevealed && !isDiscarded && !isSelected && !isFlipping)
        {
            spriteRenderer.color = new Color(0.9f, 0.9f, 0.9f);
        }
    }

    void OnMouseExit()
    {
        if (!isSelected)
        {
            UpdateCardColor();
        }
    }
}
