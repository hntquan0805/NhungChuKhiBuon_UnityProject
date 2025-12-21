using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardUI : MonoBehaviour
{
    public Button button;
    public Image artworkImage;

    private CardData cardData;
    private PlayerCharacter ownerPlayer;
    private HandController handController;
    private bool isQueued = false;

    public void Setup(CardData data, PlayerCharacter owner, HandController controller)
    {
        cardData = data;
        ownerPlayer = owner;
        handController = controller;

        if (artworkImage != null)
        {
            artworkImage.sprite = cardData.artwork;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        // Kiểm tra điều kiện play card
        if (!BattleManager.Instance.CanPlayCard())
        {
            Debug.Log("Cannot play card - not player turn or out of AP");
            return;
        }

        // Kiểm tra đã được thêm vào queue chưa
        if (isQueued)
        {
            Debug.Log("Card already queued");
            return;
        }

        // Nếu là Attack card → Phải có target hợp lệ
        if (cardData.type == CardType.Attack)
        {
            EnemyCharacter targetEnemy = TargetSelector.Instance.GetCurrentSelectedEnemy();

            // Kiểm tra target có hợp lệ không
            if (targetEnemy == null || targetEnemy.GetCurrentHP() <= 0)
            {
                // Thử lấy enemy đầu tiên còn sống
                targetEnemy = BattleManager.Instance.GetFirstAliveEnemy();

                if (targetEnemy == null)
                {
                    Debug.LogError("No valid enemy target available!");
                    return;
                }

                // Tự động select enemy này
                TargetSelector.Instance.SelectEnemy(targetEnemy);
            }

            Debug.Log($"Attack card targeting: {targetEnemy.gameObject.name}");
            EnqueueCard(targetEnemy);
        }
        else
        {
            // Các card khác không cần target
            EnqueueCard(null);
        }
    }

    void EnqueueCard(EnemyCharacter targetEnemy)
    {
        // Đánh dấu card đã queued
        isQueued = true;

        // Thêm vào queue
        if (CardActionQueue.Instance != null)
        {
            CardActionQueue.Instance.EnqueueCardAction(cardData, ownerPlayer, this, targetEnemy);

            // Xóa khỏi danh sách hand của controller
            handController.RemoveCardFromHand(this);

            // Visual feedback
            if (button != null)
            {
                button.interactable = false;
            }

            // Làm mờ card
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 0.5f;
        }
    }

    public CardData GetCardData()
    {
        return cardData;
    }

    public bool IsQueued()
    {
        return isQueued;
    }
}