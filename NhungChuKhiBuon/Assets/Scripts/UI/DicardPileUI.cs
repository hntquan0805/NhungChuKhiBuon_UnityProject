using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DiscardPileUI : MonoBehaviour
{
    [Header("References")]
    public TeamDeckManager deckManager;
    public TextMeshProUGUI countText; // Text hiển thị số lượng bài trong DiscardPile
    public TextMeshProUGUI addedText; // Text hiển thị "+N" khi thêm bài

    [Header("Animation Settings")]
    public float popUpDuration = 0.5f; // Thời gian animation +N
    public float moveUpDistance = 50f; // Khoảng cách di chuyển lên
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private int lastCount = 0;
    private Coroutine currentAnimation;

    private void Start()
    {
        if (addedText != null)
        {
            addedText.gameObject.SetActive(false);
        }

        UpdateDisplay();
    }

    private void Update()
    {
        // Kiểm tra nếu số lượng DiscardPile thay đổi
        if (deckManager != null)
        {
            int currentCount = deckManager.GetDiscardPileCount();

            if (currentCount != lastCount)
            {
                int added = currentCount - lastCount;

                if (added > 0)
                {
                    ShowAddedAnimation(added);
                }

                lastCount = currentCount;
                UpdateDisplay();
            }
        }
    }

    void UpdateDisplay()
    {
        if (deckManager != null && countText != null)
        {
            int count = deckManager.GetDiscardPileCount();
            countText.text = count.ToString();
        }
    }

    void ShowAddedAnimation(int amount)
    {
        if (addedText == null) return;

        // Dừng animation cũ nếu đang chạy
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }

        currentAnimation = StartCoroutine(AnimateAddedText(amount));
    }

    IEnumerator AnimateAddedText(int amount)
    {
        addedText.text = "+" + amount;
        addedText.gameObject.SetActive(true);

        Vector3 startPos = addedText.transform.localPosition;
        Vector3 endPos = startPos + Vector3.up * moveUpDistance;

        Color startColor = addedText.color;
        startColor.a = 1f;

        float elapsed = 0f;

        while (elapsed < popUpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / popUpDuration;

            // Di chuyển lên
            addedText.transform.localPosition = Vector3.Lerp(startPos, endPos, t);

            // Scale animation
            float scale = scaleCurve.Evaluate(t);
            addedText.transform.localScale = Vector3.one * Mathf.Lerp(0.5f, 1.2f, scale);

            // Fade out
            Color color = startColor;
            color.a = fadeCurve.Evaluate(t);
            addedText.color = color;

            yield return null;
        }

        // Reset về vị trí ban đầu
        addedText.transform.localPosition = startPos;
        addedText.transform.localScale = Vector3.one;
        addedText.gameObject.SetActive(false);

        currentAnimation = null;
    }
}