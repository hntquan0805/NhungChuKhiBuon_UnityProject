using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffIcon : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI stackText;

    private BuffInstance buff;

    public void Initialize(BuffInstance buffInstance)
    {
        buff = buffInstance;
        
        if (iconImage != null && buff.icon != null)
        {
            iconImage.sprite = buff.icon;
        }

        UpdateStackText();
    }

    public void UpdateStackText()
    {
        if (stackText != null && buff != null)
        {
            stackText.text = buff.stacks > 1 ? buff.stacks.ToString() : "";
        }
    }

    public BuffType GetBuffType()
    {
        return buff?.type ?? BuffType.IncreaseAttack;
    }

    public int GetStacks()
    {
        return buff?.stacks ?? 0;
    }
}
