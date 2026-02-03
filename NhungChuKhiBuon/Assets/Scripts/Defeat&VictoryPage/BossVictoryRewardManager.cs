using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Quản lý phần thưởng sau khi chiến thắng BOSS
/// Attach script này vào GameObject trong BossVictory Scene
/// Boss Battle: Gold x3, EXP x3, KHÔNG có Coin
/// </summary>
public class BossVictoryRewardManager : MonoBehaviour
{
    [Header("Base Reward Settings")]
    [Tooltip("Số vàng cơ bản (sẽ nhân x3 cho boss)")]
    public int baseGoldReward = 500;

    [Tooltip("Số EXP cơ bản (sẽ nhân x3 cho boss)")]
    public int baseExpReward = 300;

    [Header("Boss Multiplier")]
    [Tooltip("Hệ số nhân cho Boss Battle")]
    public float bossMultiplier = 3.0f;

    [Header("Level-based Multiplier")]
    [Tooltip("Nhân thêm theo level đã chọn")]
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
    public float animationDuration = 2.0f; // Lâu hơn một chút cho boss

    [Tooltip("Animate icon scale when showing rewards")]
    public bool animateIcons = true;
    public float iconScaleMultiplier = 1.5f; // Lớn hơn cho boss

    [Header("Special Effects")]
    [Tooltip("Particle effect khi hiển thị rewards (optional)")]
    public ParticleSystem rewardParticles;

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

        // Play particle effect nếu có
        if (rewardParticles != null)
        {
            rewardParticles.Play();
        }
    }

    /// <summary>
    /// Tính toán phần thưởng cho Boss Battle
    /// Base reward x Level Multiplier x Boss Multiplier (3x)
    /// </summary>
    void CalculateRewards()
    {
        // Bắt đầu từ base reward
        finalGoldReward = baseGoldReward;
        finalExpReward = baseExpReward;

        // Apply level multiplier trước
        if (useMultiplier)
        {
            int selectedLevel = LevelSelector.GetSelectedLevel();
            float levelMultiplier = 1.0f;

            switch (selectedLevel)
            {
                case 1:
                    levelMultiplier = level1Multiplier;
                    break;
                case 2:
                    levelMultiplier = level2Multiplier;
                    break;
                case 3:
                    levelMultiplier = level3Multiplier;
                    break;
            }

            finalGoldReward = Mathf.RoundToInt(baseGoldReward * levelMultiplier);
            finalExpReward = Mathf.RoundToInt(baseExpReward * levelMultiplier);

            Debug.Log($"[BossVictory] Level {selectedLevel} - Level Multiplier: {levelMultiplier}x");
        }

        // Apply boss multiplier sau (x3)
        finalGoldReward = Mathf.RoundToInt(finalGoldReward * bossMultiplier);
        finalExpReward = Mathf.RoundToInt(finalExpReward * bossMultiplier);

        Debug.Log($"[BossVictory] 🔥 Boss Multiplier: {bossMultiplier}x");
        Debug.Log($"[BossVictory] 🎁 Final Boss Rewards - Gold: {finalGoldReward}, EXP: {finalExpReward}");
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

        // KHÔNG cộng coin cho boss battle

        // Update UI
        UpdateRewardUI();
        UpdateTotalUI();

        rewardsGranted = true;
        Debug.Log($"[BossVictory] ✓ BOSS DEFEATED! Granted {finalGoldReward} Gold và {finalExpReward} EXP");
    }

    /// <summary>
    /// Animation hiển thị phần thưởng dần dần
    /// </summary>
    System.Collections.IEnumerator AnimateRewardGrant()
    {
        if (rewardsGranted) yield break;

        // Delay nhỏ trước khi bắt đầu animation
        yield return new WaitForSeconds(0.3f);

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

            // Animate counting up với ease-out curve
            float easedProgress = 1 - Mathf.Pow(1 - progress, 3); // Cubic ease-out

            currentGold = Mathf.RoundToInt(Mathf.Lerp(0, finalGoldReward, easedProgress));
            currentExp = Mathf.RoundToInt(Mathf.Lerp(0, finalExpReward, easedProgress));

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

            // Animate icons (pulse effect) - mạnh hơn cho boss
            if (animateIcons)
            {
                float scaleProgress = Mathf.Sin(progress * Mathf.PI * 2); // 2 cycles
                float currentScale = 1f + (Mathf.Abs(scaleProgress) * (iconScaleMultiplier - 1f));

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
        Debug.Log($"[BossVictory] ✓ Animation complete. BOSS DEFEATED! Granted {finalGoldReward} Gold và {finalExpReward} EXP");
    }

    /// <summary>
    /// Cập nhật UI hiển thị phần thưởng
    /// </summary>
    void UpdateRewardUI()
    {
        if (goldRewardText != null)
            goldRewardText.text = $"+{finalGoldReward}";

        if (expRewardText != null)
            expRewardText.text = $"+{finalExpReward}";
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
    /// Boss Battle kết thúc level, quay về Map
    /// </summary>
    void OnContinueClicked()
    {
        // Đảm bảo rewards đã được cấp
        if (!rewardsGranted)
        {
            GrantRewardsImmediately();
        }

        // Quay về map sau khi đánh boss
        ReturnToMap();
    }

    /// <summary>
    /// Quay về Map scene tương ứng với level đã hoàn thành
    /// </summary>
    void ReturnToMap()
    {
        int selectedLevel = LevelSelector.GetSelectedLevel();
        string targetScene = GetMapSceneByLevel(selectedLevel);

        Debug.Log($"[BossVictory] Returning to {targetScene} after defeating boss");

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
    /// Debug: Thêm thưởng thủ công
    /// </summary>
    [ContextMenu("Grant Boss Rewards Manually")]
    public void GrantRewardsManually()
    {
        GrantRewardsImmediately();
    }
}