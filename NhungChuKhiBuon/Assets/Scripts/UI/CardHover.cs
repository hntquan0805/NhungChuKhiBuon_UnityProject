using UnityEngine;
using UnityEngine.EventSystems;

public class CardHoverEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Scale Settings")]
    public Vector3 normalScale = Vector3.one;
    public Vector3 enlargedScale = Vector3.one * 1.5f;
    public float scaleDuration = 0.2f;
    public float holdTimeToEnlarge = 1f;

    private bool isHolding = false;
    private float holdTimer = 0f;
    private bool isEnlarged = false;

    private void Update()
    {
        if (isHolding)
        {
            holdTimer += Time.deltaTime;
            if (!isEnlarged && holdTimer >= holdTimeToEnlarge)
            {
                StopAllCoroutines();
                StartCoroutine(ScaleTo(enlargedScale));
                isEnlarged = true;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
        holdTimer = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
        holdTimer = 0f;
        if (isEnlarged)
        {
            StopAllCoroutines();
            StartCoroutine(ScaleTo(normalScale));
            isEnlarged = false;
        }
    }

    private System.Collections.IEnumerator ScaleTo(Vector3 target)
    {
        Vector3 start = transform.localScale;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / scaleDuration;
            transform.localScale = Vector3.Lerp(start, target, t);
            yield return null;
        }

        transform.localScale = target;
    }
}
