using UnityEngine;
using TMPro;

public class BlinkTextAlpha : MonoBehaviour
{
    public float speed = 2f;
    public TextMeshProUGUI text;

    void Update()
    {
        Color c = text.color;
        c.a = Mathf.Abs(Mathf.Sin(Time.time * speed));
        text.color = c;
    }
}
