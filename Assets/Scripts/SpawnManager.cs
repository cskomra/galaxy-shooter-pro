using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject[] frequentPickups;
    private float frequentSpawnFrequency = 2.0f;
    private float rareSpawnFrequency = 3.0f;
    
    //TODO: Get numWaves as Player Choice @begin
    [SerializeField] private int numWavesInGame = 3;
    [SerializeField] private GameObject[] rarePickups;

    [SerializeField] private bool _keepSpawning = true;
    
    private int _waveNum = 1;
    public int enemyCount = 0;
    public bool sendNewWave = false;

    private IDictionary<int, int> _waveData = new Dictionary<int, int>();
    
    void Start(){
        initWaveData();
        StartSpawning();
    }

    void Update(){
        if(enemyCount == 0 && _waveNum <= _waveData.Count){
            SpawnEnemyWave(); //send current wave
        }
    }

    private void initWaveData(){
         for(int i = 1; i <= numWavesInGame; i++){
             //waveData: (waveNum, enemyCount)
            _waveData.Add(i, i+1);
        }
    }

    public void StartSpawning(){
        SpawnEnemyWave();
        StartCoroutine(SpawnPickups(rarePickups, rareSpawnFrequency));
        StartCoroutine(SpawnPickups(frequentPickups, frequentSpawnFrequency));
    }

    private void SpawnEnemyWave(){
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

    private IEnumerator SpawnPickups(GameObject[] powerups, float spawnFrequency){
        yield return new WaitForSeconds(spawnFrequency);
        while(_keepSpawning){
            int randomPowerUp = Random.Range(0, powerups.Length);
            Vector3 spawnPos = new Vector3(Random.Range(-8f, 8f), 7, 0);
            GameObject powerupObject = Instantiate(powerups[randomPowerUp], spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(spawnFrequency);
        }
    }

    public void OnPlayerDeath(){
        _keepSpawning = false;
    }
}
