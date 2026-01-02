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
            return;
        }

        // Kiểm tra đã được thêm vào queue chưa
        if (isQueued)
        {
            return;
        }

        // Nếu là Attack hoặc Cast card → Phải có target hợp lệ
        if (cardData.type == CardType.Attack || cardData.type == CardType.Cast)
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

            EnqueueCard(targetEnemy);
        }
        else
        {
            // Các card khác không cần target (Heal, Shield)
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