using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unitProductionButton : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

}
