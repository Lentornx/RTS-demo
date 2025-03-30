using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }

    private List<UnitBasic> selectedUnits = new List<UnitBasic>();
    private UnitBasic selectedUnit;

    private float touchStartTime = 0;
    private Vector2 touchWorldPosition = Vector2.zero;
    private Vector2 touchScreenPosition = Vector2.zero;
    private Vector3Int touchGridPosition = Vector3Int.zero;
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
        PopulateTilemapsFromGrid();
        gridLayout = gridParent.GetComponent<GridLayout>();

        TouchManager.Instance.OnTouchBegan += HandleTouchBegan;
        TouchManager.Instance.OnTouchEnded += HandleTouchEnded;
    }
    void PopulateTilemapsFromGrid()
    {
        tilemaps = new List<Tilemap>();
        foreach (Transform child in gridParent)
        {
            Tilemap tilemap = child.GetComponent<Tilemap>();
            if (tilemap != null)
            {
                tilemaps.Add(tilemap);
            }
        }
    }

    void HandleTouchBegan(Vector2 touchPosition)
    {
        touchStartTime = Time.time;
        touchWorldPosition = Camera.main.ScreenToWorldPoint(touchPosition);
        touchScreenPosition = touchPosition;
        touchGridPosition = GetGridPosition(touchWorldPosition);

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
                if (unit != null)
                {
                    ToggleSelection(unit);
                }
            }
            else if (selectedUnit != null)
            {
                Vector2 targetPosition = Camera.main.ScreenToWorldPoint(touchEndedPosition);
                if (CanMoveTo(touchGridPosition))
                {
                    selectedUnit.MoveTo(targetPosition);
                }
            }
        }
    }

    bool CanMoveTo(Vector3Int gridPosition)
    {
        bool canMove = true;
        Tilemap topTilemap = GetTopTilemap(gridPosition);

        if (topTilemap != null)
        {
            TileBase tile = topTilemap.GetTile(gridPosition);

            if (tile != null && topTilemap.CompareTag("Obstacle"))
            {
                canMove = false;
            }
        }
        return canMove;
    }


    void ToggleSelection(UnitBasic unit)
    {
        // deselect if same unit
        if (selectedUnit == unit)
        {
            selectedUnit.Deselect();
            selectedUnit = null;
        }
        // deselect old unit and select new unit
        else
        {
            if (selectedUnit != null)
            {
                selectedUnit.Deselect();
            }
            selectedUnit = unit;
            selectedUnit.Select();
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

    // ignores Decoration tilemaps
    public Tilemap GetTopTilemap(Vector3Int gridPosition)
    {
        Tilemap topTilemap = null;
        int topTileOrderInLayer = int.MinValue;

        foreach (var tilemap in tilemaps)
        {
            if (tilemap.CompareTag("Decoration"))
                continue;

            TileBase tile = tilemap.GetTile(gridPosition);
            if (tile != null)
            {
                Renderer renderer = tilemap.GetComponent<Renderer>();

                if (renderer != null && renderer.sortingOrder > topTileOrderInLayer)
                {
                    topTileOrderInLayer = renderer.sortingOrder;
                    topTilemap = tilemap;
                }
            }
        }
        return topTilemap;
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
