using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebuffIcon : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI stackText;

    private DebuffInstance debuff;

    public void Initialize(DebuffInstance debuffInstance)
    {
        debuff = debuffInstance;
        
        if (iconImage != null && debuff.icon != null)
        {
            iconImage.sprite = debuff.icon;
        }

        UpdateStackText();
    }

    public void UpdateStackText()
    {
        if (stackText != null && debuff != null)
        {
            stackText.text = debuff.stacks > 1 ? debuff.stacks.ToString() : "";
        }
    }

    public DebuffType GetDebuffType()
    {
        return debuff?.type ?? DebuffType.Burn;
    }

    public int GetStacks()
    {
        return debuff?.stacks ?? 0;
    }
}
