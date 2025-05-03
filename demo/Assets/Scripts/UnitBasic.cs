using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;
using TMPro;


[RequireComponent(typeof(Seeker))]
public class UnitBasic : MonoBehaviour
{
    private bool isSelected = false;
    private SpriteRenderer spriteRenderer;
    public Color selectedColor = Color.green;
    private Color defaultColor;

    public float moveSpeed = 3f;
    private Seeker seeker;
    private Path currentPath;
    private int currentWaypoint = 0;
    public float nextWaypointDistance = 0.01f;
    private bool pathPending = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultColor = spriteRenderer.color;
        seeker = GetComponent<Seeker>();
    }

    void Update()
    {
        if (currentPath == null || pathPending)
        {
            return;
        }

        if (currentWaypoint >= currentPath.vectorPath.Count)
        {
            return;
        }

        Vector3 direction = (currentPath.vectorPath[currentWaypoint] - transform.position).normalized;
        Vector3 movement = direction * moveSpeed * Time.deltaTime;
        transform.position += movement;

        if (Vector3.Distance(transform.position, currentPath.vectorPath[currentWaypoint]) < nextWaypointDistance)
        {
            currentWaypoint++;
        }
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

    public void MoveTo(Vector2 targetPosition)
    {
        if (seeker.IsDone())
        {
            pathPending = true;
            seeker.StartPath(transform.position, targetPosition, OnPathComplete);
        }
    }

    private void OnPathComplete(Path p)
    {
        pathPending = false;
        if (!p.error)
        {
            currentPath = p;
            currentWaypoint = 0;
        }
        else
        {
            Debug.LogError("Pathfinding error: " + p.errorLog);
        }
    }
}
