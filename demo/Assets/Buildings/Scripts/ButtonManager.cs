using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    
    public PlacementSystem placement;

    [field: SerializeField]
    public int ID;

    void Start()
    {
        TouchManager.Instance.OnTouchEnded += TouchHandle;
    }

    void TouchHandle(Vector2 touchPosition)
    {
        Collider2D hitCollider = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(touchPosition));

        if (hitCollider != null && hitCollider.gameObject == gameObject)
        {
            Debug.LogWarning("clicked");
            Build(touchPosition);
        }
    }

    private void Build(Vector2 position)
    {
        placement.StartPlacement(position, ID);
    }
   
    
}
