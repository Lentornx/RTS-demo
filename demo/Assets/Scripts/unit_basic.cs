using UnityEngine;

public class UnitBasic : MonoBehaviour
{
    private bool isSelected = false;
    private SpriteRenderer spriteRenderer;
    public Color selectedColor = Color.green;
    private Color defaultColor;

    public float moveSpeed = 5f;
    private Vector2 targetPosition;
    private bool moving = false;

    float touchStartTime = 0;
    Vector2 touchMapPosition = Vector2.zero;
    Vector2 touchScreenPosition = Vector2.zero;
    Collider2D hitCollider = null;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultColor = spriteRenderer.color;
        targetPosition = transform.position;

        TouchManager.Instance.OnTouchBegan += HandleTouchBegan;
        TouchManager.Instance.OnTouchEnded += HandleTouchEnded;
    }

    void Update()
    {
        if (moving)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                moving = false;
            }
        }
    }

    void HandleTouchBegan(Vector2 touchPosition)
    {
        touchStartTime = Time.time;
        touchMapPosition = Camera.main.ScreenToWorldPoint(touchPosition); // what place we clicked on the world 
        touchScreenPosition = touchPosition; // where we clicked on the screen

        hitCollider = Physics2D.OverlapPoint(touchMapPosition);
    }

    void HandleTouchEnded(Vector2 touchPosition)
    {
        if (Vector2.Distance(touchPosition, touchScreenPosition) < 100f && Time.time - touchStartTime < 1f) // check if not dragging or holding
        {
            if (hitCollider != null && hitCollider.gameObject == gameObject)
            {
                ToggleSelection();
            }
            else if (isSelected)
            {
                targetPosition = Camera.main.ScreenToWorldPoint(touchPosition);
                moving = true;
            }
        }
    }

    void ToggleSelection()
    {
        isSelected = !isSelected;
        spriteRenderer.color = isSelected ? selectedColor : defaultColor;
    }

    void OnDestroy()
    {
        if (TouchManager.Instance != null)
        {
            TouchManager.Instance.OnTouchBegan -= HandleTouchBegan;
            TouchManager.Instance.OnTouchEnded -= HandleTouchEnded;
        }
    }
}
