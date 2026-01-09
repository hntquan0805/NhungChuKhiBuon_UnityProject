using System;
using UnityEngine;
using UnityEngine.UI;

public class TargetSelector : MonoBehaviour
{
    public static TargetSelector Instance;

    private EnemyCharacter currentSelectedEnemy;

    [Header("UI Feedback")]
    public GameObject targetingIndicator; // Target image GameObject

    [Header("Position Settings")]
    public bool followEnemy = true; // Có theo enemy không
    public Vector3 positionOffset = new Vector3(0, 2f, 0); // Offset từ vị trí enemy

    [Header("Blinking Animation")]
    public bool enableBlinking = true;
    public int blinkCount = 3; // Số lần nhấp nháy
    public float blinkSpeed = 0.3f; // Thời gian mỗi lần blink (giây)
    public float minAlpha = 0.3f; // Độ trong suốt tối thiểu
    public float maxAlpha = 1f; // Độ trong suốt tối đa
    public bool useSimpleBlink = false; // Dùng blink đơn giản (on/off) thay vì fade

    private Image targetImage;
    private float blinkTimer = 0f;
    private int currentBlinkCount = 0; // Đếm số lần đã nhấp nháy
    private bool isBlinking = false;
    private bool isBlinkVisible = true; // Cho simple blink

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Lấy Image component từ targetingIndicator
        if (targetingIndicator != null)
        {
            targetImage = targetingIndicator.GetComponent<Image>();
            if (targetImage == null)
            {
                Debug.LogWarning("TargetingIndicator không có Image component!");
            }
        }

        // Đợi 1 frame để BattleManager khởi tạo xong
        Invoke("InitializeDefaultTarget", 0.1f);
    }

    private void InitializeDefaultTarget()
    {
        if (BattleManager.Instance != null)
        {
            currentSelectedEnemy = BattleManager.Instance.GetFirstAliveEnemy();
            if (currentSelectedEnemy != null)
            {
                UpdateSelectionVisual();
            }
            else
            {
                Debug.LogWarning("No enemies available for default target!");
            }
        }
    }

    public void SelectEnemy(EnemyCharacter enemy)
    {
        if (enemy == null || enemy.GetCurrentHP() <= 0 || enemy.HasStealth())
        {
            return;
        }

        currentSelectedEnemy = enemy;
        UpdateSelectionVisual();
    }

    public EnemyCharacter GetCurrentSelectedEnemy()
    {
        // Kiểm tra enemy hiện tại có còn hợp lệ không (còn sống và không có Stealth)
        if (currentSelectedEnemy == null || currentSelectedEnemy.GetCurrentHP() <= 0 || currentSelectedEnemy.HasStealth())
        {
            // Tìm enemy mới còn sống và không có Stealth
            EnemyCharacter newTarget = BattleManager.Instance?.GetFirstAliveEnemyWithoutStealth();

            if (newTarget != null)
            {
                currentSelectedEnemy = newTarget;
                UpdateSelectionVisual();
            }
            else
            {
                currentSelectedEnemy = null;
            }
        }

        return currentSelectedEnemy;
    }

    private void UpdateSelectionVisual()
    {
        // Clear tất cả highlights cũ
        EnemySelector.ClearAllHighlights();

        // Highlight enemy đang được chọn
        if (currentSelectedEnemy != null)
        {
            EnemySelector[] selectors = FindObjectsOfType<EnemySelector>();
            foreach (var selector in selectors)
            {
                if (selector.enemy == currentSelectedEnemy)
                {
                    selector.ShowHighlight(true);
                    break;
                }
            }
        }

        // Di chuyển targeting indicator nếu có
        if (targetingIndicator != null)
        {
            if (currentSelectedEnemy != null)
            {
                targetingIndicator.SetActive(true);

                // Chỉ cập nhật vị trí nếu followEnemy = true
                if (followEnemy)
                {
                    Vector3 offset = positionOffset;
                    
                    // Nếu là BossEnemy thì tăng Y thêm 1.25
                    if (currentSelectedEnemy is BossEnemy)
                    {
                        offset.y += 1f;
                    }
                    
                    targetingIndicator.transform.position = currentSelectedEnemy.transform.position + offset;
                }

                // Reset blink timer và bắt đầu nhấp nháy
                blinkTimer = 0f;
                currentBlinkCount = 0;
                isBlinking = true;

                // Đảm bảo image hiển thị đầy đủ khi bắt đầu
                if (targetImage != null)
                {
                    Color color = targetImage.color;
                    color.a = maxAlpha;
                    targetImage.color = color;
                }
            }
            else
            {
                targetingIndicator.SetActive(false);
            }
        }
    }

    private void Update()
    {
        // Kiểm tra target hiện tại có còn sống không
        if (currentSelectedEnemy != null && currentSelectedEnemy.GetCurrentHP() <= 0)
        {
            GetCurrentSelectedEnemy(); // Tự động chuyển target
        }

        // Cập nhật vị trí target indicator theo enemy (nếu followEnemy = true)
        if (followEnemy && targetingIndicator != null && targetingIndicator.activeSelf && currentSelectedEnemy != null)
        {
            Vector3 offset = positionOffset;
            
            // Nếu là BossEnemy thì tăng Y thêm 1.25
            if (currentSelectedEnemy is BossEnemy)
            {
                offset.y += 1.25f;
            }
            
            targetingIndicator.transform.position = currentSelectedEnemy.transform.position + offset;
        }

        // Animation nhấp nháy
        if (enableBlinking && targetImage != null && targetingIndicator.activeSelf && isBlinking)
        {
            BlinkAnimation();
        }

        // Optional: Dùng phím số để chọn enemy
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectEnemyByIndex(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectEnemyByIndex(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectEnemyByIndex(2);
        }

        // Optional: Dùng Tab để cycle giữa các enemies
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            CycleToNextEnemy();
        }
    }

    private void BlinkAnimation()
    {
        if (useSimpleBlink)
        {
            // Simple blink: bật/tắt
            blinkTimer += Time.deltaTime;

            if (blinkTimer >= blinkSpeed)
            {
                blinkTimer = 0f;
                isBlinkVisible = !isBlinkVisible;

                if (targetImage != null)
                {
                    Color color = targetImage.color;
                    color.a = isBlinkVisible ? maxAlpha : minAlpha;
                    targetImage.color = color;
                }

                // Đếm khi chuyển từ visible -> invisible
                if (!isBlinkVisible)
                {
                    currentBlinkCount++;

                    if (currentBlinkCount >= blinkCount)
                    {
                        isBlinking = false;
                        targetingIndicator.SetActive(false);
                    }
                }
            }
        }
        else
        {
            // Smooth blink: fade in/out
            blinkTimer += Time.deltaTime * blinkSpeed;

            // Tính alpha theo hàm sin (tạo hiệu ứng mượt)
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(blinkTimer * Mathf.PI) + 1f) / 2f);

            // Áp dụng alpha
            Color color = targetImage.color;
            color.a = alpha;
            targetImage.color = color;

            // Kiểm tra đã hoàn thành 1 chu kỳ nhấp nháy chưa (sin từ 0 -> 2π)
            if (blinkTimer >= 2f)
            {
                currentBlinkCount++;
                blinkTimer = 0f;

                // Nếu đã nhấp nháy đủ số lần, tắt hẳn
                if (currentBlinkCount >= blinkCount)
                {
                    isBlinking = false;
                    targetingIndicator.SetActive(false);
                }
            }
        }
    }

    private void SelectEnemyByIndex(int index)
    {
        if (BattleManager.Instance != null &&
            BattleManager.Instance.enemies != null &&
            index < BattleManager.Instance.enemies.Count)
        {
            EnemyCharacter enemy = BattleManager.Instance.enemies[index];
            if (enemy != null && enemy.GetCurrentHP() > 0 && !enemy.HasStealth())
            {
                SelectEnemy(enemy);
            }
            else
            {
                Debug.Log($"Enemy at index {index} is not available");
            }
        }
    }

    private void CycleToNextEnemy()
    {
        if (BattleManager.Instance == null || BattleManager.Instance.enemies.Count == 0)
        {
            return;
        }

        int currentIndex = BattleManager.Instance.enemies.IndexOf(currentSelectedEnemy);
        if (currentIndex < 0) currentIndex = 0;

        // Tìm enemy tiếp theo còn sống và không có Stealth
        for (int i = 1; i <= BattleManager.Instance.enemies.Count; i++)
        {
            int checkIndex = (currentIndex + i) % BattleManager.Instance.enemies.Count;
            EnemyCharacter enemy = BattleManager.Instance.enemies[checkIndex];

            if (enemy != null && enemy.GetCurrentHP() > 0 && !enemy.HasStealth())
            {
                SelectEnemy(enemy);
                return;
            }
        }
    }

    // Public method để force refresh target visual
    public void RefreshTargetVisual()
    {
        UpdateSelectionVisual();
    }
}