using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float swipingSpeed = 0.5f;

    void Start()
    {
        TouchManager.Instance.OnDrag += HandleDrag;
    }

    void HandleDrag(Vector2 start, Vector2 end)
    {
        Vector2 delta = end - start;
        Vector3 move = new Vector3(-delta.x, -delta.y, 0) * swipingSpeed * Time.deltaTime;
        transform.position += move;
    }

    void OnDestroy()
    {
        if (TouchManager.Instance != null)
            TouchManager.Instance.OnDrag -= HandleDrag;
    }
}
