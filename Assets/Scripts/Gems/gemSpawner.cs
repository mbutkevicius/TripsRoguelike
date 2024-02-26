using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class gemSpawner : MonoBehaviour
{
    [System.Serializable]
    public class gemSpawns
    {
        public GameObject gemSpawnPoint;
        public bool spawnIsOccupied;
    }

    public List<gemSpawns> gameObjectList = new List<gemSpawns>();





    public bool GameIsActive = true;

    // Spawning timer
    public float GemSpawningTimer;

    // Lists

    // Start is called before the first frame update
    void Start()
    {
        // StartCoroutine(GemSpawningSystem());

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    // Method to select a GameObject and change its corresponding bool
    public void SelectGameObjectAndChangeBool(GameObject selectedObject, bool newBoolValue)
    {
        // Iterate through the list
        foreach (gemSpawns item in gameObjectList)
        {
            // Check if the GameObject matches the selectedObject
            if (item.gemSpawnPoint == selectedObject)
            {
                // Update the bool value
                item.spawnIsOccupied = newBoolValue;
                break; // Exit the loop since we found the matching GameObject
            }
        }
    }

}
