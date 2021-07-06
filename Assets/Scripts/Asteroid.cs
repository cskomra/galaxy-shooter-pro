using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{

    [SerializeField]
    private float _rotateSpeed = 19.0f;

    [SerializeField]
    private GameObject _explosion;

    private GameObject _asteroidExplosion;

    private Animator _explosionAnim;

    private SpawnManager _spawnManager;

    void Start(){
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if(!_spawnManager){
            Debug.Log("Spawn Manager is NULL.");
        }
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(other.tag == "Laser"){
            _asteroidExplosion = Instantiate(_explosion, transform.position, Quaternion.identity);
            Destroy(other.gameObject);

            _spawnManager.StartSpawning();

            Destroy(this.gameObject, 0.5f);
        }
    }

}
