using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Quản lý phần thưởng sau khi chiến thắng battle
/// Attach script này vào GameObject trong Victory Scene
/// </summary>
public class VictoryRewardManager : MonoBehaviour
{
    [Header("Reward Settings")]
    [Tooltip("Số vàng nhận được sau khi thắng")]
    public int goldReward = 500;

    [Tooltip("Số EXP nhận được sau khi thắng")]
    public int expReward = 300;

    [Header("Level-based Multiplier")]
    [Tooltip("Nhân thưởng theo level đã chọn")]
    public bool useMultiplier = true;
    public float level1Multiplier = 1.0f;
    public float level2Multiplier = 1.5f;
    public float level3Multiplier = 2.0f;

    [Header("UI References")]
    [Tooltip("Icon hiển thị cho Gold reward")]
    public Image goldRewardIcon;
    public TextMeshProUGUI goldRewardText;

    [Tooltip("Icon hiển thị cho EXP reward")]
    public Image expRewardIcon;
    public TextMeshProUGUI expRewardText;

    public TextMeshProUGUI totalGoldText;
    public TextMeshProUGUI totalExpText;
    public Button continueButton;

    [Header("Animation Settings")]
    public bool animateRewards = true;
    public float animationDuration = 1.5f;

    [Tooltip("Animate icon scale when showing rewards")]
    public bool animateIcons = true;
    public float iconScaleMultiplier = 1.2f;

    private int finalGoldReward;
    private int finalExpReward;
    private bool rewardsGranted = false;

    void Start()
    {
        CalculateRewards();

        if (animateRewards)
        {
            StartCoroutine(AnimateRewardGrant());
        }
        else
        {
            GrantRewardsImmediately();
        }

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueClicked);
        }
    }

    /// <summary>
    /// Tính toán phần thưởng dựa trên level
    /// </summary>
    void CalculateRewards()
    {
        finalGoldReward = goldReward;
        finalExpReward = expReward;

        if (useMultiplier)
        {
            int selectedLevel = LevelSelector.GetSelectedLevel();
            float multiplier = 1.0f;

            switch (selectedLevel)
            {
                case 1:
                    multiplier = level1Multiplier;
                    break;
                case 2:
                    multiplier = level2Multiplier;
                    break;
                case 3:
                    multiplier = level3Multiplier;
                    break;
            }

            finalGoldReward = Mathf.RoundToInt(goldReward * multiplier);
            finalExpReward = Mathf.RoundToInt(expReward * multiplier);

            Debug.Log($"[VictoryReward] Level {selectedLevel} - Multiplier: {multiplier}x");
        }

        Debug.Log($"[VictoryReward] Gold: {finalGoldReward}, EXP: {finalExpReward}");
    }

    /// <summary>
    /// Cộng thưởng ngay lập tức (không animation)
    /// </summary>
    void GrantRewardsImmediately()
    {
        if (rewardsGranted) return;

        // Cộng vào PlayerResourceManager
        PlayerResourceManager.Instance.AddGold(finalGoldReward);
        PlayerResourceManager.Instance.AddExp(finalExpReward);

        // Update UI
        UpdateRewardUI();
        UpdateTotalUI();

        rewardsGranted = true;
        Debug.Log($"[VictoryReward] ✓ Đã cộng {finalGoldReward} Gold và {finalExpReward} EXP");
    }

    /// <summary>
    /// Animation hiển thị phần thưởng dần dần
    /// </summary>
    System.Collections.IEnumerator AnimateRewardGrant()
    {
        if (rewardsGranted) yield break;

        float elapsed = 0f;
        int currentGold = 0;
        int currentExp = 0;

        int startGold = PlayerResourceManager.Instance.CurrentGold;
        int startExp = PlayerResourceManager.Instance.CurrentExp;

        // Icon animation setup
        Vector3 goldIconOriginalScale = Vector3.one;
        Vector3 expIconOriginalScale = Vector3.one;

        if (animateIcons)
        {
            if (goldRewardIcon != null)
                goldIconOriginalScale = goldRewardIcon.transform.localScale;
            if (expRewardIcon != null)
                expIconOriginalScale = expRewardIcon.transform.localScale;
        }

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / animationDuration;

            // Animate counting up
            currentGold = Mathf.RoundToInt(Mathf.Lerp(0, finalGoldReward, progress));
            currentExp = Mathf.RoundToInt(Mathf.Lerp(0, finalExpReward, progress));

            // Update reward display
            if (goldRewardText != null)
                goldRewardText.text = $"+{currentGold}";

            if (expRewardText != null)
                expRewardText.text = $"+{currentExp}";

            // Update total display
            if (totalGoldText != null)
                totalGoldText.text = $"{startGold + currentGold}";

            if (totalExpText != null)
                totalExpText.text = $"{startExp + currentExp}";

            // Animate icons (pulse effect)
            if (animateIcons)
            {
                float scaleProgress = Mathf.Sin(progress * Mathf.PI); // 0 -> 1 -> 0
                float currentScale = 1f + (scaleProgress * (iconScaleMultiplier - 1f));

                if (goldRewardIcon != null)
                    goldRewardIcon.transform.localScale = goldIconOriginalScale * currentScale;

                if (expRewardIcon != null)
                    expRewardIcon.transform.localScale = expIconOriginalScale * currentScale;
            }

            yield return null;
        }

        // Reset icon scales
        if (animateIcons)
        {
            if (goldRewardIcon != null)
                goldRewardIcon.transform.localScale = goldIconOriginalScale;
            if (expRewardIcon != null)
                expRewardIcon.transform.localScale = expIconOriginalScale;
        }

        // Ensure final values
        currentGold = finalGoldReward;
        currentExp = finalExpReward;

        // Grant actual rewards
        PlayerResourceManager.Instance.AddGold(finalGoldReward);
        PlayerResourceManager.Instance.AddExp(finalExpReward);

        // Final UI update
        UpdateRewardUI();
        UpdateTotalUI();

        rewardsGranted = true;
        Debug.Log($"[VictoryReward] ✓ Animation complete. Granted {finalGoldReward} Gold và {finalExpReward} EXP");
    }

    /// <summary>
    /// Cập nhật UI hiển thị phần thưởng
    /// </summary>
    void UpdateRewardUI()
    {
        if (goldRewardText != null)
            goldRewardText.text = $"{finalGoldReward}";

        if (expRewardText != null)
            expRewardText.text = $"{finalExpReward}";
    }

    /// <summary>
    /// Cập nhật UI hiển thị tổng tài nguyên
    /// </summary>
    void UpdateTotalUI()
    {
        if (totalGoldText != null)
            totalGoldText.text = $"{PlayerResourceManager.Instance.CurrentGold}";

        if (totalExpText != null)
            totalExpText.text = $"{PlayerResourceManager.Instance.CurrentExp}";
    }

    /// <summary>
    /// Xử lý khi nhấn nút Continue
    /// </summary>
    void OnContinueClicked()
    {
        // Đảm bảo rewards đã được cấp (nếu user spam click)
        if (!rewardsGranted)
        {
            GrantRewardsImmediately();
        }

        // Quay về map hoặc menu
        ReturnToMap();
    }

    /// <summary>
    /// Quay về Map scene tương ứng với level đã chọn
    /// </summary>
    void ReturnToMap()
    {
        int selectedLevel = LevelSelector.GetSelectedLevel();
        string targetScene = GetMapSceneByLevel(selectedLevel);
        
        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.LoadScene(targetScene);
        }
        else
        {
            SceneManager.LoadScene(targetScene);
        }
    }

    /// <summary>
    /// Lấy tên scene map theo level
    /// </summary>
    private string GetMapSceneByLevel(int level)
    {
        switch (level)
        {
            case 1:
                return "MapLv1";
            case 2:
                return "MapLv2";
            case 3:
                return "MapLv3";
            default:
                Debug.LogWarning($"⚠ Level không hợp lệ: {level}. Sử dụng MapLv1 mặc định.");
                return "MapLv1";
        }
    }

    /// <summary>
    /// Debug: Thêm thưởng thủ công (gọi từ Inspector hoặc code khác)
    /// </summary>
    [ContextMenu("Grant Rewards Manually")]
    public void GrantRewardsManually()
    {
        GrantRewardsImmediately();
    }
}