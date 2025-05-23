using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Buildings.Scripts
{
    [CreateAssetMenu]
    public class UnitDatabase : ScriptableObject
    {
        public List<UnitData> objectsData;


        public UnitData GetObjectByID(int id)
        {
            foreach (UnitData obj in objectsData)
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
    public class UnitData
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
        public float productionTime { get; private set; }

        [field: SerializeField]
        public int requiredWood { get; private set; }

    }
}
