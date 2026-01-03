using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FinishTurnButton : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] private Button finishButton;
    
    [Header("AP Warning")]
    [SerializeField] private RectTransform apTextTransform;
    [SerializeField] private float shakeStrength = 20f;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float scaleAmount = 1.3f;
    
    private bool isTurnEnding = false;

    private void Awake()
    {
        if (finishButton == null)
            finishButton = GetComponent<Button>();
            
        if (finishButton != null)
            finishButton.onClick.AddListener(OnFinishTurnClicked);
    }

    private void OnDestroy()
    {
        if (finishButton != null)
            finishButton.onClick.RemoveListener(OnFinishTurnClicked);
    }

    public void OnFinishTurnClicked()
    {
        if (isTurnEnding) return;
        
        if (BattleManager.Instance == null) return;
        
        // Kiểm tra nếu còn AP - KHÔNG CHO kết thúc lượt
        if (BattleManager.Instance.context.playerAP > 0)
        {
            PlayAPWarningAnimation();
            return; // DỬNG LẠI, không kết thúc lượt
        }
        
        isTurnEnding = true;
        
        // Kết thúc turn
        BattleManager.Instance.FinishTurn();
    }
    
    private void PlayAPWarningAnimation()
    {
        if (apTextTransform == null)
        {
            Debug.LogError("[FINISH BUTTON] AP Text Transform is NULL! Please assign it in Inspector.");
            return;
        }
        
        // Kill tất cả animation đang chạy trên AP text
        apTextTransform.DOKill();
        
        // Reset về trạng thái ban đầu
        apTextTransform.localScale = Vector3.one;
        
        // Tạo sequence animation
        Sequence seq = DOTween.Sequence();
        
        // Scale up + Shake đồng thời
        seq.Append(apTextTransform.DOScale(scaleAmount, shakeDuration * 0.3f).SetEase(Ease.OutQuad));
        seq.Join(apTextTransform.DOShakePosition(shakeDuration, shakeStrength, 10, 90, false, true));
        
        // Scale down về bình thường với easing mượt
        seq.Append(apTextTransform.DOScale(1f, shakeDuration * 0.3f).SetEase(Ease.InQuad));
    }
    
    public void ResetButton()
    {
        isTurnEnding = false;
    }
}
