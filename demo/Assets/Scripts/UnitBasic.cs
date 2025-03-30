using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class UnitBasic : MonoBehaviour
{
    private bool isSelected = false;
    private SpriteRenderer spriteRenderer;
    public Color selectedColor = Color.green;
    private Color defaultColor;

    public float moveSpeed = 5f;
    private Vector2 targetPosition;
    private bool moving = false;

    private List<(string, string)> validTransitions = new List<(string, string)>();

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultColor = spriteRenderer.color;
        targetPosition = transform.position;
        InitializeValidTransitions();
    }

    void Update()
    {
        // TODO some kind of pathing to avoid obstacles
        if (moving)
        {
            Vector2 newPosition = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Check if the unit encounters an invalid tile
            if (!CanMoveTo(transform.position, newPosition))
            {
                moving = false;
            }
            else if (Vector2.Distance(newPosition, targetPosition) < 0.1f)
            {
                moving = false;
                transform.position = newPosition;
            }
            else
            {
                transform.position = newPosition;
            }
        }
    }

    bool CanMoveTo(Vector2 currentPosition, Vector2 nextPosition)
    {
        // FIXME detecting tiles in tilemaps with complex geometry (e.g. drzewa) is bugged
        UnitManager unitManager = UnitManager.Instance;

        Vector3Int currentGridPosition = unitManager.GetGridPosition(currentPosition);
        Tilemap currentTilemap = unitManager.GetTopTilemap(currentGridPosition);
        TileBase currentTile = currentTilemap.GetTile(currentGridPosition);

        Vector3Int nextGridPosition = unitManager.GetGridPosition(nextPosition);
        Tilemap nextTilemap = unitManager.GetTopTilemap(unitManager.GetGridPosition(nextPosition));
        TileBase nextTile = nextTilemap.GetTile(nextGridPosition);

        if (currentTile != null && nextTile != null)
        {
            string currentTileType = currentTilemap.name;
            string nextTileType = nextTilemap.name;

            return IsValidTransition(currentTileType, nextTileType);
        }
        return false;
    }


    bool IsValidTransition(string fromTileType, string toTileType)
    {
        if (fromTileType == toTileType)
        {
            return true;
        }
        Debug.Log("From " + fromTileType + " to " + toTileType);
        return validTransitions.Exists(pair =>
            (pair.Item1 == fromTileType && pair.Item2 == toTileType) ||
            (pair.Item1 == toTileType && pair.Item2 == fromTileType));
    }

    void InitializeValidTransitions()
    {
        validTransitions.Add(("trawa", "kamienie - 1-schody"));
        validTransitions.Add(("trawa", "pomosty"));
        validTransitions.Add(("kamienie - 1", "kamienie - 1-schody"));
        validTransitions.Add(("kamienie - 1", "kamienie - 2-schody"));
        validTransitions.Add(("kamienie - 1", "pomosty"));
        validTransitions.Add(("trawa - piêtro", "kamienie - 1-schody"));
        validTransitions.Add(("trawa - piêtro", "kamienie - 2-schody"));
        validTransitions.Add(("kamienie - 2", "kamienie - 2-schody"));
    }

    public void Select()
    {
        isSelected = true;
        spriteRenderer.color = selectedColor;
    }

    public void Deselect()
    {
        isSelected = false;
        spriteRenderer.color = defaultColor;
    }

    public void MoveTo(Vector2 position)
    {
        targetPosition = position;
        moving = true;
    }
}
