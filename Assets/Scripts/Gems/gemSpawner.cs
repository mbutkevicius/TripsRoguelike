using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static gemSpawner;

public class gemSpawner : MonoBehaviour
{
    public GameObject gem;


    // Class to store info about spawner locations and occpancy status
    [System.Serializable]
    public class gemSpawners
    {
        public GameObject spawner;
        public bool spawnerIsOccupied;

        public void OccupySpawner()
        {
            spawnerIsOccupied = true;
        }

        public IEnumerator OccupancyTimer()
        {
            yield return new WaitForSeconds(10);
            spawnerIsOccupied = false;
        }
    }  

    // Makes a list for the spawner class in the inspector that can be populated
    public List<gemSpawners> spawnerList = new List<gemSpawners>();

  



    // Spawning timer
    public float GemSpawningTimer;


    private bool isSpawning = false;
    bool gemSpawnerWasHalted = false;
    bool spawnerSelected = false;
    public IEnumerator SpawnGems()
    {
        while(spawnerList.Count > 0)
        {
            // initiate the spawning timer
            yield return new WaitForSeconds(GemSpawningTimer);
            Debug.Log("Started Timer");

            // Check if there are any unoccupied spawners left
            bool unoccupiedSpawnerExists = false;
            foreach (var spawner in spawnerList)
            {
                if (!spawner.spawnerIsOccupied)
                {
                    unoccupiedSpawnerExists = true;
                    break;
                }
            }

            // If there are no unoccupied spawners, exit the coroutine
            if (!unoccupiedSpawnerExists)
            {
                Debug.Log("All spawners occupied. Exiting coroutine.");
                gemSpawnerWasHalted = true;
                yield break;
            }

            // Enter a loop that will iterate through spawners until it finds an empty one
            while (spawnerSelected == false)
            {
                // Choose a random index from the spawner list
                int randomIndex = Random.Range(0, spawnerList.Count);
                Debug.Log(randomIndex);

                if (spawnerList[randomIndex].spawnerIsOccupied == false)
                {
                    // Get the position of the object at the random index
                    Vector3 spawnPosition = spawnerList[randomIndex].spawner.transform.position;
                    Debug.Log(spawnPosition);

                    // Instantiate the object at the position
                    Instantiate(gem, spawnPosition, Quaternion.identity);

                    spawnerList[randomIndex].OccupySpawner();
                    StartCoroutine(spawnerList[randomIndex].OccupancyTimer());
                    spawnerSelected = true;
                }
            }
            spawnerSelected = false;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnGems());
    }

    // Update is called once per frame
    void Update()
    {
    
    }

}
