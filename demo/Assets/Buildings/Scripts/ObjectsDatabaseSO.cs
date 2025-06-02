using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Buildings.Scripts
{
    [CreateAssetMenu]
    public class ObjectsDatabseSO : ScriptableObject
    {
        public List<ObjectData> objectsData;


        public ObjectData GetObjectByID(int id)
        {
            foreach (ObjectData obj in objectsData)
            {
                if (obj.ID == id)
                {
                    return obj;
                }
            }

            return new(); 
        }

    }

    [System.Serializable]
    public class ObjectData
    {
        [field: SerializeField]
        public string Name { get; private set; }

        [field: SerializeField]
        public int ID { get; private set; }

        [field: SerializeField]
        public Vector2Int Size { get; private set; } = Vector2Int.one;

        [field: SerializeField]
        public GameObject Prefab { get; private set; }

        [field: SerializeField]
        public int requiredWood { get; private set; }
    }
}
