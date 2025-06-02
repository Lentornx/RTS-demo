using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PreviewSystem : MonoBehaviour
{
    private GameObject previewObject;
    public bool ValidPlace = false;

    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
    {
        previewObject = Instantiate(prefab);
        PreparePreview(previewObject);
    }

    private void PreparePreview(GameObject previewObject)
    {
        Renderer renderer = previewObject.GetComponent<Renderer>();
        Material material = renderer.material;
        renderer.sortingLayerName = "Buildings";
        Color color = material.color;  
        color.a = 0.8f;
        renderer.material = material;
        CheckValidFloor();
    }

    public void StopShowingPreview()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
        }
    }
    private void CheckValidFloor()
    {
        Debug.Log("CheckValid");
        BoxCollider2D boxCollider2D = previewObject.GetComponent<BoxCollider2D>();
        Collider2D hit = Physics2D.OverlapBox(previewObject.transform.position, boxCollider2D.size * previewObject.transform.localScale.x, 0f, LayerMask.GetMask("Unbuildable"));
        if (hit)
        {
            ColorPreview(Color.red);
            ValidPlace = false;
            Debug.Log("HIT");
        }
        else
        {
            ColorPreview(Color.green);
            ValidPlace = true;
            Debug.Log("VALID");
        }
    }
    public void ColorPreview(Color color)
    {
        SpriteRenderer renderer = previewObject.GetComponent<SpriteRenderer>();
        Material material = renderer.material;
        color.a = 1.0f;
        material.color = color;
        
    }
    public Vector2 GetPreviewPosition()
    {
        Vector3 p = previewObject.transform.position;
        return new Vector2(p.x, p.y);
    }
    public void MovePreview(Vector2 position)
    {
        position = Camera.main.ScreenToWorldPoint(position);
        previewObject.transform.position = new Vector3(position.x,position.y);
        CheckValidFloor();
    }
}
