using Assets.Buildings.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private TouchManager touchManager;

    [SerializeField] private ObjectsDatabseSO database;

    [SerializeField] private PreviewSystem previewSystem;

    private Vector2Int lastDetectedPosition = Vector2Int.zero;

    [SerializeField] private ObjectPlacer objectPlacer;

    [SerializeField] private GreenButton greenButton;

    [SerializeField] private RedButton redButton;

    [SerializeField] private ResourceManager resourceManager;

    int selectedID;

    public PlacementState buildingState = null;

    private ObjectData buildingfromDB = null;
   
    public void StartPlacement(Vector2 position, int building_id)
    {
        Debug.Log("StartPlacement");

        StopPlacement(position);

        selectedID = building_id;

        buildingfromDB = database.GetObjectByID(selectedID);

        buildingState = new PlacementState(selectedID, previewSystem, database, objectPlacer);

        greenButton.SetActive(true);
        redButton.SetActive(true);

        touchManager.OnTouchBegan += HandleTouch;
    }
    private void PlaceStructure(Vector2 position)
    {
            Vector3Int positionInWorld = new Vector3Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), 0);
            Debug.Log(positionInWorld);

            GameObject go = buildingState.OnAction(positionInWorld);
 
            //BIERZEMY OBIEKT Z BAZY DANYCH BUDYNKOW
            ObjectData ob = database.GetObjectByID(selectedID);
            
            StopPlacement(position);    
    }


    private void StopPlacement(Vector2 position)
    {
        if (buildingState == null)
            return;

        buildingState.EndState();

        touchManager.OnTouchBegan -= HandleTouch;
        touchManager.OnTouchEnded -= StopPlacement;

        lastDetectedPosition = Vector2Int.zero;
        greenButton.SetActive(false);
        redButton.SetActive(false);

        buildingState = null;
        buildingfromDB = null;
    }

    private void HandleTouch(Vector2 position)
    {
        if (Input.touchCount > 0)
        {
           
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);
           
           
            if(hit.collider != null)
            {
                var component = hit.collider.GetComponent<Component>();

                if (component.CompareTag("GreenButton"))
                {
                    if (previewSystem.ValidPlace && resourceManager.CanUseResource(buildingfromDB.requiredWood,"wood"))
                    {
                        position = previewSystem.GetPreviewPosition();
                        PlaceStructure(position);
                    }
                }
                else if (component.CompareTag("RedButton"))
                {
                    previewSystem.StopShowingPreview();
                    StopPlacement(position);
                }
                else
                {
                    previewSystem.MovePreview(position);
                }
            }
            else 
            {
                previewSystem.MovePreview(position);
            }
            
            
        }
    }
}
