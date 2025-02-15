using UnityEngine;

public class UnitTouchController : MonoBehaviour
{
    private bool isSelected = false;
    private SpriteRenderer spriteRenderer;
    public Color selectedColor = Color.green;
    private Color defaultColor;

    public float moveSpeed = 5f; 
    private Vector3 targetPosition;
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
    }

    void Update()
    {
        HandleTouchInput();
        if (moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                moving = false;
            }
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0) 
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touchStartTime = Time.time;
                touchMapPosition = Camera.main.ScreenToWorldPoint(touch.position); //what place we clicked on the world 
                touchScreenPosition = touch.position; //where we clicked on the screen

                hitCollider = Physics2D.OverlapPoint(touchMapPosition);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (Vector2.Distance(touch.position, touchScreenPosition) < 1f && Time.time - touchStartTime < 1f)
                {
                    if (hitCollider != null && hitCollider.gameObject == gameObject)
                    {
                        ToggleSelection();
                    }
                    else if (isSelected)
                    {
                        targetPosition = touchMapPosition;
                        moving = true;
                    }
                }
            }
        }
    }

    void ToggleSelection()
    {
        isSelected = !isSelected;
        spriteRenderer.color = isSelected ? selectedColor : defaultColor;
    }
}
