using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
    public Transform spawnPoint;
    public Transform enemyFolder;

    public Geyser[] allGeysers;
    public List<Geyser> emptyGeysers = new();

    [System.Serializable]
    public class Wave
    {
        public EnemySpawnGroup[] enemies;
    }

    [System.Serializable]
    public class EnemySpawnGroup
    {
        public EnemyController enemyToSpawn;
        public int amountToSpawn;
    }

    public Wave[] waveList;
    public int currentWaveNumber;
    public float timeBetweenEnemySpawns;

    public void UpdateEmptyGeysers()
    {
        allGeysers = FindObjectsByType<Geyser>(FindObjectsSortMode.None);
        emptyGeysers = allGeysers.ToList();
        emptyGeysers.Clear();
        foreach (Geyser geyser in allGeysers)
        {
            if (!geyser.hasPump)
            {
                emptyGeysers.Add(geyser);
            }
        }
    }

    [ContextMenu("Force Spawn Next Wave")]
    public void DebugSpawnNextWave()
    {
        //currentWaveNumber++;
        StartCoroutine(SpawnWave(waveList[0]));
    }
    private IEnumerator SpawnWave(Wave wave)
    {
        int currentEnemyGroup = 0;
        while (currentEnemyGroup < wave.enemies.Length)
        {
            int currentEnemyNumber = 0;
            while (currentEnemyNumber < wave.enemies[currentEnemyGroup].amountToSpawn)
            {
                EnemyController newEnemy = Instantiate(wave.enemies[currentEnemyGroup].enemyToSpawn, enemyFolder);
                newEnemy.transform.position = emptyGeysers[Random.Range(0, emptyGeysers.Count)].transform.position;
                currentEnemyNumber++;
                yield return new WaitForSeconds(timeBetweenEnemySpawns);
            }
            currentEnemyGroup++;
            yield return new WaitForSeconds(timeBetweenEnemySpawns);
        }

        if (wave == waveList[waveList.Length - 1])
        {
            
        }
        yield return null;
    }   
}
