using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }

    private List<UnitBasic> selectedUnits = new List<UnitBasic>();

    private float touchStartTime = 0;
    private Vector2 touchWorldPosition = Vector2.zero;
    private Vector2 touchScreenPosition = Vector2.zero;
    private Collider2D hitCollider = null;

    public Transform gridParent;
    private GridLayout gridLayout;
    private List<Tilemap> tilemaps;
    public List<Tilemap> Tilemaps => tilemaps;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        gridLayout = gridParent.GetComponent<GridLayout>();

        TouchManager.Instance.OnTouchBegan += HandleTouchBegan;
        TouchManager.Instance.OnTouchEnded += HandleTouchEnded;
    }

    void HandleTouchBegan(Vector2 touchPosition)
    {
        touchStartTime = Time.time;
        touchWorldPosition = Camera.main.ScreenToWorldPoint(touchPosition);
        touchScreenPosition = touchPosition;

        hitCollider = Physics2D.OverlapPoint(touchWorldPosition);
    }

    void HandleTouchEnded(Vector2 touchEndedPosition)
    {
        // check if user tapped, ignore drag and hold gestures
        if (Vector2.Distance(touchEndedPosition, touchScreenPosition) < 100f && Time.time - touchStartTime < 1f)
        {
            if (hitCollider != null)
            {
                UnitBasic unit = hitCollider.GetComponent<UnitBasic>();
                if (unit != null && unit.faction == "Player")
                {
                    ToggleSelection(unit);
                    return;
                }
            }

            Vector2 targetPosition = Camera.main.ScreenToWorldPoint(touchEndedPosition);

            selectedUnits.RemoveAll(u => u == null); // remove dead units
            foreach (UnitBasic selectedUnit in selectedUnits)
            {
                selectedUnit.MoveTo(targetPosition);
            }
        }
    }

    void ToggleSelection(UnitBasic unit)
    {
        if (selectedUnits.Contains(unit))
        {
            // deselect and remove the unit
            unit.Deselect();
            selectedUnits.Remove(unit);
        }
        else
        {
            // select and add the unit
            unit.Select();
            selectedUnits.Add(unit);
        }
    }

    public Vector3Int GetGridPosition(Vector2 worldPosition)
    {
        if (gridLayout != null)
        {
            Vector2 cellSize = gridLayout.cellSize;

            // convert world position to grid position
            Vector3 localPosition = gridParent.InverseTransformPoint(worldPosition);
            return new Vector3Int(
                Mathf.FloorToInt(localPosition.x / cellSize.x),
                Mathf.FloorToInt(localPosition.y / cellSize.y),
                0
            );
        }
        else
        {
            Debug.LogError("GridLayout component not found on gridParent.");
            return Vector3Int.zero;
        }
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
