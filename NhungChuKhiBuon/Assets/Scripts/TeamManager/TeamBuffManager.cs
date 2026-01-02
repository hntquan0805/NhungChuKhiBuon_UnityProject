using UnityEngine;
using System.Collections.Generic;

public class TeamBuffManager : MonoBehaviour
{
    [Header("Buff UI")]
    [SerializeField] private Transform buffIconContainer;
    [SerializeField] private GameObject buffIconPrefab;

    private PlayerTeam playerTeam;
    private List<GameObject> buffIcons = new List<GameObject>();

    private void Awake()
    {
        playerTeam = GetComponent<PlayerTeam>();
    }

    public void UpdateBuffUI()
    {
        ClearBuffIcons();

        if (buffIconContainer == null || buffIconPrefab == null || playerTeam == null)
            return;

        // Thu thập tất cả buff từ team (không duplicate)
        Dictionary<BuffType, BuffInstance> teamBuffs = new Dictionary<BuffType, BuffInstance>();

        foreach (var player in playerTeam.players)
        {
            if (player != null)
            {
                foreach (var buff in player.GetBuffs())
                {
                    if (!teamBuffs.ContainsKey(buff.type))
                    {
                        teamBuffs[buff.type] = buff;
                    }
                    else
                    {
                        // Lấy buff có stack cao nhất
                        if (buff.stacks > teamBuffs[buff.type].stacks)
                        {
                            teamBuffs[buff.type] = buff;
                        }
                    }
                }
            }
        }

        // Tạo UI cho mỗi buff type
        foreach (var buffPair in teamBuffs)
        {
            GameObject iconObj = Instantiate(buffIconPrefab, buffIconContainer);
            iconObj.transform.SetParent(buffIconContainer, false);

            RectTransform rectTransform = iconObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.localScale = Vector3.one;
                rectTransform.localPosition = Vector3.zero;
            }

            BuffIcon icon = iconObj.GetComponent<BuffIcon>();
            if (icon != null)
                icon.Initialize(buffPair.Value);

            buffIcons.Add(iconObj);
        }
    }

    private void ClearBuffIcons()
    {
        foreach (var icon in buffIcons)
        {
            if (icon != null)
                Destroy(icon);
        }
        buffIcons.Clear();
    }
}
