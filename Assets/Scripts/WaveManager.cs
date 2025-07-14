using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public Transform spawnPoint;
    public Transform enemyFolder;

    [System.Serializable]
    public class Wave
    {
        public EnemyController enemyToSpawn;
        public int amountToSpawn;
    }

    public Wave[] waveList;
    public int currentWaveNumber;

    [ContextMenu("Force Spawn Next Wave")]
    public void SpawnNextWave()
    {
        for (int i = 0; i < waveList[currentWaveNumber].amountToSpawn; i++)
        {
            EnemyController newEnemy = Instantiate(waveList[currentWaveNumber].enemyToSpawn, enemyFolder);
            newEnemy.transform.position = spawnPoint.position;
        }
        currentWaveNumber++;
    }
}
