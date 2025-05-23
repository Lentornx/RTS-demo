using Assets.Buildings.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    public GameObject PlaceObject(GameObject prefab, Vector3 position)
    {
        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = position;
        Renderer renderer = newObject.GetComponent<Renderer>();
        renderer.sortingLayerName = "Buildings";
        newObject.layer = LayerMask.NameToLayer("Unbuildable");

        return newObject;
    }
}
