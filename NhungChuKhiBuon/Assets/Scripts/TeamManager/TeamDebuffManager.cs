using UnityEngine;
using System.Collections.Generic;

public class TeamDebuffManager : MonoBehaviour
{
    [Header("Debuff UI")]
    [SerializeField] private Transform debuffIconContainer;
    [SerializeField] private GameObject debuffIconPrefab;

    private List<GameObject> debuffIcons = new List<GameObject>();
    private PlayerTeam playerTeam;

    private void Awake()
    {
        playerTeam = GetComponent<PlayerTeam>();
    }

    public void UpdateDebuffUI()
    {
        ClearDebuffIcons();

        if (debuffIconContainer == null || debuffIconPrefab == null || playerTeam == null)
        {
            return;
        }

        // ===== Thu thập debuff theo logic mới =====
        List<DebuffDisplayInfo> debuffsToDisplay = new List<DebuffDisplayInfo>();

        foreach (var player in playerTeam.players)
        {
            if (player == null) continue;

            List<DebuffInstance> playerDebuffs = player.GetDebuffs();

            foreach (var debuff in playerDebuffs)
            {
                // ===== POISON: Hiển thị riêng cho TỪNG PLAYER =====
                if (debuff.type == DebuffType.Poison)
                {
                    debuffsToDisplay.Add(new DebuffDisplayInfo
                    {
                        debuffInstance = debuff,
                        playerName = player.name,  // Đánh dấu là debuff riêng
                        displayStacks = debuff.stacks
                    });
                }
                // ===== DEBUFF KHÁC: Gộp chung tất cả players =====
                else
                {
                    // Tìm xem đã có debuff loại này chưa (không phân biệt player)
                    var existing = debuffsToDisplay.Find(d =>
                        d.debuffInstance.type == debuff.type &&
                        d.playerName == null  // null = debuff gộp chung
                    );

                    if (existing != null)
                    {
                        // Đã có -> cộng dồn stack
                        existing.displayStacks += debuff.stacks;
                    }
                    else
                    {
                        // Chưa có -> tạo mới
                        debuffsToDisplay.Add(new DebuffDisplayInfo
                        {
                            debuffInstance = debuff,
                            playerName = null,  // null = gộp chung
                            displayStacks = debuff.stacks
                        });
                    }
                }
            }
        }

        // ===== Tạo UI cho tất cả debuff =====
        foreach (var displayInfo in debuffsToDisplay)
        {
            GameObject iconObj = Instantiate(debuffIconPrefab, debuffIconContainer);
            iconObj.transform.SetParent(debuffIconContainer, false);

            RectTransform rectTransform = iconObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.localScale = Vector3.one;
                rectTransform.localPosition = Vector3.zero;
            }

            DebuffIcon icon = iconObj.GetComponent<DebuffIcon>();
            if (icon != null)
            {
                // Tạo instance tạm để hiển thị với số stack đúng
                DebuffInstance tempDisplay;

                if (displayInfo.debuffInstance.source is PlayerCharacter playerSource)
                {
                    tempDisplay = new DebuffInstance(
                        displayInfo.debuffInstance.type,
                        displayInfo.displayStacks,  // Dùng displayStacks đã tính
                        playerSource,
                        displayInfo.debuffInstance.icon
                    );
                }
                else if (displayInfo.debuffInstance.source is EnemyCharacter enemySource)
                {
                    tempDisplay = new DebuffInstance(
                        displayInfo.debuffInstance.type,
                        displayInfo.displayStacks,
                        enemySource,
                        displayInfo.debuffInstance.icon
                    );
                }
                else
                {
                    continue; // Skip nếu source không hợp lệ
                }

                icon.Initialize(tempDisplay);
            }

            debuffIcons.Add(iconObj);
        }
    }

    private void ClearDebuffIcons()
    {
        foreach (var icon in debuffIcons)
        {
            if (icon != null)
                Destroy(icon);
        }
        debuffIcons.Clear();
    }
}

// Helper class để quản lý thông tin hiển thị debuff
public class DebuffDisplayInfo
{
    public DebuffInstance debuffInstance;  // Instance gốc
    public string playerName;              // null = gộp chung, có tên = riêng player đó
    public int displayStacks;              // Số stack sẽ hiển thị trên UI
}