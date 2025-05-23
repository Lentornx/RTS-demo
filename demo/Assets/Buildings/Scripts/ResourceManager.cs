using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{

    private int wood = 50;
    
    void Start()
    {
       
    }

    public bool CanUseResource(int amount, string name)
    {
        if (name == "wood")
        {
            if(wood >= amount)
            {
                wood -= amount;
                return true;
            }
        }
        return false;
    }

    public void ProduceResource(int amount, string name)
    {
        if (name == "wood")
        {
            Debug.Log(wood);
            wood += amount;
        }
    }


    
    
}
