using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static gemSpawner;

public class gemSpawner : MonoBehaviour
{
    // Class to store info about spawner locations and occupancy status
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


    // Class to store info about gem types and weight
    [System.Serializable]
    public class gems
    {
        public GameObject gem;
        public int gemWeight;
    }

    // Makes a list for the gem class in the inspector that can be populated
    public List<gems> gemList = new List<gems>();



    // Calculate total weight of all gems
    int totalWeight = 0;
    public int CalculateTotalGemWeight()
    {
        foreach (gems gem in gemList)
        {
            totalWeight += gem.gemWeight;
        }
        return totalWeight;
    }


    public void selectGem()
    {
        int randomValue;
        bool gemWasSelected = false;

        while (gemWasSelected == false)
        {
            int i = 0;

            foreach (gems gems in gemList)
            {
                randomValue = Random.Range(totalWeight, 0);
                Debug.Log(randomValue);

                if (randomValue <= gemList[i].gemWeight)
                {
                    Debug.Log(i);
                    gemWasSelected = true;
                    break;
                }

                i++;
            }
        }
    }



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
                // ("All spawners occupied. Exiting coroutine.");
                gemSpawnerWasHalted = true;
                yield break;
            }

            // Enter a loop that will iterate through spawners until it finds an empty one
            while (spawnerSelected == false)
            {
                // represents the selected gem
                // Choose a random index from the spawner list
                int randomIndex = Random.Range(0, spawnerList.Count);
                // Debug.Log(randomIndex);
                if (spawnerList[randomIndex].spawnerIsOccupied == false)
                {
                    int randomValue;
                    
                    bool gemWasSelected = false;

                    while (gemWasSelected == false)
                    {
                        int gemIndex = 0;

                        foreach (gems gems in gemList)
                        {
                            randomValue = Random.Range(totalWeight, 0);
                            Debug.Log(randomValue);

                            if (randomValue <= gemList[gemIndex].gemWeight)
                            {
                                Debug.Log(gemIndex);
                                gemWasSelected = true;
                                // Get the position of the object at the random index
                                Vector3 spawnPosition = spawnerList[randomIndex].spawner.transform.position;

                                // Instantiate the object at the position
                                Instantiate(gemList[gemIndex].gem, spawnPosition, Quaternion.identity);
                                break;
                            }

                            gemIndex++;
                        }
                    }

                 

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
        CalculateTotalGemWeight();
    }

    // Update is called once per frame
    void Update()
    {
    
    }

}
