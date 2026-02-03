using UnityEngine;
using UnityEngine.UI; // Quan trọng: Cần cái này để dùng Image
using System.Collections;

public class MapPlayerController : MonoBehaviour
{
    public static MapPlayerController Instance;

    [Header("Cài đặt")]
    public float moveSpeed = 500f; // Tăng tốc độ lên vì Map UI đơn vị pixel rất lớn

    private Image playerImage; // Đổi từ SpriteRenderer sang Image
    public bool isMoving = false;

    void Awake()
    {
        Instance = this;
        // Lấy component Image thay vì SpriteRenderer
        playerImage = GetComponent<Image>();
    }

    void Start()
    {
        // Tắt raycast để chuột có thể bấm xuyên qua người chơi vào Node bên dưới
        if (playerImage != null) playerImage.raycastTarget = false;

        LoadLeaderSprite();
    }

    void LoadLeaderSprite()
    {
        if (TeamDataManager.Instance != null)
        {
            var team = TeamDataManager.Instance.GetSelectedTeam();
            if (team != null && team.Count > 0 && team[0].avatarSprite != null)
            {
                playerImage.sprite = team[0].avatarSprite;

                // Set native size để ảnh không bị méo, sau đó chỉnh scale nếu cần
                playerImage.SetNativeSize();

                // Nếu ảnh quá to thì chỉnh scale nhỏ lại ở đây (ví dụ 0.5)
                // transform.localScale = Vector3.one * 0.5f; 
            }
        }
    }

    public void SnapToPosition(Vector3 pos)
    {
        // Vì MapPlayer giờ là con của MapContainer, ta dùng transform.position (World)
        // để nó khớp với World position của Node
        transform.position = pos;

        // Đảm bảo Z luôn = 0 trong UI
        Vector3 p = transform.localPosition;
        p.z = 0;
        transform.localPosition = p;
    }

    public void MoveToNode(Vector3 targetPos, System.Action onArrivedCallback)
    {
        if (isMoving) return;
        StartCoroutine(MoveRoutine(targetPos, onArrivedCallback));
    }

    IEnumerator MoveRoutine(Vector3 targetPos, System.Action onArrivedCallback)
    {
        isMoving = true;

        // UI thì không cần quan tâm Z âm hay dương để nổi, 
        // chỉ cần MapPlayer nằm cuối danh sách con trong Hierarchy là nổi.
        targetPos.z = transform.position.z;

        while (Vector3.Distance(transform.position, targetPos) > 1f) // UI thì sai số 1px là ok
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
        onArrivedCallback?.Invoke();
    }
}