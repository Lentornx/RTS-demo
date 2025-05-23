using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceProducer : MonoBehaviour
{

    [SerializeField] private float producingTime;

    [SerializeField]
    private string resourceName;

    [SerializeField]
    private int resourceAmountPerSecond;

    [SerializeField] private ResourceManager resourceManager;

    [SerializeField] private PlacementSystem placementSystem;

    void Start()
    {
        resourceManager = FindAnyObjectByType<ResourceManager>();
        placementSystem = FindAnyObjectByType<PlacementSystem>();
        StartCoroutine(ProduceResource());
    }

    IEnumerator ProduceResource()
    {
        while (true)
        {
            yield return new WaitForSeconds(producingTime);

            if(placementSystem.buildingState == null)
            {
                GameObject building = this.gameObject;
                CircleCollider2D circleCollider2D = building.GetComponent<CircleCollider2D>();
                Collider2D[] colliders = Physics2D.OverlapCircleAll(building.transform.position, circleCollider2D.radius, LayerMask.GetMask("Unbuildable"));

                foreach (Collider2D c in colliders)
                {
                    if (c.gameObject.tag == "Worker")
                    {
                        resourceManager.ProduceResource(resourceAmountPerSecond, resourceName);
                    }
                }
            }
           
        }
    }

}
