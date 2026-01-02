using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class CardHoverEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Hold Settings")]
    public float holdTimeToShow = 0.3f;

    [Header("Description Panel")]
    public GameObject descriptionPanel;
    public float descriptionDisplayTime = 3f;
    public Vector3 descriptionOffset = new Vector3(0f, 200f, 0f);

    private bool isHolding = false;
    private float holdTimer = 0f;
    private bool isDescriptionShowing = false;
    private CardUI cardUI;
    private TextMeshProUGUI descriptionText;
    private Coroutine hideDescriptionCoroutine;

    private void Awake()
    {
        cardUI = GetComponent<CardUI>();

        if (descriptionPanel != null)
        {
            descriptionPanel.SetActive(false);
        }
    }

    private void Start()
    {
        if (descriptionPanel != null)
        {
            descriptionText = descriptionPanel.GetComponentInChildren<TextMeshProUGUI>();
            descriptionPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (isHolding)
        {
            holdTimer += Time.deltaTime;

            if (!isDescriptionShowing && holdTimer >= holdTimeToShow)
            {
                ShowDescription();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        isHolding = true;
        holdTimer = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        isHolding = false;
        holdTimer = 0f;

        if (isDescriptionShowing)
        {
            HideDescription();
        }
    }

    private void ShowDescription()
    {
        if (descriptionPanel == null || descriptionText == null || cardUI == null)
            return;

        CardData cardData = cardUI.GetCardData();
        if (cardData == null)
            return;

        descriptionText.text = cardData.description;

        // Đặt vị trí panel
        if (descriptionPanel.transform.parent == transform)
        {
            descriptionPanel.transform.localPosition = descriptionOffset;
        }
        else
        {
            RectTransform cardRect = GetComponent<RectTransform>();
            RectTransform panelRect = descriptionPanel.GetComponent<RectTransform>();

            if (cardRect != null && panelRect != null)
            {
                Vector2 cardPos = cardRect.anchoredPosition;
                panelRect.anchoredPosition = cardPos + new Vector2(descriptionOffset.x, descriptionOffset.y);
            }
        }

        descriptionPanel.SetActive(true);
        isDescriptionShowing = true;

        if (hideDescriptionCoroutine != null)
        {
            StopCoroutine(hideDescriptionCoroutine);
        }

        hideDescriptionCoroutine = StartCoroutine(HideDescriptionAfterDelay());
    }

    private void HideDescription()
    {
        if (descriptionPanel != null)
        {
            descriptionPanel.SetActive(false);
            isDescriptionShowing = false;
        }

        if (hideDescriptionCoroutine != null)
        {
            StopCoroutine(hideDescriptionCoroutine);
            hideDescriptionCoroutine = null;
        }
    }

    private IEnumerator HideDescriptionAfterDelay()
    {
        yield return new WaitForSeconds(descriptionDisplayTime);

        if (descriptionPanel != null)
        {
            descriptionPanel.SetActive(false);
            isDescriptionShowing = false;
        }

        hideDescriptionCoroutine = null;
    }

    private void OnDisable()
    {
        if (descriptionPanel != null)
        {
            descriptionPanel.SetActive(false);
        }

        isHolding = false;
        isDescriptionShowing = false;
        holdTimer = 0f;
    }

    private void OnDestroy()
    {
        if (descriptionPanel != null)
        {
            descriptionPanel.SetActive(false);
        }
    }
}