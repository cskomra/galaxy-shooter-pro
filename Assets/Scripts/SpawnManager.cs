using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject[] _frequentPickups;
    [SerializeField] private GameObject[] _rarePickups;

    private float _frequentSpawnFrequency = 2.0f;
    private float _rareSpawnFrequency = 3.0f;

    //TODO: Get numWaves as Player Choice @begin

    [SerializeField] private int _numWavesInGame = 3;
    [SerializeField] private bool _keepSpawning = true;
    
    private int _waveNum = 1;
    public int _enemyCount = 0;
    public bool _sendNewWave = false;

    private IDictionary<int, int> _waveData = new Dictionary<int, int>();
    
    void Start(){
        initWaveData();
        StartSpawning();
    }

    void Update(){
        if(_enemyCount == 0 && _waveNum <= _waveData.Count){
            SpawnEnemyWave(); //send current wave
        }
    }

    private void initWaveData(){
         for(int i = 1; i <= _numWavesInGame; i++){
             //waveData: (waveNum, enemyCount)
            _waveData.Add(i, i+1);
        }
    }

    public void StartSpawning(){
        SpawnEnemyWave();
        StartCoroutine(SpawnPickups(_rarePickups, _rareSpawnFrequency));
        StartCoroutine(SpawnPickups(_frequentPickups, _frequentSpawnFrequency));
    }

    private void SpawnEnemyWave(){
        _enemyCount = _waveData[_waveNum];
        if(_keepSpawning){
            for(int i = 0; i < _enemyCount; i++){
                if(i%2 != 0){
                    //odd enemy --> enable shield
                    GameObject enemy = Instantiate(_enemyPrefab, RandomSpawnPos(), Quaternion.identity);
                    enemy.transform.parent = _enemyContainer.transform;
                    enemy.GetComponent<Enemy>().enemyShield.SetActive(true);
                }else{
                    GameObject enemy = Instantiate(_enemyPrefab, RandomSpawnPos(), Quaternion.identity);
                    enemy.transform.parent = _enemyContainer.transform;
                }
                
            }
        }
        //seed next wave
        _waveNum++;
    }

    private Vector3 RandomSpawnPos(){
        Vector3 spawnPos = new Vector3(Random.Range(-8f, 8f), 6, 0);
        return spawnPos;
    }

    private IEnumerator SpawnPickups(GameObject[] powerups, float spawnFrequency){
        Debug.Log("Pickup TAG: " + powerups[0].tag);
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
