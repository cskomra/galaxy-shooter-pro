using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject[] powerups;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private bool _keepSpawning = true;

    public void StartSpawning(){
        StartCoroutine(SpawnEnemies(5.0f));
        StartCoroutine(SpawnPowerups());
    }

    private IEnumerator SpawnEnemies(float waitTime){

        yield return new WaitForSeconds(3.0f);
        Debug.Log("Keep Spawning = " + _keepSpawning);
        while(_keepSpawning){
            Vector3 spawnPos = new Vector3(Random.Range(-8f, 8f), 7, 0);
            GameObject enemy = Instantiate(_enemyPrefab, spawnPos, Quaternion.identity);
            enemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(waitTime);
        }
    }

    private IEnumerator SpawnPowerups(){

        yield return new WaitForSeconds(3.0f);
        while(_keepSpawning){
            int randomPowerUp = Random.Range(0, powerups.Length);
            Vector3 spawnPos = new Vector3(Random.Range(-8f, 8f), 7, 0);
            float waitTime = Random.Range(3f, 7f);
            GameObject tripleShotPowerup = Instantiate(powerups[randomPowerUp], spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(waitTime);
        }
    }

    public void OnPlayerDeath(){
        _keepSpawning = false;
    }
}
