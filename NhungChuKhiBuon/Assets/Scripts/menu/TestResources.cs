using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script debug để reset Gold và EXP
/// Attach vào GameObject trong Menu scene và gán button hoặc gọi trực tiếp
/// </summary>
public class TestResources : MonoBehaviour
{
    [Header("Debug Button (Optional)")]
    public Button resetResourcesButton;

    void Start()
    {
        if (resetResourcesButton != null)
            resetResourcesButton.onClick.AddListener(ResetResources);
    }

    /// <summary>
    /// Reset Gold và EXP về giá trị mặc định (999,999)
    /// </summary>
    public void ResetResources()
    {
        PlayerResourceManager.Instance.ResetResources();
        Debug.Log("[TestResources] ✓ Đã reset Gold và EXP về 999,999");
    }
}
