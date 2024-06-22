using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GemSpawner : MonoBehaviour
{
    public GameDataManager gameDataManager;
    public WhiteGhost whiteGhost;

    public float IncreaseGemWeightTimeInterval;
    public int numberOfWeightIncreaseCycles;

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

    public List<gemSpawners> spawnerList = new List<gemSpawners>();

    [System.Serializable]
    public class gems
    {
        public GameObject gem;
        public int gemWeight;
    }

    public List<gems> gemList = new List<gems>();

    int totalWeight = 0;

    public int CalculateTotalGemWeight()
    {
        foreach (gems gem in gemList)
        {
            totalWeight += gem.gemWeight;
        }
        return totalWeight;
    }

    public void SelectGem()
    {
        int randomValue;
        bool gemWasSelected = false;

        while (!gemWasSelected)
        {
            int i = 0;

            foreach (gems gem in gemList)
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

    public float GemSpawningTimer;

    private bool isSpawning = false;
    bool gemSpawnerWasHalted = false;

    public IEnumerator SpawnGems()
    {
        while (spawnerList.Count > 0)
        {
            yield return new WaitForSeconds(GemSpawningTimer);

            if (!AnyUnoccupiedSpawner())
            {
                gemSpawnerWasHalted = true;
                yield break;
            }

            SpawnGemAtRandomSpawner();
        }
    }

    private bool AnyUnoccupiedSpawner()
    {
        foreach (var spawner in spawnerList)
        {
            if (!spawner.spawnerIsOccupied)
            {
                return true;
            }
        }
        return false;
    }

    private void SpawnGemAtRandomSpawner()
    {
        bool spawnerSelected = false;

        while (!spawnerSelected)
        {
            int randomIndex = Random.Range(0, spawnerList.Count);

            if (!spawnerList[randomIndex].spawnerIsOccupied)
            {
                int gemIndex = SelectRandomGemIndex();

                Vector3 spawnPosition = spawnerList[randomIndex].spawner.transform.position;
                Instantiate(gemList[gemIndex].gem, spawnPosition, Quaternion.identity);

                spawnerList[randomIndex].OccupySpawner();
                StartCoroutine(spawnerList[randomIndex].OccupancyTimer());
                spawnerSelected = true;
            }
        }
    }

    private int SelectRandomGemIndex()
    {
        int randomValue;
        int gemIndex = 0;
        bool gemWasSelected = false;

        while (!gemWasSelected)
        {
            gemIndex = 0;

            foreach (gems gem in gemList)
            {
                randomValue = Random.Range(totalWeight, 0);
                Debug.Log(randomValue);

                if (randomValue <= gem.gemWeight)
                {
                    Debug.Log(gemIndex);
                    gemWasSelected = true;
                    break;
                }

                gemIndex++;
            }
        }

        return gemIndex;
    }

    public void IncreaseGemWeight()
    {
        gemList[1].gemWeight = gemList[1].gemWeight + 2;
        gemList[2].gemWeight = gemList[2].gemWeight + 3;
        GemSpawningTimer -= 0.3f;
        CalculateTotalGemWeight();
    }
    public void DecreaseGemWeight()
    {
        gemList[1].gemWeight = gemList[1].gemWeight - 2;
        gemList[2].gemWeight = gemList[2].gemWeight - 3;
        GemSpawningTimer += 0.3f;
        CalculateTotalGemWeight();
    }

    void Start()
    {
        StartCoroutine(SpawnGems());
        CalculateTotalGemWeight();
        StartCoroutine(IncreaseGemWeightTimer());

        // Find the game data manager
        gameDataManager = GameObject.FindGameObjectWithTag("Logic").GetComponent<GameDataManager>();
    }

    public IEnumerator IncreaseGemWeightTimer()
    {
        yield return new WaitForSeconds(3);

        int i = 0;
        while (i < numberOfWeightIncreaseCycles)
        {
            yield return new WaitForSeconds(IncreaseGemWeightTimeInterval);
            gemList[1].gemWeight = gemList[1].gemWeight + 2;
            gemList[2].gemWeight = gemList[2].gemWeight + 3;
            GemSpawningTimer -= 0.2f;
            i++;
        }
    }
}
