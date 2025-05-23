using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class TouchManager : MonoBehaviour
{
    public static TouchManager Instance; 

    public event Action<Vector2> OnTouchBegan; 
    public event Action<Vector2> OnTouchEnded;
    public event Action<Vector2> OnHold;
    public event Action<Vector2, Vector2> OnDrag; 

    private Vector2 lastTouchPosition;
    private bool isDragging = false;
    private bool isHolding = false;
    private float touchStartTime;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (Input.touchCount == 1) 
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartTime = Time.time;
                isDragging = true; // probably not needed, need to read a bit more about Touch library
                lastTouchPosition = touch.position;
                OnTouchBegan?.Invoke(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                OnDrag?.Invoke(lastTouchPosition, touch.position);
                lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isDragging = false;
                isHolding = false;
                OnTouchEnded?.Invoke(touch.position);
            }

            if(Time.time - touchStartTime > 5f && !isHolding && touch.phase != TouchPhase.Ended)
            {
                isHolding = true;
                OnHold?.Invoke(touch.position);
            }
        }
    }
    public Vector2 GetSelectedMapPosition()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            //Debug.Log("Zmieniono pozycje");
            lastTouchPosition =  touch.position;
        }
        return lastTouchPosition;
    }

 

}
