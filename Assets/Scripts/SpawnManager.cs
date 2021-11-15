using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject[] powerups;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private bool _keepSpawning = true;
    //[SerializeField] private GameObject _alienitePrefab;
    
    private int _waveNum = 1;
    public int enemyCount = 0;
    public bool sendNewWave = false;

    private IDictionary<int, int> _waveData = new Dictionary<int, int>();

    void Start(){
        //seed initial waveData: (waveNum, enemyCount)
        _waveData.Add(1, 3);
        StartSpawning();
    }

    void Update(){
        if(enemyCount == 0){
            SendEnemyWave(); //send current wave
        }
    }

    public void StartSpawning(){
        SendEnemyWave();
        StartCoroutine(SpawnPowerups());
        //StartCoroutine(SpawnAlienite(60f));
    }

    private void SendEnemyWave(){
        Debug.Log("Wave Num: " + _waveNum);
        enemyCount = _waveData[_waveNum];
        if(_keepSpawning){
            for(int i = 0; i < enemyCount; i++){
            GameObject enemy = Instantiate(_enemyPrefab, RandomSpawnPos(), Quaternion.identity);
            enemy.transform.parent = _enemyContainer.transform;
            }
        }
        Debug.Log("Enemies on the scene = " + GameObject.FindGameObjectsWithTag("Enemy").Length);
        //seed next wave
        _waveNum++;
        int numEnemies = _waveNum++;
        _waveData.Add(_waveNum, (numEnemies));
    }

    private Vector3 RandomSpawnPos(){
        Vector3 spawnPos = new Vector3(Random.Range(-8f, 8f), 6, 0);
        return spawnPos;
    }

    private IEnumerator SpawnEnemies(float waitToSpawn){
        //This function was called by StartCoroutine() in StartSpawning()
        yield return new WaitForSeconds(3.0f);
        Debug.Log("Keep Spawning = " + _keepSpawning);
        while(_keepSpawning){
            Vector3 spawnPos = new Vector3(Random.Range(-8f, 8f), 6, 0);
            GameObject enemy = Instantiate(_enemyPrefab, spawnPos, Quaternion.identity);
            enemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(waitToSpawn);
        }
    }

    /* private IEnumerator SpawnAlienite(float waitToSpawn){
        yield return new WaitForSeconds(60f);
        Debug.Log("Keep Spawning Alienite = " + _keepSpawning);
        while(_keepSpawning){
            Vector3 spawnPos = new Vector3(Random.Range(-8f, 8f), 6, 0);
            GameObject alienite = Instantiate(_alienitePrefab, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(waitToSpawn);
        }
    } */

    private IEnumerator SpawnPowerups(){
        yield return new WaitForSeconds(3.0f);
        while(_keepSpawning){
            int randomPowerUp = Random.Range(0, powerups.Length);
            Vector3 spawnPos = new Vector3(Random.Range(-8f, 8f), 7, 0);
            float waitTime = Random.Range(3f, 7f);
            GameObject powerupObject = Instantiate(powerups[randomPowerUp], spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(waitTime);
        }
    }

    public void OnPlayerDeath(){
        _keepSpawning = false;
    }
}
