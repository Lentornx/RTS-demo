using Assets.Buildings.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Pathfinding;

public class ObjectPlacer : MonoBehaviour
{
    public GameObject PlaceObject(GameObject prefab, Vector3 position)
    {
        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = position;
        Renderer renderer = newObject.GetComponent<Renderer>();
        renderer.sortingLayerName = "Buildings";
        newObject.layer = LayerMask.NameToLayer("Unbuildable");

        UpdatePathfindingGraph(newObject);
        return newObject;
    }

    void UpdatePathfindingGraph(GameObject placedObject)
    {
        Physics2D.SyncTransforms(); // ¿eby collider siê zaktualizowa³
        Collider2D col = placedObject.GetComponent<Collider2D>();
        if (col != null)
        {
            Bounds bounds = col.bounds;
            GraphUpdateObject guo = new GraphUpdateObject(bounds);
            AstarPath.active.UpdateGraphs(guo);
        }
        else
        {
            Debug.LogWarning("Placed object has no Collider2D. Cannot update pathfinding graph.");
        }
    }
}
