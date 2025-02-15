using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickTracker : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false; 
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(touch.position);
            transform.position = new Vector3(worldPosition.x, worldPosition.y, 0);

            if (touch.phase == TouchPhase.Began)
            {
                spriteRenderer.enabled = true; 
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                spriteRenderer.enabled = false; 
            }
        }
        else
        {
            spriteRenderer.enabled = false; 
        }
    }
}
