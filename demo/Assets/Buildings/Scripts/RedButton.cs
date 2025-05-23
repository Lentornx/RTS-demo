using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedButton : MonoBehaviour
{
    private void Awake()
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        Material material = renderer.material;
        material.color = Color.red;
    }

    void Start()
    {
        gameObject.SetActive(false);
       
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
