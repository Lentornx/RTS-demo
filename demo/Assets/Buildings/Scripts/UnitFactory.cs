using Assets.Buildings.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFactory : MonoBehaviour
{
    private GameObject building;
    public Sprite unitProducingSprite;
    [SerializeField] private TouchManager touchManager;

    [SerializeField] private UnitDatabase database;

    [SerializeField] private unitProductionButton unitbutton;

    [SerializeField] private ResourceManager resourceManager;

    //[SerializeField] private UnitManager unitManager;
    [SerializeField] private PlacementSystem placementSystem;

    private bool producingMode = false;


    void Start()
    {
        building = null;
        touchManager.OnTouchBegan += HandleTouch;
    }

    private void HandleTouch(Vector2 position)
    {
        if (Input.touchCount > 0)
        {
            Debug.Log("Touch OK");
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log("hit OK");
                var component = hit.collider.GetComponent<Component>();

                if (component.CompareTag("UnitButton"))
                {
                   if(producingMode)
                    {
                        //TO DO funkcja do pobierania id z przycisku
                        int id = 0;
                        UnitData unit = database.GetObjectByID(id);
                        if(resourceManager.CanUseResource(unit.requiredWood,"wood"))
                        {
                            Vector3 p = building.transform.position;
                            ProduceUnit(id, p);
                            unitbutton.SetActive(false);
                        }
                    }
                }
                else if (component.CompareTag("Building"))
                {
                    if(placementSystem.buildingState == null)
                    {
                        producingMode = true;
                        building = hit.collider.gameObject;
                        SpriteRenderer sr = building.GetComponent<SpriteRenderer>(); // tutaj wstawiam tymczasowa nalepke by nie pokazywalo guzika unit factory przy farmie itp
                        if (sr.sprite == unitProducingSprite) 
                            unitbutton.SetActive(true);
                    }
                }
                else
                {
                    producingMode = false;
                    building = null;
                    unitbutton.SetActive(false);
                }
            }
            else
            {
                producingMode = false;
                building = null;
                unitbutton.SetActive(false);
            }
        }
    }

    private void ProduceUnit(int id, Vector3 p)
    {
        UnitData unit = database.GetObjectByID(id);

        StartCoroutine(ProdukcjaJednostki(unit, p));

    }
    IEnumerator ProdukcjaJednostki(UnitData unit, Vector3 p)
    {
        yield return new WaitForSeconds(unit.productionTime);
        GameObject newUnit = Instantiate(unit.Prefab);

        //TODO stawianie na razie na sztywno, potem jednostka bêdzie to jakoœ musiala rozpatrywac

        newUnit.transform.position = new Vector3(p.x - 2,p.y);

        //newUnit.transform.parent = unitManager;
    }
}
