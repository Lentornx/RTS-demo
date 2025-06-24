using Assets.Buildings.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private int spawntime;
    [SerializeField] private GameObject unit;


    void Start()
    {
        resourceManager = FindAnyObjectByType<ResourceManager>();
        StartCoroutine(Spawn());
    }


    IEnumerator Spawn()
    {
        Vector3 p = transform.position;
        while (true)
        {

            yield return new WaitForSeconds(spawntime);
            GameObject newUnit = Instantiate(unit);
            newUnit.transform.position = new Vector3(p.x, p.y - 1);
        }
    }
}