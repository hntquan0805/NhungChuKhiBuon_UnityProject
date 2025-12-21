using UnityEngine;
using UnityEngine.EventSystems;

public class EnemySelector : MonoBehaviour, IPointerClickHandler
{
    public EnemyCharacter enemy;

    [Header("Visual Feedback")]
    public GameObject selectionHighlight; // Optional: Visual indicator khi được select

    private static EnemySelector currentlySelected;

    private void Start()
    {
        if (selectionHighlight != null)
        {
            selectionHighlight.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Click vào enemy để chọn làm target
        if (TargetSelector.Instance != null && enemy != null && enemy.GetCurrentHP() > 0)
        {
            TargetSelector.Instance.SelectEnemy(enemy);
        }
    }

    public void ShowHighlight(bool show)
    {
        if (selectionHighlight != null)
        {
            selectionHighlight.SetActive(show);
        }
    }

    public static void ClearAllHighlights()
    {
        EnemySelector[] allSelectors = FindObjectsOfType<EnemySelector>();
        foreach (var selector in allSelectors)
        {
            selector.ShowHighlight(false);
        }
    }
}