using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Buildings.Scripts
{
    public class PlacementState
    {
        private int selectedObjectIndex = -1;
        int ID;
        PreviewSystem previewSystem;
        ObjectsDatabseSO database;
        ObjectPlacer objectPlacer;

        public PlacementState(int iD,  PreviewSystem previewSystem, ObjectsDatabseSO database, ObjectPlacer objectPlacer)
        {
            ID = iD;
            this.previewSystem = previewSystem;
            this.database = database;
            this.objectPlacer = objectPlacer;

            selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
            if (selectedObjectIndex > -1)
            {
                previewSystem.StartShowingPlacementPreview(database.objectsData[selectedObjectIndex].Prefab,
                database.objectsData[selectedObjectIndex].Size);
            }
            else
            {
                throw new System.Exception($"No object with ID {iD}");
            }
        }

        public void EndState()
        {
            previewSystem.StopShowingPreview();
        }

        public GameObject OnAction(Vector3Int gridPosition)
        {
            GameObject go = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab, gridPosition);
            return go;
        }
    }
}
