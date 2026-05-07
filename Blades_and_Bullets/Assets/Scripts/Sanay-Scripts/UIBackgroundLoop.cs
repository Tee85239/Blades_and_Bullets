using UnityEngine;

public class UIBackgroundLoop : MonoBehaviour
{
    public RectTransform bg1;
    public RectTransform bg2;

    [Header("Tuning")]
    public float speed = 120f;
    public float imageHeight = 482.11f;
    public float resetBelowY = -600f;
    public float topOffset = 0f;

    void Update()
    {
        Move(bg1, bg2);
        Move(bg2, bg1);
    }

    void Move(RectTransform bg, RectTransform other)
    {
        bg.anchoredPosition += Vector2.down * speed * Time.deltaTime;

        if (bg.anchoredPosition.y <= resetBelowY)
        {
            bg.anchoredPosition = new Vector2(
                bg.anchoredPosition.x,
                other.anchoredPosition.y + imageHeight + topOffset
            );
        }
    }
}