using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Buildings.Scripts
{
    public class GreenButton : MonoBehaviour
    {
        private void Awake()
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();
            Material material = renderer.material;
            material.color = Color.green;
        }
        void Start()
        {
            gameObject.SetActive(false);
            
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        void Update()
        {

        }
    }
}
